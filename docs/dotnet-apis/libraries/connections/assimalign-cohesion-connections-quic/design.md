# Assimalign.Cohesion.Connections.Quic design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Quic`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

## Handshake facts

QUIC runs a TLS 1.3 handshake inside its own (RFC 9001), so a QUIC connection is a TLS-terminating
connection in the contracts' sense. `QuicMultiplexedConnection` implements `ITlsConnectionInfo` and
reports what that handshake negotiated: the ALPN application protocol, the TLS version (always
TLS 1.3), the cipher suite, and the peer's certificate (the client's on a server-side connection,
present when the listener's `ServerAuthenticationOptions` requested one). The values are the same
on every stream of the connection; the streams themselves do not implement the facet. The driver
reads the values and never branches on them, so it stays free of protocol semantics.

Reading `QuicConnection.RemoteCertificate` hands the certificate's ownership to the reader (a
`QuicConnection` no longer disposes a certificate it has exposed), so `QuicMultiplexedConnection`
disposes the peer certificate after it has disposed the QUIC connection.

## A failed handshake never ends the accept

`System.Net.Quic` runs each inbound connection's handshake in the background. When one fails, it
reports the failure from the next `QuicListener.AcceptConnectionAsync`, as an
`AuthenticationException` or a `QuicException`. Causes include a client certificate the policy
refuses, a handshake that exceeds its timeout, and an error from the connection-options callback.
The listener stays usable.

`QuicConnectionListener.AcceptAsync` treats each such exception as that one connection's: it
reports `HandshakeFailed` (Warning: `listenerId`, `exceptionType`, `exceptionMessage`) and accepts
the next connection (#1304). Only the listener's disposal and the caller's own cancellation leave
`AcceptAsync`. Before #1304 a single client without a required certificate stopped the HTTP/3
endpoint. Kestrel's QUIC transport makes the same call.

Pending handshakes are bounded by `Backlog`, which becomes `QuicListenerOptions.ListenBacklog`: it
counts connections whose handshake is in progress plus those waiting to be accepted. The handshake
timeout is `System.Net.Quic`'s default of 10 seconds. `TlsServerOptions.HandshakeTimeout` and
`MaxConcurrentHandshakes` belong to the TCP TLS layer and do not apply here.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.
