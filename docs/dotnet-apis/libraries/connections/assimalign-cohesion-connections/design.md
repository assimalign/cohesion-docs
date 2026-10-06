# Assimalign.Cohesion.Connections design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A stream connection is itself an `IDuplexPipe`, with input and output anchored to its holder.
Listeners bind explicitly and factories establish outbound connections. Capabilities describe
delivery and security; connection layers transform connections at establishment without introducing
a parallel transport abstraction.

## Handshake facts: `ITlsConnectionInfo`

`ConnectionSecurity.Tls` says a connection is secured; it cannot say what the handshake agreed,
because capabilities describe a transport class and the handshake is per connection. An application
protocol needs that agreement: HTTP chooses between HTTP/2 and HTTP/1.1 from the application
protocol ALPN selected (RFC 7301), and shows its handlers the client certificate, TLS version, and
cipher suite. `ITlsConnectionInfo` is the typed facet that carries it. A connection that terminates
TLS implements it beside its connection contract, and a consumer finds it with a type test on the
connection it holds:

- **The secured `IConnection`** the TLS layer returns (`Assimalign.Cohesion.Connections.Security`).
- **A multiplexed connection** whose transport is TLS itself (QUIC, RFC 9001).

The facet reports four values, all fixed once the handshake completes:

| Member | Meaning |
|---|---|
| `ApplicationProtocol` | the protocol ALPN selected (RFC 7301), or none when the client offered none |
| `TlsProtocol` | the negotiated TLS version |
| `CipherSuite` | the negotiated cipher suite |
| `RemoteCertificate` | the certificate the peer presented, or `null`: the client's on a server-side connection (present only when the server requested one, RFC 8446 §4.3.2), the server's on a client-side one |

The connection owns `RemoteCertificate` and disposes it with itself. Both platform stacks hand
ownership of the peer certificate to whoever reads it (`SslStream.RemoteCertificate`,
`QuicConnection.RemoteCertificate`), so an implementation that reads it to report it must release
it; a consumer that keeps the certificate past the connection copies it.

Why this shape:

- **It lives in the contracts library** so the TLS layer and the QUIC driver can each implement it
  without a new reference, and a consumer reads it while depending on the contracts alone. HTTP
  never references the TLS layer.
- **Not an `Items` bag**: the facts are few, typed, and fixed at handshake time, which is exactly
  what a typed contract is for.
- **Not a member of `IConnection` or `IMultiplexedConnection`**: every driver and test double would
  have to answer a question only TLS-terminating connections can, and a QUIC stream would answer
  for its whole connection.
- **A facet, not a base type**: `IConnection` and `IMultiplexedConnection` are different shapes, and
  both have TLS-terminating implementations, so the facet extends neither.

The cost is the usual one for decorators. A layer composed above TLS that returns a new connection
hides the facet unless it implements the facet too and forwards it. A pass-through layer, which
returns the connection it was given, keeps it visible.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Assimalign.Cohesion.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src`.
