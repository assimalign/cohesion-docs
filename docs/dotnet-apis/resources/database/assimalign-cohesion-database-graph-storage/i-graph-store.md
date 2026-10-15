# IGraphStore

The `IGraphStore` type is part of the documented `Assimalign.Cohesion.Database.Graph.Storage` API.

> **Status:** Partial.

Transactional graph records, adjacency, and exact node-property indexes.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

## Documented behavior

`GraphStore.Open(GraphStorage, TransactionCoordinator)` returns `IGraphStore`, whose operations
create/find/delete nodes and relationships, enumerate nodes and incident relationships,
create/drop/search exact property indexes, and recover index writers. Mutations require an active
caller transaction and do not commit it. Snapshot reads return immutable `StoredGraphNode` and
`StoredGraphRelationship` scalar records.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Storage`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/docs/Assembly/Assimalign.Cohesion.Database.Graph.Storage/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/IGraphStore.cs`.
