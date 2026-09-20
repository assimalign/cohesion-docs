# Assimalign.Cohesion.Connections.Udp

Provides message-oriented User Datagram Protocol (UDP) connections.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

The factory creates `IDatagramConnection` instances rather than pretending datagrams are ordered
streams. The project references the connection contracts and Core. There is no separate project
design document; the factory and datagram implementation define the current behavior.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `UdpBindOptions` | `src/UdpBindOptions.cs` |
| `UdpConnectionFactory` | `src/UdpConnectionFactory.cs` |
| `UdpConnectOptions` | `src/UdpConnectOptions.cs` |
| `UdpTraceCode` | `src/UdpTraceCode.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/Assimalign.Cohesion.Connections.Udp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpBindOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpConnectOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpTraceCode.cs`.
