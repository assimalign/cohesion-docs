# Graph storage

`Assimalign.Cohesion.Database.Graph.Storage` stores property-graph nodes, relationships, endpoint adjacency, MVCC label postings and secondary node-property indexes.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`. Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Graph`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](storage-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/storage.md)** — Source-backed usage and expected behavior.
- **[`GraphStorage`](graph-storage.md)** — Documented public type.
- **[`GraphStore`](graph-store.md)** — Documented public type.
- **[`GraphIncidentDirection`](graph-incident-direction.md)** — Direction selection for streaming incident reads.
- **[IGraphStore migration](i-graph-store.md)** — Use the sealed GraphStore and concrete contexts.
- **[`StoredGraphNode`](stored-graph-node.md)** — Documented public type.
- **[`StoredGraphRelationship`](stored-graph-relationship.md)** — Documented public type.

`Assimalign.Cohesion.Database.Graph.Storage` stores property-graph nodes, relationships, endpoint
adjacency, MVCC label postings and secondary node-property indexes. It targets .NET 10, is AOT compatible, and uses
explicit binary codecs without reflection.

`GraphStorage` owns the data, journal and backup streams and delegates page allocation, checksums
and physical recovery to `Database.Storage`. Construct a `TransactionCoordinator` from its
`WriteAheadJournal` and `Records`, then use `GraphStore.Open` to obtain a sealed `GraphStore`. Every
mutation takes the caller's `TransactionContext` and leaves commit or rollback to that caller.
Reads take an immutable `TransactionSnapshot`.

The store exposes creation, versioned node/relationship updates, lookup, restricted or cascading deletion, incident-edge lookup, and
exact node-property index creation/search/drop. An incident-edge lookup is a B+Tree range seek and
does not scan every relationship. Property index searches return scalar-verified candidates in
identity order. The graph root maps storage DTO identities to the frozen `GraphNodeId` and
`GraphRelationshipId` contracts and plans traversals.

Dependencies are `Database.Storage`, `Database.Transactions` and `Database.Indexing`. There is no
dependency on the Graph root, its catalog, Hosting or ApplicationModel. Owner-zero pages remain
available to `Graph.Catalog` through the low-level record seam.

See [DESIGN.md](storage-design.md) for the binary format, adjacency costs, numeric index semantics,
transaction lifecycle, recovery sequence and current limits.

Creation accepts complete StoredGraphNode/StoredGraphRelationship records with IDs reserved
from the backing storage. UpdateNodeAsync replaces labels/properties and UpdateRelationshipAsync
replaces properties. Both node methods require a caller-resolved label-name/catalog-GUID map.
Label reads seek MVCC postings and decode only matching visible nodes. Format 4 persists catalog
label GUIDs, label postings, snapshot-visible version chains and catalog type IDs.
Older records and tree registrations are refused with COHDBG017; recreate the database because
there is no upgrade path. CreateRelationshipAsync requires a nonempty typeId. Filtered incident
reads stream selected node/direction/type ranges; see [changed signatures](graph-store.md).

At the engine boundary, an open-time graph-format refusal becomes
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md)
with COHDBS003, COHDBG018 and COHDBG017 retained in Diagnostics; no SQLSTATE is assigned.


[Parent: Assimalign.Cohesion.Database.Graph](index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
