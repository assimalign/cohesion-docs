# IGraphDatabaseEngineBuilder

The `IGraphDatabaseEngineBuilder` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Captures dependency-free options and deferred components for one graph engine.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`AddGraph((context, engine) => ...)`** — is an extension member on the root
  `IDatabaseApplicationBuilder`; it captures construction and returns the application builder.
  Its `Build`-time callback receives `IGraphDatabaseEngineBuilder` with model options and
  deferred `AddWorker` / `AddServer` factories. Failed construction disposes completed products.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Abstractions/IGraphDatabaseEngineBuilder.cs`.
