# Assimalign.Cohesion.Database.Graph

`Assimalign.Cohesion.Database.Graph` is the embedded property-graph engine.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GraphDatabaseEngine`](graph-database-engine.md)** — Documented public type.
- **[`GraphNode`](graph-node.md)** — Documented public type.
- **[`GraphQueryRequest`](graph-query-request.md)** — Documented public type.
- **[`GraphRelationship`](graph-relationship.md)** — Documented public type.
- **[`GraphSchema`](graph-schema.md)** — Documented public type.
- **[`GraphTraversal`](graph-traversal.md)** — Documented public type.
- **[`IGraphDatabase`](i-graph-database.md)** — Documented public type.
- **[`IGraphDatabaseEngineBuilder`](i-graph-database-engine-builder.md)** — Documented public type.
- **[`IGraphSchema`](i-graph-schema.md)** — Documented public type.
- **[`IGraphStorageStrategy`](i-graph-storage-strategy.md)** — Documented public type.

`Assimalign.Cohesion.Database.Graph` is the embedded property-graph engine. It creates, opens,
enumerates and drops logical databases, opens database-bound sessions, and implements the frozen
`IGraphDatabase` node, relationship and traversal operations. A session executes the bounded GQL
subset described in [Graph.Language](../assimalign-cohesion-database-graph-language/design.md) .

See the [source-backed usage examples](examples/index.md).

`GraphSchema.Open` supplies session-bound label/type discovery, property metadata, ownership
enforcement and node-property index creation. `AddGraph((context, engine) => ...)` captures
construction through `IDatabaseApplicationBuilder` and returns that builder. The callback runs at
`Build` with `IGraphDatabaseEngineBuilder`, whose options include an optional borrowed
`IGraphStorageStrategy`. Workers and servers register as nested factories; the built engine owns
their products. Creating the engine starts its four built-in maintenance workers; application Start
starts the nested servers.

The engine references the area root and Graph.Language, Graph.Catalog and `Graph.Storage`. The storage
and catalog compose the shared kernel. It is `net10.0`, AOT compatible, and uses neither reflection
nor `Microsoft.Extensions.*`. There is no Graph wire client, security policy integration,
replication or compiled-schema provisioning in this phase. See [DESIGN.md](design.md) for lifecycle,
isolation, traversal bounds and the disk format.

`GraphDatabaseEngine.CreateBuilder()` returns the same model builder for standalone composition or
the concrete hosting builder's build-aware engine factory. This lets the consumer pass already
resolved values and register nested components while keeping the model package dependency-free.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Graph.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Graph.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Graph.Catalog` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
