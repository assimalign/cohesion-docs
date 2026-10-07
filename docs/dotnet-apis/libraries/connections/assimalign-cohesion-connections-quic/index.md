# Assimalign.Cohesion.Connections.Quic

Adapts QUIC connections and streams to Cohesion multiplexed connection contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

`QuicMultiplexedConnection` reports its TLS 1.3 handshake (the ALPN protocol, the cipher suite, the
peer certificate) through the contracts' `ITlsConnectionInfo`, so HTTP/3 can show the session to
handlers. See the [design](design.md#handshake-facts).

A stream's `ConnectionClosed` also fires when the peer abandons the stream (`RESET_STREAM` or
`STOP_SENDING`) or the QUIC connection is lost, which is how an HTTP/3 server fires
`RequestCancelled` for a request its client cancelled (#1329). See the
[design](design.md#connectionclosed-on-an-abandoned-stream).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `QuicConnectionFactory` | `src/QuicConnectionFactory.cs` |
| `QuicConnectionListener` | `src/QuicConnectionListener.cs` |
| `QuicConnectionListenerOptions` | `src/QuicConnectionListenerOptions.cs` |
| `QuicMultiplexedConnection` | `src/QuicMultiplexedConnection.cs` |
| `QuicConnectionFactoryOptions` | `src/QuicConnectionFactoryOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicMultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionFactoryOptions.cs`.
