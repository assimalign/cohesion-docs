# Assimalign.Cohesion.Database.Graph

`Assimalign.Cohesion.Database.Graph` is the embedded property-graph engine.

> **Status:** Partial.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

Catalog and Storage ship in this assembly with their existing namespaces. The former satellite
packages are retired. Wire contracts ship separately in [Graph.Protocol](../assimalign-cohesion-database-graph-protocol/index.md).

## Reference pages

- **[GraphDatabaseServerOptions](graph-database-server-options.md)** — Terminal drain deadline and peer-close linger.

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GraphDatabaseEngine`](graph-database-engine.md)** — Documented public type.
- **[`GraphNode`](../assimalign-cohesion-database-graph-protocol/graph-node.md)** — Documented public type.
- **[`GraphQueryRequest`](graph-query-request.md)** — Documented public type.
- **[`GraphRelationship`](../assimalign-cohesion-database-graph-protocol/graph-relationship.md)** — Documented public type.
- **[`GraphSchema`](graph-schema.md)** — Documented public type.
- **[`GraphTraversal`](graph-traversal.md)** — Documented public type.
- **[`GraphDatabase`](graph-database.md)** — Documented public type.
- **[`GraphDatabaseEngineBuilder`](graph-database-engine-builder.md)** — Documented public type.
- **[GraphValueCodec](../assimalign-cohesion-database-graph-protocol/graph-value-codec.md)** — Bounded scalar/list/map wire components.
- **[GraphProtocol](../assimalign-cohesion-database-graph-protocol/graph-protocol.md)** — Protocol 2.1 graph.composite-values capability.
- **[GraphProtocolExecuteMessage](../assimalign-cohesion-database-graph-protocol/graph-protocol-execute-message.md)** — Text and encoded parameters.
- **[GraphProtocolResultHeaderMessage](../assimalign-cohesion-database-graph-protocol/graph-protocol-result-header-message.md)** — Column type and composite flag.
- **[GraphRequest](graph-request.md)** — Public execution contract.
- **[GraphPathsQueryRequest](graph-paths-query-request.md)** — Public execution contract.

`Assimalign.Cohesion.Database.Graph` is the embedded property-graph engine. It creates, opens,
enumerates and drops logical databases, opens database-bound sessions, and implements the frozen
`GraphDatabase` node, relationship and traversal operations. A session executes the bounded GQL
subset described in [Graph.Language](../assimalign-cohesion-database-graph-language/design.md) .

See the [source-backed usage examples](examples/index.md).

`GraphSchema.Open` supplies session-bound label/type discovery, property metadata, ownership
enforcement and node-property index creation. `AddGraph(name, graph => ...)` captures
construction through `IDatabaseApplicationBuilder` and returns that builder. The callback runs at
`Build` with `GraphDatabaseEngineBuilder`. Its Options.RootPath selects file-backed storage;
omitting it uses memory. Workers and servers register as nested factories; the built engine owns
their products. Creating the engine starts its four built-in maintenance workers; application Start
starts the nested servers.

The engine references the area root, Graph.Language and Graph.Protocol. Its Storage and Catalog
components compose the shared kernel. It is `net10.0`, AOT compatible, and uses neither
reflection nor `Microsoft.Extensions.*`. `GraphDatabaseServer` dispatches scalar GQL, mutations,
catalog `SHOW`, and path queries to the same engine. The separate NuGet-only
[Graph.Client](../assimalign-cohesion-database-graph-client/index.md) package supplies pooled
scalar execution and path streaming. Security policy integration, replication and compiled-schema
provisioning remain outside this phase. See [DESIGN.md](design.md) for lifecycle, isolation,
traversal bounds and the disk format.

For real path results, pass `GraphPathsQueryRequest.FromGql(...)` to the existing session
`ExecuteAsync` method and read `GraphPathsQueryResult.Paths`. A single projected node becomes a
one-node path, a single relationship includes its stored endpoints, and
`MATCH p = (a)-[r:KNOWS]->(b) RETURN p` preserves the matched traversal order. Scalar and mutation
requests continue to use ordinary execution. Explicit transactions remain available through the
in-process session API and standalone START TRANSACTION, COMMIT and ROLLBACK over Execute.

`GraphDatabaseEngine.CreateBuilder(name)` returns the same model builder for standalone composition or
the concrete hosting builder's build-aware engine factory. This lets the consumer pass already
resolved values and register nested components while keeping the model package dependency-free.

`GraphDatabaseServerOptions.Listener` takes an `IDuplexConnectionListener`, including TCP and InMemory listeners. The server owns accepted duplex connections and their protocol I/O.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Graph.Protocol` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database.Graph.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Execution` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Graph.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Additional reference pages

- **[Graph catalog design](catalog-design.md)** — Reference and usage.
- **[Graph catalog](catalog.md)** — Reference and usage.
- **[GraphCatalogException](graph-catalog-exception.md)** — Reference and usage.
- **[GraphCatalog](graph-catalog.md)** — Reference and usage.
- **[GraphIncidentDirection](graph-incident-direction.md)** — Reference and usage.
- **[GraphIndexMetadata](graph-index-metadata.md)** — Reference and usage.
- **[GraphLabelMetadata](graph-label-metadata.md)** — Reference and usage.
- **[GraphPropertyKeyMetadata](graph-property-key-metadata.md)** — Reference and usage.
- **[GraphRelationshipTypeMetadata](graph-relationship-type-metadata.md)** — Reference and usage.
- **[GraphStorage](graph-storage.md)** — Reference and usage.
- **[GraphStore](graph-store.md)** — Reference and usage.
- **[IGraphStorageStrategy (removed)](i-graph-storage-strategy.md)** — Reference and usage.
- **[IGraphStore migration](i-graph-store.md)** — Reference and usage.
- **[Graph storage design](storage-design.md)** — Reference and usage.
- **[Graph storage](storage.md)** — Reference and usage.
- **[StoredGraphNode](stored-graph-node.md)** — Reference and usage.
- **[StoredGraphRelationship](stored-graph-relationship.md)** — Reference and usage.

## Execution budgets

[Engine options](graph-database-engine-options.md) configure memory/work ceilings; [GraphQueryLimits](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md) lowers embedded request limits.

## Catalog diagnostics

[GraphCatalogException](graph-catalog-exception.md) exposes non-null structured Diagnostics.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
