# GraphDatabaseEngine

The `GraphDatabaseEngine` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Manages logical graph databases and their four maintenance workers.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphDatabaseEngine.Create(options)`** — creates an operational engine. Its lifecycle is defined by
  `IDatabaseEngine`; `Workers` exposes the four maintenance duties. `Dispose` the engine to flush
  and release all databases.
- **`GraphDatabaseEngine.CreateBuilder()`** — exposes dependency-free options and nested component
  factories for standalone construction or a hosting-aware engine factory.
- **`GraphDatabaseEngine.Servers`** — exposes nested servers whose lifetime the engine owns.
  `Create`/open/drop/lookup now accept `DatabaseName`, including its implicit string conversion.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngine.cs`.
