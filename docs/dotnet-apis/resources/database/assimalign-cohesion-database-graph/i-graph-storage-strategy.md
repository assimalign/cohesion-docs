# IGraphStorageStrategy

The `IGraphStorageStrategy` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Creates, reopens, discovers and drops storage for a graph engine.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`IGraphStorageStrategy`** — optionally supplies storage create/open/drop and discovery, overriding
  RootPath. The engine owns returned storage; the caller owns the strategy.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Abstractions/IGraphStorageStrategy.cs`.
