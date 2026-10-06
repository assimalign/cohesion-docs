# Assimalign.Cohesion.Http design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

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

- **An `Items`-key bridge with a feature package**, as extended CONNECT does. That fits a single
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

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.
