# Assimalign.Cohesion.Connections

Defines a shared connection lifecycle with duplex, multiplexed, and datagram data planes.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Connection, listener, factory, and layer contracts shared by Cohesion network transports and the
application protocols built on top of them. This is the lowest layer of the networking stack:
concrete transports implement it, application protocols consume it, and connection
transformations (such as TLS) compose over it.

## Key Types

- `IConnection` / `Connection` — shared identity, endpoints, capabilities, features, state, and lifetime.
- `IDuplexConnection` / `DuplexConnection` — a live byte channel with `Input`, `Output`, and `Direction`.
- `IConnectionListener` / `ConnectionListener<TConnection>` — explicitly binds and accepts inbound connections
  (server side).
- `IConnectionFactory` / `ConnectionFactory<TConnection>` — establishes outbound connections (client side).
- `IMultiplexedConnection` / `MultiplexedConnection` — carries multiple `IMultiplexedConnectionStream` streams;
  `OpenStreamAsync(ConnectionDirection, ...)` for bidirectional or supported unidirectional streams,
  and `Abort(code, reason)` for a transport-specific coded connection close.
- `IMultiplexedConnectionStream` / `MultiplexedConnectionStream` — an `IDuplexConnection` with coded whole-stream reset
  (`Abort(code, reason)`) and clean input completion (`IsInputCompleted`).
- `IMultiplexedConnectionListener` / `IMultiplexedConnectionFactory` (+ bases).
- `IConnectionFeature` / `IConnectionFeatureCollection` / `ConnectionFeatureCollection` — typed,
  named optional capabilities on all three connection shapes, with own features before wrapped
  features, replacement by name, and removal that hides the wrapped name.
- `IDatagramConnection` / `DatagramConnection` — message-oriented send/receive (e.g., UDP).
- `IConnectionLayer` — duplex-to-duplex transformation; composed via
  `listener.Use(layer)` / `factory.Use(layer)`. A layered listener upgrades each accepted connection
  on its own task, at most 512 at a time (`listener.Use(layer, maxConcurrentUpgrades)`), and a failed
  or timed-out upgrade closes only that connection.
- Optional packages keep the family namespace: `Connections.Tls` owns `ITlsConnectionInfo`
  (`connection.Tls`); `Connections.StreamAbort` owns `IMultiplexedStreamAbort`
  (`stream.DirectionalAbort`); `Connections.StreamLimits` owns
  `IMultiplexedConnectionStreamLimits` (`connection.StreamLimits`).
- `ConnectionCapabilities`, `ConnectionProtocol`, `ConnectionDelivery`, `ConnectionSecurity`,
  `ConnectionDirection`, `ConnectionState`, `ConnectionId`, `DatagramReceiveResult`.
- `DuplexPipeStream`, `ConnectionExtensions.AsStream()` — lazy pipe-to-stream adaptation.
- `ConnectionException`, `ConnectionAbortedException`, `ConnectionResetException` (which carries the
  peer's `ApplicationErrorCode` when a driver reports a coded stream reset through it).

## Accepting and observing connections

Use `IDuplexConnectionListener` for byte protocols and `IMultiplexedConnectionListener` for
stream multiplexers. Both also implement `IConnectionListener`; its `AcceptAsync` drains the
same queue. `Connections` is one live, concurrently enumerable collection, removed by each
connection's closure token. Only the accepting owner performs I/O and disposes the connection;
disposing its listener leaves it open. See [design](design.md) for retention and allocation costs.

## Registering and reading features

Drivers override `ConfigureFeatures(IConnectionFeatureCollection)` to register their capabilities.
Layers override `InnerFeatures` with their wrapped connection's `Features`, so optional capabilities
remain visible without forwarding individual contracts. The guided bases create and configure the
collection once on first access. Consumers may register their own typed features with
`connection.Features.Set(feature)`, look up by exact name or `Get<TFeature>()`, or hide an inherited
name with `Remove(name)`. Cache a feature when using it repeatedly during I/O.

Adding `Features` to `IConnection`, `IMultiplexedConnection`, and `IDatagramConnection` breaks direct
implementers of 10.0.0-preview.1. The guided bases provide the implementation; see the accepted
breaking change and layering rules in [design](design.md).

## Multiplexed stream lifecycle

A whole-stream `Abort(code, reason)` resets both directions and ends the lifecycle. A direction
already ended keeps its first code. Drivers validate transport-specific codes; negative codes are
always invalid. The first connection abort or disposal decides its close.

`IsInputCompleted` reports a clean peer half-close without reading or waiting. Combine it with an
empty input buffer before concluding no bytes remain; `false` means the end is not known yet.
Per-direction aborts remain the optional `stream.DirectionalAbort` feature.

A bidirectional-only transport rejects `OpenStreamAsync(WriteOnly)` with `NotSupportedException`
and never accepts a `ReadOnly` stream. Accept/open return types changed from `IConnection` to
`IMultiplexedConnectionStream` (from `Connection` to `MultiplexedConnectionStream` on the guided base), and coded
connection abort is now required. These break preview.1 implementers and existing binaries under
decisions 15 and 56; see [design](design.md).

## Source Layout

- `src/Abstractions` — interfaces (connection shapes, listeners/factories, the layer arrow).
- `src/` — guided abstract base classes and `DuplexPipeStream`.
- `src/Extensions` — layer composition (`Use`) and connection conveniences (`AsStream`).
- `src/Internal` — layered listener/factory decorators, and the layered listener's internal event
  source (`Assimalign.Cohesion.Connections`).
- `src/ValueObjects` — value types and enums.
- `src/Exceptions` — area exception root and specific exceptions.

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
| `ConnectionFactory<TConnection>` | `src/ConnectionFactory.cs` |
| `ConnectionFeatureCollection` | `src/ConnectionFeatureCollection.cs` |
| `ConnectionFeatureCollectionExtensions` | `src/Extensions/ConnectionFeatureCollectionExtensions.cs` |
| `ConnectionListener<TConnection>` | `src/ConnectionListener.cs` |
| `ConnectionProtocol` | `src/ValueObjects/ConnectionProtocol.cs` |
| `ConnectionResetException` | `src/Exceptions/ConnectionResetException.cs` |
| `ConnectionSecurity` | `src/ValueObjects/ConnectionSecurity.cs` |
| `ConnectionState` | `src/ValueObjects/ConnectionState.cs` |
| `DatagramConnection` | `src/DatagramConnection.cs` |
| `DatagramReceiveResult` | `src/ValueObjects/DatagramReceiveResult.cs` |
| `DuplexConnection` | `src/DuplexConnection.cs` |
| `DuplexConnectionFactory` | `src/DuplexConnectionFactory.cs` |
| `DuplexConnectionListener` | `src/DuplexConnectionListener.cs` |
| `DuplexPipeStream` | `src/DuplexPipeStream.cs` |
| `IConnection` | `src/Abstractions/IConnection.cs` |
| `IConnectionFactory` | `src/Abstractions/IConnectionFactory.cs` |
| `IConnectionFactory<TConnection>` | `src/Abstractions/IConnectionFactory.Generic.cs` |
| `IConnectionFeature` | `src/Abstractions/IConnectionFeature.cs` |
| `IConnectionFeatureCollection` | `src/Abstractions/IConnectionFeatureCollection.cs` |
| `IConnectionLayer` | `src/Abstractions/IConnectionLayer.cs` |
| `IConnectionListener` | `src/Abstractions/IConnectionListener.cs` |
| `IConnectionListener<TConnection>` | `src/Abstractions/IConnectionListener.Generic.cs` |
| `IDatagramConnection` | `src/Abstractions/IDatagramConnection.cs` |
| `IDuplexConnection` | `src/Abstractions/IDuplexConnection.cs` |
| `IDuplexConnectionFactory` | `src/Abstractions/IDuplexConnectionFactory.cs` |
| `IDuplexConnectionListener` | `src/Abstractions/IDuplexConnectionListener.cs` |
| `IMultiplexedConnection` | `src/Abstractions/IMultiplexedConnection.cs` |
| `IMultiplexedConnectionFactory` | `src/Abstractions/IMultiplexedConnectionFactory.cs` |
| `IMultiplexedConnectionListener` | `src/Abstractions/IMultiplexedConnectionListener.cs` |
| `IMultiplexedConnectionStream` | `src/Abstractions/IMultiplexedConnectionStream.cs` |
| `MultiplexedConnection` | `src/MultiplexedConnection.cs` |
| `MultiplexedConnectionFactory` | `src/MultiplexedConnectionFactory.cs` |
| `MultiplexedConnectionListener` | `src/MultiplexedConnectionListener.cs` |
| `MultiplexedConnectionStream` | `src/MultiplexedConnectionStream.cs` |

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

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/ConnectionFeatureCollection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Extensions/ConnectionFeatureCollectionExtensions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionFeature.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionFeatureCollection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IMultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IMultiplexedConnectionStream.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedConnectionStream.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/DuplexConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/DuplexConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/DuplexConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionFactory.Generic.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionLayer.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IConnectionListener.Generic.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IDatagramConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IDuplexConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IDuplexConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IDuplexConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IMultiplexedConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IMultiplexedConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedConnectionListener.cs`.
