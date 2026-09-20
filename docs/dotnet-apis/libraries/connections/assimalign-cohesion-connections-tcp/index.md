# Assimalign.Cohesion.Connections.Tcp

Provides socket-backed streams over TCP, Unix domain sockets, and inherited listening sockets.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

One socket data path serves the supported endpoint forms. Binding is explicit, and Unix socket-file
lifetime belongs to the listener. Shared pipe plumbing is compiled from the connection contracts
project rather than exposed as driver-specific public infrastructure.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `TcpConnectionFactory` | `src/TcpConnectionFactory.cs` |
| `TcpConnectionFactoryOptions` | `src/TcpConnectionFactoryOptions.cs` |
| `TcpConnectionListener` | `src/TcpConnectionListener.cs` |
| `TcpConnectionListenerOptions` | `src/TcpConnectionListenerOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/Assimalign.Cohesion.Connections.Tcp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactoryOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionListenerOptions.cs`.
