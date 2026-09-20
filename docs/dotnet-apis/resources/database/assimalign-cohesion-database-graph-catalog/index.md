# Assimalign.Cohesion.Database.Graph.Catalog

`Assimalign.Cohesion.Database.Graph.Catalog` provides durable discovery and definition metadata for one logical graph database.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GraphCatalog`](graph-catalog.md)** — Documented public type.
- **[`GraphCatalogException`](graph-catalog-exception.md)** — Documented public type.
- **[`GraphIndexMetadata`](graph-index-metadata.md)** — Documented public type.
- **[`GraphLabelMetadata`](graph-label-metadata.md)** — Documented public type.
- **[`GraphPropertyKeyMetadata`](graph-property-key-metadata.md)** — Documented public type.
- **[`GraphRelationshipTypeMetadata`](graph-relationship-type-metadata.md)** — Documented public type.
- **[`IGraphCatalog`](i-graph-catalog.md)** — Documented public type.

`Assimalign.Cohesion.Database.Graph.Catalog` provides durable discovery and definition metadata for
one logical graph database. It records node labels, relationship types, property keys, and named
node-property indexes. Labels and relationship types carry `DatabaseObjectOwner` and an optional
owning schema.

`Open` the catalog with `GraphCatalog.Open(storage, coordinator)` after the shared coordinator has
analyzed and scrubbed recovery. Reads take a `TransactionSnapshot`; writes take an active
`ITransactionContext` and join that transaction without committing it. The engine holds the
definition lock and coordinates changes to graph data and physical indexes.

An initial definition can be marked schema-owned to exercise ownership enforcement. Subsequent
alteration, drop, property change, or index change throws `DatabaseObjectLockedException` naming the
definition, schema, and operation. Compiled graph schema provisioning remains a separate feature.

The implementation depends on the `Database` root for ownership contracts, `Graph.Storage` for the
file-set facade, and the shared `Storage`, Transactions, and Types packages. It has no reference to
the Graph engine, Hosting, or ApplicationModel. Physical B+Trees and their maintenance belong to
`Graph.Storage`. See [DESIGN.md](design.md) for the binary format, MVCC behavior, and division of
responsibilities.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Graph.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Catalog/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Catalog/src/Assimalign.Cohesion.Database.Graph.Catalog.csproj`.
