# Assimalign.Cohesion.Connections.InMemory

Provides socketless stream and multiplexed connections for live protocol tests.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Paired pipes are cross-wired so both ends can exchange bytes repeatedly. Listener/factory pairs
preserve the production connection contracts while avoiding operating-system sockets. This driver
moves bytes; application protocol behavior belongs to its consumers.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `InMemoryConnectionFactory` | `src/InMemoryConnectionFactory.cs` |
| `InMemoryConnectionListener` | `src/InMemoryConnectionListener.cs` |
| `InMemoryConnectionPair` | `src/InMemoryConnectionPair.cs` |
| `InMemoryEndPoint` | `src/InMemoryEndPoint.cs` |
| `InMemoryMultiplexedConnectionFactory` | `src/InMemoryMultiplexedConnectionFactory.cs` |
| `InMemoryMultiplexedConnectionListener` | `src/InMemoryMultiplexedConnectionListener.cs` |
| `InMemoryMultiplexedConnectionPair` | `src/InMemoryMultiplexedConnectionPair.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionPair.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryEndPoint.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionPair.cs`.
