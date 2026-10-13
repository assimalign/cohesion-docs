# Assimalign.Cohesion.Connections.StreamLimits

Read the cumulative inbound bidirectional stream allowance of a multiplexed transport. The package owns `IMultiplexedConnectionStreamLimits` and the `StreamLimits` extension property on `IMultiplexedConnection` in the `Assimalign.Cohesion.Connections` namespace.

`IMultiplexedConnection` inherits the common `IConnection` lifecycle and features.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Usage

Reference this package, import `Assimalign.Cohesion.Connections`, and read `connection.StreamLimits`. The property returns the registered feature or `null` when the connection does not offer it. Cache the result when using it repeatedly on a stream or connection.

Drivers register an implementation with `features.Set(feature)` in `ConfigureFeatures`. Its `Name` returns `IMultiplexedConnectionStreamLimits.FeatureName`, pinned to "StreamLimits". Layers return the wrapped connection's collection from `InnerFeatures` to pass the feature through, or replace or remove that named slot.

`InboundBidirectionalStreamLimit` is a nonnegative, nondecreasing conservative upper bound on the number of bidirectional streams the peer may have been allowed to initiate over the connection lifetime. It includes credit whose release has begun but may not yet have been advertised. It is not a concurrent-stream count or permission to open an outbound stream. The multiplexed root migration (#1453) retains this optional feature: QUIC and WebTransport have cumulative allowances; HTTP/2's concurrent-stream limit is a different quantity.

## Dependencies

The package references only `Assimalign.Cohesion.Connections`, which owns the feature collection and connection contracts. See [design](design.md) for registration, layering and compatibility.

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Connections`](../assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IMultiplexedConnectionStreamLimits` | `src/Abstractions/IMultiplexedConnectionStreamLimits.cs` |
| `ConnectionStreamLimitsExtensions` | `src/Extensions/ConnectionStreamLimitsExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/src/Assimalign.Cohesion.Connections.StreamLimits.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/src/Abstractions/IMultiplexedConnectionStreamLimits.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/src/Extensions/ConnectionStreamLimitsExtensions.cs`.
