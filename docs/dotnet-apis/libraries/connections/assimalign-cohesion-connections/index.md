# Assimalign.Cohesion.Connections

Defines byte-stream, multiplexed, and datagram connection contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

A stream connection is itself an `IDuplexPipe`, with input and output anchored to its holder.
Listeners bind explicitly and factories establish outbound connections. Capabilities describe
delivery and security; connection layers transform connections at establishment without introducing
a parallel transport abstraction.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Connection` | `src/Connection.cs` |
| `ConnectionAbortedException` | `src/Exceptions/ConnectionAbortedException.cs` |
| `ConnectionCapabilities` | `src/ValueObjects/ConnectionCapabilities.cs` |
| `ConnectionDelivery` | `src/ValueObjects/ConnectionDelivery.cs` |
| `ConnectionDirection` | `src/ValueObjects/ConnectionDirection.cs` |
| `ConnectionException` | `src/Exceptions/ConnectionException.cs` |
| `ConnectionExtensions` | `src/Extensions/ConnectionExtensions.cs` |
| `ConnectionFactory` | `src/ConnectionFactory.cs` |
| `ConnectionListener` | `src/ConnectionListener.cs` |
| `ConnectionProtocol` | `src/ValueObjects/ConnectionProtocol.cs` |
| `ConnectionResetException` | `src/Exceptions/ConnectionResetException.cs` |
| `ConnectionSecurity` | `src/ValueObjects/ConnectionSecurity.cs` |
| `ConnectionState` | `src/ValueObjects/ConnectionState.cs` |
| `DatagramConnection` | `src/DatagramConnection.cs` |
| `DatagramReceiveResult` | `src/ValueObjects/DatagramReceiveResult.cs` |
| `DuplexPipeStream` | `src/DuplexPipeStream.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Assimalign.Cohesion.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Connection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Exceptions/ConnectionAbortedException.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionCapabilities.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionDelivery.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionDirection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Exceptions/ConnectionException.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Extensions/ConnectionExtensions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionProtocol.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Exceptions/ConnectionResetException.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionSecurity.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/ConnectionState.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/DatagramConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ValueObjects/DatagramReceiveResult.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/DuplexPipeStream.cs`.
