# GraphStorage

The `GraphStorage` type is part of the documented `Assimalign.Cohesion.Database.Graph.Storage` API.

> **Status:** Partial.

Owns graph pages and delegates journaling, allocation, and recovery to the shared kernel.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

## Documented behavior

`GraphStorage.Create` and `Open` own three streams and derive from the shared Storage base and expose its surface,
`WriteAheadJournal`, and `Records` for coordinator construction. Their raw owner-zero record
methods support Graph.Catalog. `PackLocation` and `UnpackLocation` convert the stable page/slot
reference format.

`GraphStore.Open(GraphStorage, TransactionCoordinator)` returns the sealed `GraphStore`, whose operations
create/find/update/delete nodes and relationships, enumerate nodes and incident relationships,
create/drop/search exact property indexes, and recover index writers. Mutations require an active
caller transaction and do not commit it. Snapshot reads return immutable `StoredGraphNode` and
`StoredGraphRelationship` scalar records.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Storage`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/docs/Assembly/Assimalign.Cohesion.Database.Graph.Storage/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/GraphStorage.cs`.
