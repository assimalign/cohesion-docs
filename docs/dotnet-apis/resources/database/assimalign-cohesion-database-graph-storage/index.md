# Assimalign.Cohesion.Database.Graph.Storage

`Assimalign.Cohesion.Database.Graph.Storage` stores property-graph nodes, relationships, endpoint adjacency and secondary node-property indexes.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GraphStorage`](graph-storage.md)** — Documented public type.
- **[`GraphStore`](graph-store.md)** — Documented public type.
- **[`GraphIncidentDirection`](graph-incident-direction.md)** — Direction selection for streaming incident reads.
- **[IGraphStore migration](i-graph-store.md)** — Use the sealed GraphStore and concrete contexts.
- **[`StoredGraphNode`](stored-graph-node.md)** — Documented public type.
- **[`StoredGraphRelationship`](stored-graph-relationship.md)** — Documented public type.

`Assimalign.Cohesion.Database.Graph.Storage` stores property-graph nodes, relationships, endpoint
adjacency and secondary node-property indexes. It targets .NET 10, is AOT compatible, and uses
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

See [DESIGN.md](design.md) for the binary format, adjacency costs, numeric index semantics,
transaction lifecycle, recovery sequence and current limits.

Creation accepts complete StoredGraphNode/StoredGraphRelationship records with IDs reserved
from the backing storage. UpdateNodeAsync replaces labels/properties and UpdateRelationshipAsync
replaces properties. Format 3 retains snapshot-visible version chains and catalog type IDs.
Older records and tree registrations are refused with COHDBG017; recreate the database because
there is no upgrade path. CreateRelationshipAsync requires a nonempty typeId. Filtered incident
reads stream selected node/direction/type ranges; see [changed signatures](graph-store.md).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/Assimalign.Cohesion.Database.Graph.Storage.csproj`.
