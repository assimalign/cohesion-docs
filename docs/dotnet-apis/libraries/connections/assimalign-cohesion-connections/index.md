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

`ITlsConnectionInfo`, owned by [Connections.Tls](../assimalign-cohesion-connections-tls/index.md),
reports what a TLS handshake negotiated: the ALPN application protocol, the TLS version, the cipher
suite, and the peer's certificate. Connections that terminate TLS register it as a feature, and a
consumer reads the `Tls` extension property. See the
[design](design.md#handshake-facts-itlsconnectioninfo).

`IMultiplexedConnection` / `MultiplexedConnection` carries multiple `IMultiplexedStream` streams;
`OpenStreamAsync(ConnectionDirection, ...)` opens bidirectional or supported unidirectional streams,
and `Abort(code, reason)` performs a transport-specific coded connection close.
`IMultiplexedStream` / `MultiplexedStream` is an `IConnection` with coded whole-stream reset
(`Abort(code, reason)`) and clean input completion (`IsInputCompleted`).

`IConnectionFeature` / `IConnectionFeatureCollection` / `ConnectionFeatureCollection` provides
typed, named optional capabilities on all three connection shapes, with own features before wrapped
features, replacement by name, and removal that hides the wrapped name. Optional packages keep the
family namespace: [Connections.Tls](../assimalign-cohesion-connections-tls/index.md) owns
`ITlsConnectionInfo` (`connection.Tls`);
[Connections.StreamAbort](../assimalign-cohesion-connections-streamabort/index.md) owns
`IMultiplexedStreamAbort` (`stream.DirectionalAbort`);
[Connections.StreamLimits](../assimalign-cohesion-connections-streamlimits/index.md) owns
`IMultiplexedConnectionStreamLimits` (`connection.StreamLimits`). See the
[design](design.md#connection-features).

A listener contains each connection's failure: what escapes `AcceptAsync` is the listener's own.
A layered listener (`listener.Use(layer)`) upgrades each accepted connection on its own task, at
most 512 at a time (`listener.Use(layer, maxConcurrentUpgrades)`). A failed or timed-out upgrade
closes only that connection, and the library's internal event source, `Assimalign.Cohesion.Connections`,
reports it. See the [design](design.md#a-listener-contains-each-connections-failure).

`ConnectionClosed` fires when a connection is closed or aborted. A stream of a multiplexed connection
also fires it when its peer abandons the stream (a QUIC `RESET_STREAM` or `STOP_SENDING`, or the
in-memory equivalents), so a consumer such as an HTTP/3 request learns of it without reading or
writing (#1329). See the [design](design.md#when-connectionclosed-fires).

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
`IMultiplexedStream` (from `Connection` to `MultiplexedStream` on the guided base), and coded
connection abort is now required. These break preview.1 implementers and existing binaries under
decisions 15 and 56; see [design](design.md).

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
| `ConnectionFeatureCollection` | `src/ConnectionFeatureCollection.cs` |
| `ConnectionFeatureCollectionExtensions` | `src/Extensions/ConnectionFeatureCollectionExtensions.cs` |
| `ConnectionListener` | `src/ConnectionListener.cs` |
| `ConnectionProtocol` | `src/ValueObjects/ConnectionProtocol.cs` |
| `ConnectionResetException` | `src/Exceptions/ConnectionResetException.cs` |
| `ConnectionSecurity` | `src/ValueObjects/ConnectionSecurity.cs` |
| `ConnectionState` | `src/ValueObjects/ConnectionState.cs` |
| `DatagramConnection` | `src/DatagramConnection.cs` |
| `DatagramReceiveResult` | `src/ValueObjects/DatagramReceiveResult.cs` |
| `DuplexPipeStream` | `src/DuplexPipeStream.cs` |
| `IConnectionFeature` | `src/Abstractions/IConnectionFeature.cs` |
| `IConnectionFeatureCollection` | `src/Abstractions/IConnectionFeatureCollection.cs` |
| `IMultiplexedConnection` | `src/Abstractions/IMultiplexedConnection.cs` |
| `IMultiplexedStream` | `src/Abstractions/IMultiplexedStream.cs` |
| `MultiplexedConnection` | `src/MultiplexedConnection.cs` |
| `MultiplexedStream` | `src/MultiplexedStream.cs` |

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

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Abstractions/IMultiplexedStream.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/MultiplexedStream.cs`.
