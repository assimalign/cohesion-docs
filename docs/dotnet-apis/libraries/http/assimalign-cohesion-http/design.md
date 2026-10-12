# Assimalign.Cohesion.Http design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

## Small header collections and cached fields (#1417)

A parameterless `HttpHeaderCollection` holds its first field in the collection
itself. Subsequent distinct fields use a lazy dictionary, while the first field
remains inline. Seeding a response with a process-cached `Date` therefore allocates
nothing and consumes none of the dictionary capacity needed by application
fields. The core knows no Date policy or clock; this is generic collection
storage, and the transport owns the default field and its lifetime.

The capacity constructor retains a preallocated dictionary. A constructor passed
a dictionary retains that exact live store, including a caller's comparer and
later external mutations. `AsReadOnly` points at the original collection, so a
view created while it is empty follows later inline insertion, dictionary growth,
removal and reuse. Mutations through the view still fail. `Add` retains an empty
value as a present field; assigning an empty value through the indexer removes
it. Enumeration visits the inline field and then the dictionary. Field-name order
after removals and reinsertions is unspecified, as it was for dictionary storage;
repeated values within a field retain their order.

This changes no public contract from 10.0.0-preview.1. It removes the eager empty
dictionary allocation and the first field's bucket/entry allocations. The
collection itself carries the inline key/value and the live-view reference;
larger collections still allocate their ordinary dictionary. The alternative,
putting the first field into that dictionary when the second arrives, would make
a default Date trigger an extra resize at ordinary field capacity boundaries.

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
- **The rules judge characters, not octets.** Every reader decodes a field octet as Latin-1
  (HTTP/1.1 since #1341, QPACK, and HPACK since #1387), so obs-text reaches the rules as the
  character of the same value, and a character above U+00FF is never produced by a reader; it is
  left to the encoder that writes it.

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

## Request-target forms (#1334)

`HttpFieldNormalization.ValidateExtendedConnect` and `ValidateRequestTarget` hold the pseudo-header
shapes HTTP/2 and HTTP/3 share. An extended CONNECT carries a non-empty `:protocol` with `:scheme`,
`:path`, and `:authority` (RFC 8441 §4, RFC 9220 §3). A classic CONNECT carries the host and port in
`:authority` and neither `:scheme` nor `:path` (RFC 9113 §8.5, RFC 9114 §4.4), and a `:path` that
starts with `*` is exactly `*` on an OPTIONS request (RFC 9110 §7.1, #1334).

- **A request target holds only VCHAR.** `:path`, `:authority`, and every `Host` value hold only
  VCHAR, as HTTP/1.1 holds its request line and `Host` (#1341, #1387): HPACK and QPACK decode
  obs-text as Latin-1, which an ordinary value may carry but no request target or host does.
- **`:method` is what `HttpMethod` can represent.** `ValidateRequestTarget` also holds `:method` to
  a token (RFC 9110 §9.1) of at most 32 characters (`HttpMethod.MaximumLength`, internal), so a
  request that passes can always be built with `HttpMethod.GetCanonicalizedValue`, which throws for
  anything else.
- **A violation is a message, not an exception.** Both return a violation message rather than throw,
  and the message never quotes the peer's text, so HTTP/2 resets the stream with `PROTOCOL_ERROR`
  and HTTP/3 with `H3_MESSAGE_ERROR`.
- **One authority-form parse.** `ValidateRequestTarget` reuses `HttpRequestTarget.TryParse` for the
  authority-form, so HTTP/1.1, which parses its request line through the same type, accepts the same
  CONNECT and asterisk-form targets.

`ValidateRequestTarget` is a new public member. The transport's design lists the requests HTTP/2 and
HTTP/3 now reset
([HTTP/2 request heads](../assimalign-cohesion-http-connections/design.md#http2-request-heads-rfc-9113-83)).

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

### Repeated values (#1335, decision 31)

A key maps to an ordered `HttpQueryValue` list. `HttpQuery.Parse` appends
instead of replacing, including empty and interleaved occurrences. `Count`
on the collection still counts distinct keys; `Count` on a value counts its
occurrences. Existing case-insensitive key matching is unchanged. The values
follow the ordered name/value model in the [WHATWG URL parser](https://url.spec.whatwg.org/#urlencoded-parsing);
this change does not alter the existing plus-sign or empty-name parsing rules.

`Value`, `ToString`, conversion to string, and typed scalar getters read the
**first** occurrence. Enumerating or indexing the value reads every occurrence,
without comma joining. `HttpQueryValue.Empty` has zero occurrences and a scalar
empty string; `new HttpQueryValue("")` has one empty occurrence. Equality and
hashing compare the whole ordered list. `HttpQueryCollection.Add` appends;
its setter replaces the whole list, and removal removes the key and all values.
The existing dictionary constructor still wraps its store.

The value privately reuses `HttpHeaderValue`'s immutable geometric append
storage, avoiding quadratic copies for attacker-controlled repeats without
adopting header comma-folding. No collection interface gains a member, no
reflection is introduced, and the value remains one reference wide. The Web
endpoint generator emits loops over this list for supported query sequences;
scalar bindings still use `Value`.

**Breaking changes from 10.0.0-preview.1**, accepted during previews on decision
15's terms: parsed duplicates select the first scalar instead of the last;
`Add` appends duplicates instead of throwing; value equality observes all
occurrences; a missing value is distinct from one explicitly empty occurrence.
Serialization now includes every value. Supported scalar sequence parameters
in generated endpoints infer query binding; an explicit `[FromBody]` retains
body binding (see [Web.Api's design](../../../resources/web/assimalign-cohesion-web-api/design.md)).

**In-repository caller audit.** No caller or existing test explicitly requires
last-wins. The following scalar readers now observe the first occurrence:

- `ConfigurationEndpointService` (ConfigurationStore.Hosting): `name`.
- `SecretsEndpointService` (SecretStore.Hosting): `path`, `name`,
  `application`, and `resource` through its query helper.
- `LogSpaceHttp`: `limit`, `since`, `resource`, and `cursor`.
- `EndpointBindingGenerator`: explicit scalar query binding and scalar
  route-to-query fallback; Http.Connections' HTTP/1, HTTP/2 and HTTP/3 examples:
  `name`.
- `HttpLoggingMiddleware.SerializeQuery` and
  `WebServerTelemetry.GetQueryAttribute`: their existing scalar projection
  records the first occurrence once per key. Their disclosure/redaction scope
  is unchanged.

Non-scalar consumers were checked too. `OutputCacheKeyBuilder` now includes
all occurrences in order for both all-key and selected-key variation, with
escaped components so decoded separators cannot collide. `HttpQuery.Create`
now writes every occurrence, covering HTTPS and directory redirects and
`RewriteContext` serialization. `RewriteUrl` parses replacements through the
same parser. Request wrappers retain the complete collection by reference.

## Writing a path and a query back out (`ToUriComponent`, `HttpQuery.Create`)

Every transport hands middleware a **decoded** request: `HttpPath.FromUriComponent` decodes the
path, and `HttpQuery.Parse` decodes the query into a collection. A component that writes the request
back into a URL, such as a redirect's `Location`, needs the inverse of each, and before #1324 the
Web redirects wrote the decoded path as it was: `/caf%C3%A9` became `/café`, which the HTTP/1.1
writer sends as `/caf?`, and `/100%25` became `/100%`, which is not a URI reference (RFC 9110
§10.2.2). The inverses live here, beside the decoder whose rules they have to mirror:

- **`HttpPath.ToUriComponent()`** encodes each segment (RFC 3986 §3.3): `/` and every `pchar`
  (unreserved, sub-delims, `:` and `@`) are written as they are, and everything else is
  percent-encoded as UTF-8, a lone surrogate as U+FFFD. A `%` is written as `%25`, **except** where
  it begins an escape `UrlDecoder` leaves as written, which a decoded path can only carry because
  the request did:
  - `%2F` or `%2f`. `UrlDecoder` never decodes an encoded slash, so it can never become a segment
    separator (the routing `{**}` invariant depends on that).
  - A sequence that is not valid UTF-8: an invalid or lone byte (the Latin-1 `/caf%E9`, a stray
    continuation `%A9`), a truncated sequence (`/%C3` at the end), an overlong one (`%C0%AE`) or an
    encoded surrogate (`%ED%A0%80`).
- **`HttpQuery.Create(IHttpQueryCollection)`** joins the entries with `&` in key enumeration order
  and each key's values in arrival order, percent-encodes each name and value except for the
  unreserved characters, so an `&`, `=` or `+`
  inside one cannot split or change it, and writes an empty value as the bare name. `Parse` reads it
  back as the same collection. It is not the query the request carried: only the parsed collection
  survives the transport, so order and percent-encoding are rebuilt.

**Why the kept escapes.** Encoding the `%` of either kind again would write `%252F` or `%25E9`: it
decodes to the same `HttpPath`, but it is a different URI, which a cache or another server in front
reads as another resource. `UrlDecoder.DecodesEscapeAt` runs the decoder's own rule at the `%`, so
the two cannot drift; any other `%` must have arrived as `%25`, because the decoder would have
decoded it, and is escaped. The rule is applied to the decoded text and holds for the encoded text:
an escape that a lead byte needs as its continuation is one the encoder keeps, since a character it
encodes never starts with a continuation byte. So `FromUriComponent(path.ToUriComponent())` equals
`path` for every path. A request-target in canonical form (upper-case hex, nothing escaped that need
not be) comes back unchanged, except where it spells as `%25` a `%` that begins an escape the
decoder leaves: `/a%2F` and `/a%252F` both decode to `/a%2F`, `/caf%E9` and `/caf%25E9` both decode
to `/caf%E9`, and the decoded form cannot tell them apart. Each is written back in the form a client
sends, `/a%2F` and `/caf%E9`, which decodes to the same path. (#1324's review, 2026-10-10: the first
version kept only `%2F`, so `/caf%E9` came back as `/caf%25E9`.)

A space, `?` or `#` never needs encoding in a path that came from a request: `HttpPath` rejects
them, and the transports fail a request-target whose path decodes to one
(`HttpRequestTargetDecodeParityTests`, `Http2MalformedPathTests`, `Http3MalformedPathTests` in
Http.Connections). Both helpers are value-object members, not feature contracts, so decision 20's
rule for the core is unchanged.
[`Web.HttpsPolicy`](../../../resources/web/assimalign-cohesion-web-httpspolicy/index.md),
[`Web.StaticFiles`](../../../resources/web/assimalign-cohesion-web-staticfiles/index.md) and
[`Web.Rewrite`](../../../resources/web/assimalign-cohesion-web-rewrite/index.md) build their
redirects with them.

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

## `If-Range` is step 5 (RFC 9110 §13.1.5)

`HttpConditionalRequest.Evaluate` resolves the RFC 9110 §13.2.2 precondition steps 1–4 and returns
`Proceed`, `NotModified` or `PreconditionFailed`. It deliberately stops there: `If-Range` is
range-specific. `HttpIfRange.Matches` is step 5, and the two compose into the full decision a range
feature needs, with `HttpRangeSelector` choosing `206` or `416`:

```text
outcome = HttpConditionalRequest.Evaluate(context)      // steps 1–4 → 304 / 412 / proceed
if outcome != Proceed              → send 304 / 412
else if a Range header is present:
    applyRange = ifRange is null  ||  ifRange.Matches(currentETag, currentLastModified, responseDate)   // step 5
    if applyRange   → HttpRangeSelector.Select(...)     → 206 / 416 / (Full → 200)
    else            → 200 full          // validator stale: give the client the whole thing
else                               → 200 full
```

Three correctness details:

- **`If-Range` is always strong.** The entity-tag form uses `HttpEntityTag.StrongEquals` (a weak or
  absent current tag never applies the range). A mismatch *ignores* the range (full `200`) —
  `If-Range` is "give me the whole thing if my copy is stale", never a `412`.
- **A date applies the range only on an exact match with a strong `Last-Modified` (#1185).** RFC
  9110 §13.1.5 makes the date form true only when the date "exactly matches" `Last-Modified`, and
  only when that date is a strong validator (§8.8.2.2). §8.8.2.2 lets an origin server treat
  `Last-Modified` as strong only when it reliably knows the representation did not change twice
  during that second, which a timestamp alone cannot show. `Matches` therefore uses a conservative
  heuristic modeled on §8.8.2.2's client rule, which reads the `Date` of the response that delivered
  the cached copy: `Last-Modified` counts as strong only when it is at least one second before
  `responseDate`. So a representation modified in the current second, or dated after the response (a
  clock running ahead), never applies a date.
- **One-second granularity.** The date comparison and the strength test both run on whole Unix
  seconds, because an HTTP-date carries only whole seconds. A sub-second write therefore does not
  read as "modified", and a date built in code with a sub-second part names the second it falls in.

**What the heuristic cannot detect.** The representation changes twice inside the validator's
second, and the client's copy was taken between the two changes. Both versions carry the same
`Last-Modified`, so the client's date matches and it gets a `206` of bytes that do not splice with
its copy. The strong `ETag` form has no such case and is the reliable path for resuming a transfer.
The caller passes `responseDate`, normally the current time, because the core reads no ambient clock
(the `HttpFreshness` rule).

**A source break.** Through 10.0.0-preview.1 the date form applied the range whenever
`Last-Modified ≤ If-Range date`, the rule ASP.NET Core's StaticFiles and `RangeHelper` use. A client
holding a *later* date (a file replaced by an older copy, or a client clock ahead of the server's)
then got a `206` of a representation its partial copy does not splice with. The two-argument
`Matches` was replaced rather than kept beside the new one, since it cannot decide strength without
a date. That is a source break for a caller outside this repository, accepted during the previews on
the terms of the HTTP/Web program's decision 15.
[`Web.StaticFiles`](../../../resources/web/assimalign-cohesion-web-staticfiles/index.md) and output
caching consume this layer.

## Status codes and reason phrases (RFC 9110 §15)

`HttpStatusCode` holds one table: the IANA HTTP Status Code Registry's assigned
codes and the phrase the registry gives each (#1204). That is RFC 9110 §15's
phrase for every code RFC 9110 defines, and the defining RFC's for the rest,
`226 IM Used` (RFC 3229) included. The registry lists `510` as "Not Extended
(OBSOLETED)" since RFC 2774 moved to Historic; that annotation is not part of
the phrase, so the table sends `510 Not Extended`.

- **The phrase table is not validation.** `ReasonPhrase` returns a registered
  phrase or an empty string for a code absent from the table. `ToString()`
  renders `200 OK`, or `299 ` with the separating space and no phrase, for the
  HTTP/1.1 status line (the transport's head writer and the Http.ProtocolUpgrade
  `101`/`200` writer). A default-constructed instance still renders an empty
  string. [Web.ProblemDetails](../../../resources/web/assimalign-cohesion-web-problemdetails/index.md)
  and [Web.OpenApi](../../../resources/web/assimalign-cohesion-web-openapi/index.md) read the phrase
  too. Before #1204 Web.OpenApi kept a private RFC 9110 copy,
  because the core still had pre-RFC 9110 wording ("Ok", "Redirect Keep Verb",
  "Request Entity Too Large", "Request Uri Too Long", "Requested Range Not
  Satisfiable", "Un-Processable Entity", "Http Version Not Supported"). That
  wording reached HTTP/1.1 status lines and problem titles in 10.0.0-preview.1.
- **Every code from 100 through 599 is valid (#1414).** `IsValid`, the constructor
  and implicit conversion from `int` accept the inclusive numeric range in
  [RFC 9110 §15](https://www.rfc-editor.org/rfc/rfc9110.html#section-15). Anything
  below 100 or above 599 still fails. The original number is preserved; the
  recipient applies the `x00` class semantics when it does not recognize it.
  An empty phrase avoids inventing a registered meaning for a private or newly
  registered code. No new public member or reason-phrase table entry is needed.
- **Reserved and temporary codes.** The phrase table still omits `418` and
  temporary registrations such as `104`; both are now numerically valid and
  have empty phrases. The reserved `306` retains `(Unused)`, and the public
  `Unused` accessor remains `[Obsolete]`, preserving #1204's compatibility
  choice. Range validation neither asserts registry assignment nor recommends
  sending a reserved code.
- **All three transports.** HTTP/1.1 writes `HTTP/1.1 299 \r\n` with the required
  space and an empty phrase, as allowed by
  [RFC 9112 §4](https://www.rfc-editor.org/rfc/rfc9112.html#section-4). HTTP/2 and
  HTTP/3 write only `:status: 299`; no reason phrase is carried. The existing
  numeric interim/final rules still apply: `104` and `199` can be sent through
  the interim seam, while a final response cannot use 1xx. `101` continues to
  require the protocol-upgrade path.
- **`IsValid` caller audit.** The only production caller outside the constructor
  is `ProblemDetailsDefaults.GetTitle`. It now uses `ReasonPhrase` only when
  nonempty, preserving the prior generic title for an unregistered code:
  `Error` below 500 and `Internal Server Error` for 5xx. An explicit title is
  preserved. This title policy is independent of the empty wire phrase.

**Breaking behavior from 10.0.0-preview.1** (decision 15): a caller that relied
on `IsValid` returning false, or construction throwing, for an unregistered
100–599 value now gets a valid status. The public members and registered
phrases are unchanged. Core tests cover every integer in the range, invalid
boundaries and phrase rendering; transport tests exercise buffered and streamed
responses on all three protocols, and the unregistered interim encoders.

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
its data rate, the bounds on a trailer section). The read throws, as any stream read does, and the
transport answers the exchange itself, replacing a response that has not started and closing the
connection (the Http.Connections design,
[HTTP/1.1: reporting the client fault](../assimalign-cohesion-http-connections/design.md#http11-reporting-the-client-fault-1340)).
The member tells the code that observes the exception that it was the client's fault, without
inspecting the exception.

| Body read fails with | Latched status |
|---|---|
| `InvalidDataException`: the body breaks the message framing | `400` |
| `IOException`: the body breaks a limit | `408`, `413` or `431` |
| `OperationCanceledException`: the connection is lost before the body completes | `null` |

Once it reports a status, it reports the same status for the rest of the exchange. A response that
had already started is finished or reset as the host decides; the status then does not reach the
wire, and the member still reports it.

HTTP/2 and HTTP/3 report it too (#1378). Those versions frame the request themselves, so a body that
breaks the framing — content that contradicts its `content-length`, or a malformed trailer section —
makes the request malformed, and the transport resets the stream instead of answering it (RFC 9113
§8.1.1, RFC 9114 §4.1.2). No status reaches the wire, and the member reports `400`, the status
HTTP/1.1 answers a malformed body with, so a host classifies the exchange the same on every version.
A body over a limit (`413`, `408`, and on HTTP/3 `431` for its trailer section) is answered with its
status, as on HTTP/1.1. The reset also fires the exchange's cancellation, so a read on these
versions reports the fault's `IOException` rather than the cancellation (the Http.Connections
design,
[client faults on HTTP/2 and HTTP/3](../assimalign-cohesion-http-connections/design.md#client-faults-on-http2-and-http3-1378)).

A connection lost before the dispatched body completes instead cancels the exchange on every
protocol (#1430, #1439; owner decision 45). `RequestCancelled` fires, the read throws
`OperationCanceledException` carrying that exchange token, and `ClientFaultStatusCode` remains
`null`. This includes HTTP/1.1 EOF before its declared body ends, replacing #1340's incomplete-body
`400`. The read's exception and token change from 10.0.0-preview.1 under decision 15; malformed
framing bytes and configured limits retain their client-fault status.

**Why a probe on the control, not a typed exception or a feature contract.** Owner decision 28
(2026-10-09) chose the seam for #1340, under decision 20's rule that the core holds base contracts
and generic seams only:

- **The body stays a plain `Stream`.** Client-fault statuses still accompany `InvalidDataException`
  and `IOException`. A core exception type carrying the status would be a concern-specific contract
  in the core, and every wrapper stream (decompression, capture, a form parser) would have to preserve it.
- **The control is the transport's per-exchange surface.** It already reports exchange state through
  report-don't-throw probes (`HasResponseStarted`, `CanTakeOver`, `CanAcceptTunnel`); a client fault
  is one more fact of that kind. A feature package wraps it into a typed feature from the control its
  response interceptor captures, as `Http.ProtocolUpgrade` wraps `TakeOver`. The Web host does
  (`IWebClientFaultFeature`, [`Web.Server`](../../../resources/web/assimalign-cohesion-web-server/index.md)).

**Why a default member.** Unlike `CanAcceptTunnel` and `AcceptTunnelAsync`, the member has a default
implementation that returns `null`, the precedent of the [trailer collections](#trailers). `null`
means "no client fault reported", which is exactly what an implementation that predates the member,
or a protocol version whose transport does not report faults yet, can honestly say. Nothing is hidden
behind a runtime failure, so adding it is not a source break. The server transport overrides it on
every version: HTTP/1.1 since #1340, HTTP/2 and HTTP/3 since #1378.

### A response abandoned by its client (#1432)

`IHttpExchangeControl.ResponseAbortedByClient` reports the response-side outcome separately from
`ClientFaultStatusCode`: the transport ended the response because its peer stopped accepting it,
whether by a connection or stream reset, HTTP/3 `STOP_SENDING`, or the minimum response data rate.
It starts `false`, latches `true` for the rest of the exchange, and implies no HTTP status. The
transport publishes the reason before exposing the response failure; a locally initiated slow-read
abort publishes it before firing cancellation too. The exchange's `RequestCancelled` fires, and
response failures normalized by the transport carry that token. Driver stream-closure notification
can precede the write's observation of a peer reset; the latch is readable when the write fails.

This is another generic fact on the exchange control, following decision 20 and the request-side
report. Its default implementation returns `false`, so an existing implementation need not add a
member. #1432 explicitly sanctions this addition to the public interface; it is recorded for the
owner's stage review. An application exception, application cancellation, local reset, or a rejected
request body alone does not set the response report. A consumer must read the latch, rather than infer
it from `OperationCanceledException`, `IOException`, or a cancelled request token.

The Web host projects the value through `IWebResponseClientFaultFeature` in `Web.Server`; its existing
response-starting feature implements that second interface over the same control. This covers a
bodiless streamed `GET` without adding a feature object or another response interceptor. The core and
transport reference no Web package, and the Web root gains no contract. `Web.ErrorHandling` reads the
projection before classifying the caught exception, so even a middleware wrapper that changes the
exception type leaves the exchange cancelled, with no `OnException` or Error log. #1401 can consume
the same fact for its event source.

**Compatibility with 10.0.0-preview.1.** The default member is additive. Outcome classification changes
under decision 15: peer-abandoned responses and a slow HTTP/1.1 streamed response no longer reach the
application fault observer as faults. The cross-protocol minimum-rate enforcement was added by #1384;
the response latch adds no new rate policy or wire reset code.

### Exchange control reports

The transport exposes its per-exchange **exchange control** as an
`IHttpExchangeControl` on `HttpExchangeInterceptorResponseContext.Control` — the
single generic surface for the transport-owned wire mechanisms outside the
normal response path: interim (`1xx`) writes (`CanWriteInterimResponse` /
`WriteInterimResponseAsync`), the raw-connection takeover (`CanTakeOver` /
`TakeOver()`), the extended CONNECT stream tunnel (`CanAcceptTunnel` /
`AcceptTunnelAsync`), and the client-fault reports (`ClientFaultStatusCode` and
`ResponseAbortedByClient`, [the client-fault report](#the-client-fault-report)).
One control deliberately replaces the former per-capability contracts (`IHttpConnectionTakeover`, `IHttpInterimResponseWriter`, and the
core `IHttpExtendedConnectFeature` that #1368 removed): a new
wire mechanism composes from the hooks plus this control instead of adding a
new core abstraction and new transport plumbing. The control carries
mechanisms, not decisions — aborting is `IHttpContext.Cancel`, on the
application surface.

**Capability probes are report-don't-throw.** `CanTakeOver` is `false` on
HTTP/2 / HTTP/3 (multiplexed streams over a shared connection; those protocols
removed `Upgrade`) and once the exchange can no longer be handed off;
`CanAcceptTunnel` is `false` on HTTP/1.1, on any exchange that is not an
extended CONNECT, and once the tunnel was accepted or the response started;
`CanWriteInterimResponse` flips to `false` once the final head is committed.
Feature packages degrade (e.g. `context.Upgrade == null`,
`context.ExtendedConnect == null`) rather than surface a feature whose action
could never work. `ClientFaultStatusCode` reports in the same spirit: `null`
until the transport latches a client fault, then that status for the rest of the
exchange, and `null` from any control that does not report faults. `ResponseAbortedByClient`
likewise stays `false` unless the transport latches a response the client abandoned, and then
remains `true` without implying a status or requiring exception-type inspection.

## The body signal: `HttpExchangeInterceptorRequestContext.CanHaveBody` (#1378)

The control exists only in an exchange's response phase, which costs a response sink and the control
itself, so a host that publishes the report joins the response phase only of an exchange whose body
can fault. On HTTP/1.1 the head shows that: a `Transfer-Encoding`, or a `Content-Length` other than
zero. An HTTP/2 or HTTP/3 request need not declare its body at all: content follows a HEADERS frame
that did not end the stream, or a request stream that has not ended. Only the transport sees that, so
the request context carries it as a generic fact, as it carries `Protocol` for an extended CONNECT:

- **HTTP/1.1** — the framing declares a body: a `Transfer-Encoding`, or a `Content-Length` other than
  zero. The transport decides the framing before the head hooks run, so they see it.
- **HTTP/2** — the HEADERS frame did not carry `END_STREAM`, whatever `content-length` it declares:
  DATA or a trailer section may still follow, and either can make the request malformed.
- **HTTP/3** — the request stream had not visibly ended with its HEADERS frame, or a positive declared
  content length still requires validation. The transport looks for a FIN already buffered behind
  the frame, without reading or waiting. The QUIC driver publishes a clean transport end through
  Connections' `IMultiplexedStream.IsInputCompleted` root member (#1427, #1453). The transport samples
  completion before probing the pipe, so an asynchronous producer publishing its final DATA between
  the two cannot hide those bytes. It combines completion with an empty pipe buffer, so a head
  followed directly by FIN reports `false` even before the stream-backed pipe reads zero bytes,
  unless a positive content length requires the body read to report missing content. Buffered DATA,
  open streams, and drivers without observed completion remain conservative. The in-memory pipe
  reports FIN directly.
- **CONNECT** never has one: the octets after its head are tunnel traffic, not content.

The alternative #1378 weighed — a host that joins every HTTP/2 and HTTP/3 exchange — gives up the fast
path for every one of them, a `GET` included, to learn what the transport already knows. The property
is optional and defaults to `true`: a context built outside a transport, a test fake, can have a body
until it says it cannot, which errs toward taking the response phase rather than toward missing a
fault.

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

The first added interceptor is held in one singleton array (#1389). With no
listener response interceptors, the transport retains that same array; a
second distinct addition promotes the parse-side storage to a list. Equality
deduplication and listener-first order remain unchanged. This changes neither
public contracts nor preview.1 behavior: per-exchange response joining was
introduced after preview.1.

## The request context's data rates (#1383, #1384)

The request-parse context, `HttpExchangeInterceptorRequestContext`, is also where a request changes
the transport's minimum data rates for itself, beside the body-size knob:

- **The request-body rate cell (#1383).** `MinRequestBodyDataRate` (an `HttpMinDataRate`, `null` =
  no rate) is seeded by every transport from its listener limits and frozen at the first read of the
  request body on every protocol (`FreezeMinRequestBodyDataRate()` /
  `IsMinRequestBodyDataRateReadOnly`), so a hook, middleware or endpoint can lower, raise or disable
  it for one request until then. HTTP/2 freezes the body-size knob at dispatch but this one only at
  its reader's first read, so the two freeze independently.
- **The response rate cell (#1384).** `MinResponseDataRate` is the slow-read defence's knob: seeded
  the same way, frozen at the response's first write (`FreezeMinResponseDataRate()` /
  `IsMinResponseDataRateReadOnly`), independently of the request-body knobs. The context therefore
  lives as long as the exchange that ran request hooks, not only until its body is consumed: the
  transport hands it to the exchange so the response can read the knob when it starts.

`HttpMinDataRate` is a core value object for this reason: the context, the transport's listener
limits and the typed feature in
[`Http.RequestLimits`](../assimalign-cohesion-http-requestlimits/index.md) all name it, and core can
name nothing in `Http.Connections`. Moving it from there was a source and binary break (the
transport's design,
[per-request data rates](../assimalign-cohesion-http-connections/design.md#per-request-data-rates-1383),
records it).

The context's v1 members are `required`, and every member added since is optional with a default, so
existing construction sites, test fakes included, keep compiling. `Protocol`,
`MinRequestBodyDataRate` and `MinResponseDataRate` are such members: a context built without them
reads `null`.

## Feature lookup by contract

`IHttpFeatureCollection` is keyed by name. `Features.Get<TFeature>()`
(`HttpFeatureCollectionExtensions`) looks up a contract instead: it returns the
first installed feature that implements `TFeature`, in enumeration order. That
order is the local features, then each defaults level's features that no level
above it hides by name.

- **No allocation on `HttpFeatureCollection`.** Every transport context carries
  that exact type, and per-request readers call the lookup on every request: the
  `Http.Forwarded` `Effective*` members, which host filtering, routing's
  `RequireHost` and rate limiting read, among others. Enumerating through
  `OfType<TFeature>().FirstOrDefault()` allocated the LINQ iterator and the
  collection's `yield` enumerator on each call. The lookup now scans the backing
  dictionaries with their struct enumerators and walks the defaults chain itself
  (raised by the #1077 review). #1337 finishes that path by enumerating entries
  directly: `Dictionary.Values` otherwise creates a 24-byte wrapper on the first
  lookup of every populated exchange, even after the lookup code is warmed up.
  Both `Get<T>` and the search in `Set<T>(null)` use this path.
- **The same answer as enumeration.** A defaults-level feature is skipped when a
  level above installs its name, whatever that level's feature implements, as
  the enumerator skips it. A defaults source of any other type, a derived
  `HttpFeatureCollection` included, is enumerated through its own enumerator,
  because a derived type can re-implement `IEnumerable<IHttpFeature>`. That path
  now uses a plain loop, removing the LINQ iterator; any allocation in the
  implementation's own enumerator remains. Exact-type checks preserve custom
  enumeration order and disposal. The fallback selects the non-generic
  `IEnumerable` enumerator, as `OfType` did, even when an implementation gives
  its generic enumerator a different order. A universal allocation guarantee would need
  an optional public lookup seam; #1337 leaves that boundary question open and
  adds no public interface member.
- **Still `O(n)`.** The scan covers typically fewer than ten features. A caller
  that needs `O(1)` caches the resolved feature.

The #1381 capacity hint still sizes the same name-keyed dictionary; no second
index or invalidation cache is needed. Web.Compression, Web.RequestTimeouts,
Web.Rewrite and Web.Diagnostics request views forward `_inner.Features`, so they
retain the same collection and use the same fast path.

`HttpFeatureCollectionTests` pins warmed hits/misses through defaults and checks
shadowing against enumeration. `HttpFeatureLookupTests` also checks fresh
presized exchanges, typed removal, reused slots and a derived collection's
custom enumeration. The ordering tests pin existing behavior; the six allocation
cases fail on the Stage 14 base with 24,000 bytes for 1,000 fresh exchanges and
pass with zero after this fix.

**Allocation comparison (#1337).** Windows x64, .NET 10.0, Debug, 1,000 fresh
exchanges with code warmed on a separate collection, four hits and four misses
per exchange; construction and stamping are outside the measured region:

| Features / initial capacity | preview.1 LINQ lookup expression | Stage 14 base fast path | #1337 entry scan |
|---|---:|---:|---:|
| 4 | 1,112 B/exchange | 24 B/exchange | 0 B/exchange |
| 16 | 1,112 B/exchange | 24 B/exchange | 0 B/exchange |

The comparison evaluates the original LINQ expression against today's collection,
not a whole-server benchmark of preview.1. The repeated-lookup cost is 136 bytes
per LINQ lookup plus the first 24-byte Values wrapper; the entry scan allocates
zero on the first lookup and thereafter. Run the core tests filtered to
`HttpFeatureLookupTests` with the normal console logger to reproduce the numbers.
Lookup semantics and the public surface are unchanged from 10.0.0-preview.1.

## Why the seam is core and the features are not

Seams live in the core, and features live in packages. The interceptor seam is a compile-time
contract because the transport must enforce mutation mid-parse, attach features before dispatch, and
replace streams, none of which a loosely typed `Items` key can express. A capability that only needs
one-way publication after parsing can still use an `Items` key.

A transport never puts a feature contract in this core. It publishes what it knows about a
connection as a *facet* on the exchange's connection info: an extra interface the
`HttpConnectionInfo` it hands out also implements, found with a type test on that HTTP snapshot
(the TLS handshake contract is the `Connections.Tls` package's `ITlsConnectionInfo`, which
`Assimalign.Cohesion.Http.Tls` turns into `context.TlsConnection`). The transport obtains these TLS
facts through the underlying connection's `Tls` feature; connection decorators forward
`InnerFeatures` (#1452). A wire mechanism only the transport can perform is offered through
`IHttpExchangeControl`, which a feature package wraps (`context.Upgrade` over `TakeOver`,
`context.ExtendedConnect` over `AcceptTunnelAsync`). A request fact a hook needs that the parsed head
cannot show is a member of the request context (`Protocol`, the validated `:protocol` of an extended
CONNECT; `CanHaveBody`, whether content may follow the head). Either way the feature package owns
the application-facing contract, and the transport references no feature package. The rule has no
exceptions left: the last two transport-produced contracts moved out in #1367 (TLS) and #1368
(extended CONNECT).

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpPath.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpIfRange.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpStatusCode.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpMinDataRate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpHeaderCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpQueryCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQueryValue.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpQueryCollection.cs`.
