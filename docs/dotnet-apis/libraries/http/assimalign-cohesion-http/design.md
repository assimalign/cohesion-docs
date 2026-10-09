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
  section cannot carry — pseudo-headers, connection-specific fields, and the
  `IsProhibitedInTrailers` set — with `ArgumentException`.
- **A response to `HEAD` sends no trailers**, and a `CONNECT` exchange reports the response
  collection unsupported, since its stream becomes a DATA-only tunnel.
- **HTTP/1.1 response trailers stay out** (decision 18): a buffered HTTP/1.1 response carries
  `Content-Length`, and HTTP/1.1 clients rarely consume chunked trailers. If a consumer appears, the
  model makes it a drop-in: `IsSupported = true` and a populated collection.

Trailers were decided as HTTP semantics, apart from gRPC, which stays outside the HTTP/Web program.

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpExchangeInterceptorRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQuery.cs`.
