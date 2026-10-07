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

## The TLS connection feature

`IHttpTlsConnectionFeature` tells a handler how the connection its exchange arrived on is secured:
`ClientCertificate` (`null` when the client presented none), `Protocol` (the TLS version),
`CipherSuite`, and `ApplicationProtocol` (what ALPN selected, RFC 7301). The server transport
(`Assimalign.Cohesion.Http.Connections`) attaches it to every exchange that arrived over TLS —
HTTP/1.1 and HTTP/2 over the TLS layer, HTTP/3 over QUIC, which carries TLS 1.3 itself (RFC 9001) —
and attaches none to a cleartext exchange. Code reads it as `context.TlsConnection`
(`HttpTlsConnectionExtensions`), the same accessor shape as the other features.

The values belong to the connection: the transport copies them from the connection's handshake
once, and every exchange on the connection carries the same instance. A client certificate exists
only when the server's TLS options requested one in the handshake (RFC 8446 §4.3.2); HTTP/2 rules
out asking later (RFC 9113 §9.2.3), so there is no "renegotiate for a certificate" member.

**Why the contract lives in the core.** A feature contract belongs to the package that produces the
capability. Here the producer is the transport itself, which references no feature package, so the
contract has to be visible to the transport and to applications alike: the core.
`IHttpConnectionInfo`, the endpoints of the same connection, sits here for the same reason. The
implementation stays in the transport. Rejected:

- **An `Items`-key bridge with a feature package**, as extended CONNECT used until it gained its
  tunnel (see [the extended CONNECT feature](#the-extended-connect-feature)). That fits a single
  string published one way; a session of four typed values, one of them a certificate with an
  owner, would travel as an untyped object, and a new package would exist only to cast it back.
- **New members on `IHttpConnectionInfo`.** Adding members to the interface breaks every
  implementation, test doubles included, while a feature is optional by construction: an exchange
  without TLS simply has none, and a host other than the transport can attach its own.

**Ownership and lifetime.** The certificate belongs to the connection, which disposes it when the
connection closes. The feature is therefore not disposable: an exchange's disposal walk disposes
the disposable features it carries, and this one must survive the exchange. Code that keeps the
certificate beyond the exchange copies it.

**What it does not do.** It reports; it does not decide. Requesting, requiring, and validating
client certificates is the server's TLS configuration (`Assimalign.Cohesion.Connections.Security`'s
`TlsServerOptions`, exposed on `Web.Hosting`'s endpoints), and authenticating a request from the
certificate belongs to an authentication handler, which does not exist yet.

## The extended CONNECT feature

`IHttpExtendedConnectFeature` is the HTTP/2 and HTTP/3 *extended CONNECT* capability (RFC 8441,
RFC 9220): a `CONNECT` request that carries `:protocol` asks to run another protocol — most often
WebSocket — over its one stream. The feature reports the requested `Protocol` and offers
`AcceptAsync`, which answers `200` without ending the stream and returns the stream as a duplex
`Stream`:

- **The head** carries the headers the application set before accepting, without `Content-Length`,
  `Transfer-Encoding` (RFC 9110 §9.3.6) or the connection-specific fields (RFC 9113 §8.2.2, RFC 9114
  §4.2). Any status the application set is replaced by `200`, and a body it wrote is discarded.
- **Reads** return the client's `DATA` and return 0 once the client ends its side (HTTP/2
  `END_STREAM`, HTTP/3 FIN).
- **Writes** go out as `DATA` at once, unbuffered and paced by the peer's flow control.
- **Disposing** ends the server's side; the client may still send until it ends its own.
- **A peer reset or a lost connection** faults pending and later reads and writes with an
  `IOException`.
- **Misuse** — accepting twice, after the response started, or on a cancelled exchange — throws
  `InvalidOperationException`.

Accepting takes the exchange over, as an HTTP/1.1 protocol upgrade does: the transport no longer
writes the application's response, and the exchange interceptors' response-head and after-response
hooks do not run for it. The tunnel lasts as long as the exchange: when the handler returns, the
transport ends a tunnel the application left open, and a cancelled exchange resets the stream. The
server transport (`Assimalign.Cohesion.Http.Connections`) installs the feature on every valid
extended CONNECT and on no other exchange; code reads it as `context.ExtendedConnect`, an accessor
that ships in `Assimalign.Cohesion.Http.ExtendedConnect`. The Http area's ADR 1 records why the
tunnel exists: server WebSockets on HTTP/2 and HTTP/3.

**Why the contract lives in the core.** The same rule as the TLS feature: the producer of the
capability is the transport. Accepting writes a HEADERS block without `END_STREAM` and frames `DATA`
under the stream's flow-control windows, which only the transport can do, and the transport
references no feature package. Until the tunnel existed the feature only reported `:protocol`, and
the transport published that string under an `IHttpContext.Items` key for the package to wrap. An
accept call cannot travel as a string, so the bridge is gone and the contract moved here (#1316). The
namespace is unchanged, so source that referenced the package compiles as before, but a binary built
against the old assembly has to be rebuilt.

**What it does not do.** It carries octets, not a protocol: WebSocket framing comes from the BCL over
the accepted stream, and the handshake from `Http.WebSockets`. A classic `CONNECT` (no `:protocol`)
carries no such feature, and nothing here dials the request's authority.

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

The interceptor seam is a compile-time contract because the transport must enforce mutation
mid-parse, attach features before dispatch, and replace streams, none of which a loosely typed
`Items` key can express. A capability that only needs one-way publication after parsing can still
use an `Items` key; one the transport itself must implement puts its contract in this core, as the
TLS and extended CONNECT features do.

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExtendedConnectFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpExchangeInterceptorRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/ValueObjects/HttpQuery.cs`.
