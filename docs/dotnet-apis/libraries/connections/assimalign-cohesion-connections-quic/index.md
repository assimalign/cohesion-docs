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
