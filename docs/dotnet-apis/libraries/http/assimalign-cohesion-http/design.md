# Assimalign.Cohesion.Http design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

## Trailers

`IHttpRequest.Trailers` and `IHttpResponse.Trailers` are the trailer sections of RFC 9110 §6.5,
distinct from the headers. `IsSupported` is a capability signal: whether this exchange can carry a
trailer section. When it is `false`, the collection is empty and adding to it throws
`InvalidOperationException`, so a server that adds trailers to an exchange that cannot transmit them
fails where the mistake is made rather than silently dropping them on the wire. The shared
`HttpTrailerCollection.Unsupported` is the default, which is what the default interface members and
the abstract `HttpRequest` and `HttpResponse` bases return; a transport that surfaces or sends
trailers overrides them.

The `Assimalign.Cohesion.Http.Connections` transports report it per direction and version (decision
18, the Http area's ADR 2 in `cohesion/docs/libraries/Http/DECISIONS.md`):

| Version | `Request.Trailers` | `Response.Trailers` |
|---|---|---|
| HTTP/1.1 | Supported for a chunked request; unsupported otherwise | Unsupported |
| HTTP/2 | Supported | Supported: sent as a HEADERS frame that ends the stream |
| HTTP/3 | Supported | Supported: sent as a HEADERS frame before the stream's FIN |

- **Request trailers** are filled once the body has been read to its end. Every version holds a
  received trailer section to one rule set: a connection-specific field, or a field in the
  `HttpFieldRules.IsProhibitedInTrailers` set (RFC 9110 §6.5.1: framing, routing, request
  modifiers, authentication, content-processing controls, and `Trailer` itself), makes the request
  malformed. HTTP/1.1's chunked reader applies it too.
- **A supported response collection** also refuses, when they are added, the fields a trailer
  section cannot carry — pseudo-headers, a name that is not a token, connection-specific fields,
  the `IsProhibitedInTrailers` set, and a value holding a control character other than HTAB — with
  `ArgumentException`.
- **A response to `HEAD` sends no trailers**, and a `CONNECT` exchange reports the response
  collection unsupported, since its stream becomes a DATA-only tunnel.
- **HTTP/1.1 response trailers stay out** (decision 18): a buffered HTTP/1.1 response carries
  `Content-Length`, and HTTP/1.1 clients rarely consume chunked trailers. If a consumer appears, the
  model makes it a drop-in: `IsSupported = true` and a populated collection.

Trailers were decided as HTTP semantics, apart from gRPC, which stays outside the HTTP/Web program.

## Repeated fields combine in linear time

`HttpHeaderValue` stores either a single string or several, so repeated field lines are preserved
without forcing a lossy early join; the comma-folded `Value` is computed on demand.

Combining a field repeated `n` times costs time and allocation linear in `n`. Every transport
combines a repeated line with `HttpHeaderValue.Concat`, one line at a time. Concat used to copy every
value into a new exact array on each call, which made `n` repeats quadratic. An HTTP/3 request of
one-octet QPACK references reached about 32,000 repeats per request (#1082). Concat now appends in
amortized constant time:

- **Exact arrays up to four values.** The common one-to-four-value field allocates exactly what it
  did before.
- **Geometric growth past four.** The array grows to twice the value count. A value past four keeps
  its array in a small private holder that records how many of the array's elements belong to it.
  Every accessor reads only those elements: the indexer, `Count`, `Value`, enumeration, `CopyTo`,
  `ToArray`, equality, and the hash.
- **The struct stays one reference wide.** `_values` is the only field: `null`, a string, a
  caller's array, or the holder. The struct is 8 bytes on 64-bit, so a header collection's
  dictionary entry stays 24 bytes. A count field beside the reference would have padded the struct
  to 16 bytes and every entry to 32, on every request, response and trailer collection, to serve
  fields repeated five or more times. One field also cannot be read torn.
- **Atomic slot claims.** The next append writes into the first spare slot, claimed with a
  compare-and-exchange from `null`, and returns a new holder. The first append from a given value
  takes the slot. Any other append from that value finds the slot taken and copies, on any thread,
  so two values appended to one original never see each other's. Appended values are never `null`,
  and only a holder's array has spare slots, so a caller's array is never written to.
- **Accepted costs.** Each append past four allocates one 32-byte holder (on 64-bit), so `n`
  repeats allocate about `32n` bytes of holders plus the arrays. A value can hold its array alive at
  up to twice the size its values need.

The alternative was to group repeats in every transport before building the collection. That fixes
each caller separately, and any caller that misses it stays quadratic. Fixing Concat covers
HTTP/1.1, HTTP/2, HTTP/3, the trailer readers, and `AppendValue` in one place. `Cookie` is the
exception. Its crumbs join into one string with `"; "`, and an immutable string cannot grow in
place, so `HttpFieldNormalization.CombineFieldValue` copies the growing value for every crumb. The
HTTP/2 and HTTP/3 transports collect a section's crumbs and join them once at the end of the section
(#1082).

## Field syntax

`HttpFieldNormalization` states the one field rule every reader and writer applies, received or
sent (#1341; decision 27 of the HTTP/Web program): `IsValidFieldName`, `IsValidFieldValue` and
`IndexOfInvalidControlCharacter`. The members take `ReadOnlySpan<char>`, allocate nothing, and
return a boolean or an index, so each caller raises its own error: a `400` on HTTP/1.1, a
malformed-request stream error on HTTP/2 and HTTP/3, an exception before the first byte on a writer.
They scan spans with `SearchValues<char>` and `IndexOfAny`, which vectorize.

- **A name** is a token, `1*tchar` (RFC 9110 §5.1, §5.6.2). That excludes every HTTP/1.1 delimiter
  (SP, HTAB, `:`, CR, LF), every control character, and every non-ASCII character. HTTP/2 and
  HTTP/3 also require lowercase and check that themselves; pseudo-headers are not field names.
- **A value** has no NUL, CR, or LF, and no SP or HTAB at either end (RFC 9110 §5.5, RFC 9113
  §8.2.1, RFC 9114 §4.1.2, §10.3). This is the minimum a recipient must enforce and a sender must
  never break: CR and LF end an HTTP/1.1 field line, and a hop that strips boundary whitespace
  changes the value.
- **Other control characters** are a separate, stricter check: `IndexOfInvalidControlCharacter`
  finds any CTL but HTAB (`%x00-08`, `%x0A-1F`, `%x7F`), NUL, CR, and LF included. RFC 9110 §5.5
  calls such a value invalid but lets a recipient keep it, so the check is a separate member.
  HTTP/1.1 applies it to every received value once SP and HTAB are trimmed. obs-text (`%x80-FF`, C1
  controls included) is a valid octet in both rules.
- **The rules judge characters, not octets.** A character above U+00FF is never produced by a
  Latin-1 decode and is left to the encoder that writes it.

**Readers.** HTTP/1.1's `Http1FieldLine` applies the rule to headers and chunked trailers, and
`Http1ChunkExtensions` to the tokens and quoted strings of chunk extensions. Since #1376 the HTTP/2
and HTTP/3 decoders apply the same rule to every received field line, heads and trailer sections
alike (`HttpReceivedFieldRules` in the transport): a lowercase token name, and a value that passes
`IsValidFieldValue` and holds no other control character either, so all three versions refuse the
same requests.

**Writers.** Since #1183 every response head writer applies `IsValidFieldName` and
`IndexOfInvalidControlCharacter` to each field line as it encodes it, on all three versions and in
the `Http.ProtocolUpgrade` 101 writer, and refuses the head before writing a byte with an
`HttpException` whose code is `HttpErrorCode.InvalidResponseField`. The one refusal that comes after
bytes went out is a streamed response's trailer section, which follows its head and body; the
transport resets that stream. The code is the same in both cases, so its documentation tells a
caller to check `IHttpExchangeControl.HasResponseStarted` before replacing the response, and each
public member that throws it (`IHttpExchangeControl`'s interim write and tunnel accept, and the
interim response, extended CONNECT, protocol upgrade, and response streaming features) documents
what the refusal leaves behind.

- **Every control character but HTAB is refused.** A sender may not generate a value outside
  `field-content`, so the writers refuse every control character but HTAB, not only NUL, CR, and
  LF.
- **Edge whitespace is not refused.** SP and HTAB at a value's ends split nothing and are not part
  of the value; the HTTP/2 and HTTP/3 encoders, whose versions make such a value malformed, send it
  without them.

**Why at every reader and writer.** A check in the header collection alone would not be enough: any
`IHttpHeaderCollection` implementation could bypass it, and a value built over an array shares that
array with its caller.

## Host values

`HttpHost` carries the request's effective authority exactly as the transport resolved it, and
splits it structurally into a host and a port (`TryGetComponents`, `Host`, `Port`). Only SP and HTAB
are trimmed from the value before the split (RFC 9110 §5.6.3, `HttpFieldSyntax.TrimOws`).
`string.Trim()` also stripped `U+0085` and `U+00A0`, which an HTTP/1.1 transport decoding octets as
Latin-1 can deliver, so `api.test\xA0` matched an `api.test` allowlist entry while a front end that
routes on the raw value saw another host (#1341). HTTP/1.1 now answers such a `Host` with `400`
before it reaches `HttpHost`; the narrower trim also covers the HTTP/2 and HTTP/3 `:authority`. The
routing constraint's matcher trims the same way.

## Query parameters

`HttpQuery.Parse` splits the raw query on `&`, splits each parameter on its first `=`, and
percent-decodes both halves (RFC 3986 §2.1) into an `HttpQueryCollection`. Every transport parses the
query through it, so a query reads the same on HTTP/1.1, HTTP/2 and HTTP/3.

A parameter with an empty name — `?=1`, a bare `=` — is **skipped** (#1323). `HttpQueryKey` is
never empty, and RFC 3986 §3.4 gives the query no syntax that would make such a parameter an error,
so it is left out of the collection and stays visible only in the raw `HttpQuery.Value`. Before
#1323 the parse threw `ArgumentException` while the transport read the request head, so any client
could fail its own connection or stream, and have the server log the failure as a defect, with one
`=`. A rewrite target is held to a stricter rule: `Web.Rewrite` still refuses a query entry without
a name, at registration for a target's literal text and with `400` when a capture produces one.

## Content types: two lookups

The extension-to-content-type table of `HttpContentTypes` is a `static FrozenDictionary<string,string>`
built once at startup with case-insensitive keys and no reflection. It covers common web asset types
rather than the full IANA registry; consumers that need custom mappings build their own overlay with
`CreateMap`, which clones the defaults and applies overrides — the default table is immutable and
shared.

**Two lookups, never a guess between them (#1186, decision 29).** The table is read through a
file-name lookup (`TryGetFromFileName`, `GetFromFileName`) and an extension lookup
(`TryGetFromExtension`, `GetFromExtension`). Each takes the default table or a caller-supplied one,
and the `Get` forms return `Fallback` (`application/octet-stream`) where the `Try` forms return
`false`.

- **The file-name lookup** reads the **final** extension of the name's final segment (after the
  last `/` or `\`), so `archive.tar.gz` is `.gz` and `assets.v2/site.css` is `.css`. Leading dots
  belong to the name. A name with no extension maps to nothing: one with no dot (`html`, `README`),
  a dotfile (`.json`, `.env`), one of dots only (`.`, `..`), and one ending in a dot
  (`index.html.`). A dotfile with an extension of its own resolves by it (`.config.json` is JSON).
  This is the dotfile rule of Python's `os.path.splitext`.
- **The extension lookup** requires the leading dot: an extension is a dot followed by at least one
  character, none of them a dot or a path separator, which is exactly what the file-name lookup can
  extract. `css`, `site.css`, and `.tar.gz` map to nothing.

The single `TryGetContentType(fileNameOrExtension)` it replaces accepted both forms and could not
tell a file named `json` from the bare extension token `json`, so it read every dotless name as an
extension. `Web.StaticFiles` passes file names, so an upload named `html` under the static root was
served as `text/html` — stored XSS from a file name — and passed the
`ServeUnknownContentTypes = false` gate meant to block it. The bare-token form is gone rather than
kept beside the split: a lookup that guesses is the defect. The source break is accepted during the
previews.

`CreateMap` still accepts a key with or without its leading dot. A key is always an extension, so
there is nothing to disambiguate: the key `gltf` maps `.gltf`, and a file named `gltf` still maps to
nothing. A key with an interior dot (`.tar.gz`) is stored but never matched, because neither lookup
produces one.

## Methods are case-sensitive (RFC 9110 §9.1)

`HttpMethod` keeps its token exactly as given and compares it byte for byte: `Equals`, `GetHashCode`
and the operators are ordinal, and `GetCanonicalizedValue` returns a standard method's shared
instance only for its exact upper-case token. `get` is an unknown extension method: it is not `GET`,
it is not safe, idempotent or cacheable, and nothing that tests for `GET` matches it (#1301,
decision 26). The classification properties (`IsSafe`, `IsIdempotent`, `IsCacheable`,
`CacheKeyIncludesContent`) are plain ordinal switches over the token, so only the exact upper-case
standard tokens match, and `new HttpMethod("query")` no longer equals `HttpMethod.Query`.

**Why.** RFC 9110 §9.1 defines methods as case-sensitive, and conformant intermediaries treat `get`
as a method they do not know. Before #1301 the constructor upper-cased every token and equality
ignored case, so this server applied the standard semantics a proxy, WAF or cache in front of it did
not:

- a method-based access rule at the intermediary was bypassed by `post` or `delete`;
- `head` had its response body suppressed while the intermediary, seeing an unknown method, waited
  for one, which misframes the next response on a shared upstream connection;
- `connect` with `:scheme` and `:path` passed the HTTP/2 and HTTP/3 pseudo-header checks as an
  ordinary request (they already compared `CONNECT` ordinally), then became `CONNECT` and skipped
  the request-body hooks;
- `connect host:port` on HTTP/1.1 became a `CONNECT` request and opened a tunnel. It is now rejected
  as a malformed request-target (authority-form on a method that is not `CONNECT`), and `options *`
  is rejected the same way, because asterisk-form belongs to `OPTIONS` alone.

Every transport parses the method through `GetCanonicalizedValue`, so the rule holds on HTTP/1.1,
HTTP/2 and HTTP/3 alike, and so does every consumer that compares against the standard instances
(routing, CORS, antiforgery, output caching, telemetry). Telemetry now reports `get` as `_OTHER`
with `http.request.method_original` = `get`, as the semantic convention intends.

**Breaking change** (accepted during the previews on decision 15's terms). A method built from a
token that is not upper case, including through the implicit conversion from `string`, no longer
equals the standard method it spells, and `Value` keeps the case it was given. Code that wrote
`request.Method == "get"` or mapped a route with `new HttpMethod("get")` now names a different
method.

**Alternatives considered.**

- **Reject a non-standard-case method with `400`/`501`.** RFC 9110 lets a server answer an unknown
  method with `501`, and an application that wants that can do it in its pipeline; the transport's
  job is to report what was sent, not to guess which extension methods the application serves.
- **Keep folding and compare ordinally only at the edges.** Every consumer would need to know which
  comparison is safe; one forgotten case-insensitive check reopens the gap. Fixing the value type
  closes it everywhere at once.

## The extended CONNECT seam

An HTTP/2 or HTTP/3 *extended CONNECT* (RFC 8441, RFC 9220) is a `CONNECT` request that carries
`:protocol`, asking to run another protocol, most often WebSocket, over its one stream. The core
carries two generic seam members for it and no feature contract:

- **`HttpExchangeInterceptorRequestContext.Protocol`** is the `:protocol` the transport validated
  (RFC 8441 §4, RFC 9220 §3), or `null` on any other request. It is the only signal that tells a
  request-parse hook an extended CONNECT from a classic one; the method alone cannot.
- **`IHttpExchangeControl.CanAcceptTunnel` and `AcceptTunnelAsync`** are the per-stream counterpart
  of `TakeOver`. HTTP/1.1's control reports `false`, because its `CONNECT` and upgrades take the
  whole connection over instead.

`AcceptTunnelAsync` is one-shot: it answers `200` in a head that does not end the stream, takes the
exchange over, and returns the stream as a duplex `Stream`. The member's documentation carries the
full contract:

- **The head** carries the headers already set on the response, without `Content-Length`,
  `Transfer-Encoding` (RFC 9110 §9.3.6) or the connection-specific fields (RFC 9113 §8.2.2, RFC 9114
  §4.2). Any status already set is replaced by `200`, and a body written to the response is
  discarded.
- **Reads** return the client's `DATA` and return 0 once the client ends its side (HTTP/2
  `END_STREAM`, HTTP/3 FIN).
- **Writes** go out as `DATA` at once, unbuffered and paced by the peer's flow control.
- **Disposing** ends the server's side; the client may still send until it ends its own.
- **A peer reset or a lost connection** faults pending and later reads and writes with an
  `IOException`.
- **The first attempt latches.** On an extended CONNECT, the first call uses the accept up whether or
  not it succeeds, so `CanAcceptTunnel` is `false` once `AcceptTunnelAsync` has been called. A
  second call, a call after the response started, and a call on a cancelled exchange throw
  `InvalidOperationException`; a stream the peer already reset, or a closed connection, is an
  `IOException`. Cancelling the token before the head is written throws
  `OperationCanceledException`, uses the accept up, and the exchange is reset when it ends. A call on
  an exchange that is not an extended CONNECT is refused without latching.
- **A refused response field** — a name that is not a token, or a value holding a control character
  other than HTAB (RFC 9110 §5.1, §5.5) — throws an `HttpException` whose code is
  `HttpErrorCode.InvalidResponseField`. The head is checked before the stream is claimed, so nothing
  was written, the response has not started, and the status set before the call is restored: the
  exchange can still be answered with an ordinary response. The accept is spent and cannot be
  retried.

Accepting takes the exchange over, as `TakeOver` does for an HTTP/1.1 connection. The transport
registers the tunnel before it writes the head, so neither its send path nor the raw response sink
can put a second head on the stream, even if writing the `200` fails. The guards run before anything
is written, in a fixed order: accepted once, cancelled, response started, stream reset or connection
closed. The exchange interceptors' response-head and after-response hooks do not run for a tunnel.
The tunnel lasts as long as the exchange: when the handler returns, the transport ends a tunnel the
application left open, and a cancelled exchange resets the stream.

The probe reports rather than throws, like `CanTakeOver` and `CanWriteInterimResponse`:
`CanAcceptTunnel` is `false` on HTTP/1.1, on any exchange that is not an extended CONNECT, once the
tunnel was accepted or the response started, and on a cancelled exchange. It does not report a
stream the peer has already reset; the accept learns that as an `IOException`.

The application-facing feature, `IHttpExtendedConnectFeature` (`context.ExtendedConnect`), ships in
`Assimalign.Cohesion.Http.ExtendedConnect`. Its interceptor installs the feature from `Protocol` and
binds it to the exchange control, the way `Http.ProtocolUpgrade` wraps `TakeOver`. The transport's
design carries the wire behavior, and the Http area's ADR 1 records why the tunnel exists: server
WebSockets on HTTP/2 and HTTP/3.

**Why seam members, not a feature contract.** Only the transport can perform the accept: it writes a
HEADERS block without `END_STREAM` through the connection's shared HPACK or QPACK state and frames
`DATA` under the stream's flow-control windows. #1316 therefore put the feature contract in the core,
so that the transport could install its own implementation without referencing a feature package.
That contradicted the rule that seams live in the core and features in packages (see
[Why the seam is core and the features are not](#why-the-seam-is-core-and-the-features-are-not)),
and owner decision 20 (2026-10-09) reversed it in #1368: the accept became a mechanism on
`IHttpExchangeControl`, the generic control that exists so that a new wire mechanism needs no
per-capability contract, and the feature returned to `Http.ExtendedConnect`, its preview.1 home. The
TLS session feature that sat beside it moved to `Assimalign.Cohesion.Http.Tls` in #1367, so the core
now holds no transport-produced feature contract. Two consequences follow:

- **The feature depends on a registration.** It exists only on a listener that registers
  `HttpExtendedConnect.CreateInterceptor()`; the Web host does by default. The HTTP/2 and HTTP/3
  transports advertise extended CONNECT regardless, so a listener without the interceptor surfaces a
  client's extended CONNECT as an ordinary `CONNECT`.
- **An extended CONNECT pays for the response phase.** The interceptor reaches the control through
  `AddResponseInterceptor` (see
  [per-exchange response interceptors](#per-exchange-response-interceptors)), so the transport
  builds a response sink and an exchange control for each extended CONNECT, once per WebSocket
  handshake. Every other exchange keeps the fast path.

**A source break for outside implementers.** `IHttpExchangeControl` shipped in preview.1, and the two
members were added as plain interface members, not default implementations: a throwing default on a
public seam would hide an unsupported mechanism behind a runtime failure. The only shipped
implementers are the transport's three controls. For an implementer outside this repository the
addition is a source break: a class written against preview.1's interface no longer compiles until
it implements `CanAcceptTunnel` and `AcceptTunnelAsync`. The
[Http.ExtendedConnect design](../assimalign-cohesion-http-extendedconnect/design.md) ("Behavior
change from 10.0.0-preview.1") records this, and that the feature now needs its interceptor
registered.

**What it does not do.** It carries octets, not a protocol: WebSocket framing comes from the BCL over
the accepted stream, and the handshake from `Http.WebSockets`. A classic `CONNECT` (no `:protocol`)
has no `Protocol`, and nothing here dials the request's authority. `AcceptTunnelAsync` is the stream
tunnel a classic `CONNECT` over HTTP/2 or HTTP/3 (RFC 9113 §8.5) would need, so adding that later is
a package change, not a core one.

## The client-fault report

`IHttpExchangeControl.ClientFaultStatusCode` reports the `4xx` status the transport answers an
exchange with because the client's request was at fault, or `null`. A transport that dispatches a
request at its head learns some faults only while the application reads the body: the body breaks the
message framing (a malformed chunk size, a malformed trailer section) or a configured limit (its size,
its data rate, the bounds on a trailer section), or the client closes the connection before the body
its framing declared is complete (RFC 9112 §8). The read throws, as any stream read does, and the
transport answers the exchange itself, replacing a response that has not started and closing the
connection (the Http.Connections design,
[HTTP/1.1: reporting the client fault](../assimalign-cohesion-http-connections/design.md#http11-reporting-the-client-fault-1340)).
The member tells the code that observes the exception that it was the client's fault, without
inspecting the exception.

| Body read fails with | Latched status |
|---|---|
| `InvalidDataException`: the body breaks the message framing | `400` |
| `IOException`: the body breaks a limit | `408`, `413` or `431` |
| `EndOfStreamException`: the body is cut short | `400` |

Once it reports a status, it reports the same status for the rest of the exchange. A response that
had already started is finished or reset as the host decides; the status then does not reach the
wire, and the member still reports it.

**Why a probe on the control, not a typed exception or a feature contract.** Owner decision 28
(2026-10-09) chose the seam for #1340, under decision 20's rule that the core holds base contracts
and generic seams only:

- **The body stays a plain `Stream`.** It keeps throwing `InvalidDataException` and `IOException`,
  so every reader that already catches them keeps working. A core exception type carrying the status
  would be a concern-specific contract in the core, and every wrapper stream (decompression, capture,
  a form parser) would have to preserve it.
- **The control is the transport's per-exchange surface.** It already reports exchange state through
  report-don't-throw probes (`HasResponseStarted`, `CanTakeOver`, `CanAcceptTunnel`); a client fault
  is one more fact of that kind. A feature package wraps it into a typed feature from the control its
  response interceptor captures, as `Http.ProtocolUpgrade` wraps `TakeOver`. The Web host does
  (`IWebClientFaultFeature`, [`Web.Server`](../../../resources/web/assimalign-cohesion-web-server/index.md)).

**Why a default member.** Unlike `CanAcceptTunnel` and `AcceptTunnelAsync`, the member has a default
implementation that returns `null`, the precedent of the [trailer collections](#trailers). `null`
means "no client fault reported", which is exactly what an implementation that predates the member,
or a protocol version whose transport does not report faults yet, can honestly say. Nothing is hidden
behind a runtime failure, so adding it is not a source break. The server transport overrides it for
HTTP/1.1; HTTP/2 and HTTP/3 keep the default until #1378.

## Per-exchange response interceptors

An exchange interceptor declares the phases it takes part in (`HttpInterceptorScopes`), and the
transport runs it only there. A response-scoped interceptor costs every exchange its response body
sink and exchange control, which the transport builds before the handler runs, so a request-only
interceptor never makes an exchange pay for them.

A request hook can claim one exchange's response phase. An interceptor that needs the response phase
for a few exchanges only declares `Request` and, from a request-parse hook, adds itself (or any
interceptor) to that exchange alone: `HttpExchangeInterceptorRequestContext.AddResponseInterceptor`.
The transport then runs the response phase for that exchange with the listener's response
interceptors first and the added ones after, each at most once, and every other exchange keeps the
fast path. Only a call from a request-parse hook takes effect: the transport reads the added
interceptors once, when it sets up the exchange. `Http.ProtocolUpgrade`, which `Web.Hosting`
installs by default and which needs the exchange control only for an HTTP/1.1 upgrade or `CONNECT`,
is the case it exists for.

## Feature lookup by contract

`IHttpFeatureCollection` is keyed by name. `Features.Get<TFeature>()`
(`HttpFeatureCollectionExtensions`) looks up a contract instead: it returns the first installed
feature that implements `TFeature`, in enumeration order. That order is the local features, then each
defaults level's features that no level above it hides by name.

- **No allocation on `HttpFeatureCollection`.** Every transport context carries that exact type, and
  per-request readers call the lookup on every request: the `Http.Forwarded` `Effective*` members,
  which host filtering, routing's `RequireHost` and rate limiting read, among others. Enumerating
  through `OfType<TFeature>().FirstOrDefault()` allocated the LINQ iterator and the collection's
  `yield` enumerator on each call. The lookup now scans the backing dictionaries with their struct
  enumerators and walks the defaults chain itself (raised by the #1077 review).
- **The same answer as enumeration.** A defaults-level feature is skipped when a level above
  installs its name, whatever that level's feature implements, as the enumerator skips it. A
  defaults source of any other type, a derived `HttpFeatureCollection` included, is enumerated
  through its own enumerator, because a derived type can re-implement `IEnumerable<IHttpFeature>`.
  That path allocates as before.
- **Still `O(n)`.** The scan covers typically fewer than ten features. A caller that needs `O(1)`
  caches the resolved feature.

`HttpFeatureCollectionTests` pins the zero-allocation lookup and checks the shadowing cases against
enumeration.

## Why the seam is core and the features are not

Seams live in the core, and features live in packages. The interceptor seam is a compile-time
contract because the transport must enforce mutation mid-parse, attach features before dispatch, and
replace streams, none of which a loosely typed `Items` key can express. A capability that only needs
one-way publication after parsing can still use an `Items` key.

A transport never puts a feature contract in this core. It publishes what it knows about a
connection as a *facet* on the exchange's connection info: an extra interface the
`HttpConnectionInfo` it hands out also implements, found with a type test (the TLS handshake is the
Connections library's `ITlsConnectionInfo`, which `Assimalign.Cohesion.Http.Tls` turns into
`context.TlsConnection`). A wire mechanism only the transport can perform is offered through
`IHttpExchangeControl`, which a feature package wraps (`context.Upgrade` over `TakeOver`,
`context.ExtendedConnect` over `AcceptTunnelAsync`). A request fact a hook needs that the parsed head
cannot show is a member of the request context (`Protocol`, the validated `:protocol` of an extended
CONNECT). Either way the feature package owns the application-facing contract, and the transport
references no feature package. The rule has no exceptions left: the last two transport-produced
contracts moved out in #1367 (TLS) and #1368 (extended CONNECT).

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.

- **Source** — `cohesion/docs/libraries/Http/DECISIONS.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExchangeControl.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpResponse.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpExchangeInterceptorRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQuery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpHeaderValue.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpHost.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpMethod.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpFieldNormalization.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentTypes.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpFeatureCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpFeatureCollectionExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Exceptions/HttpErrorCode.cs`.
