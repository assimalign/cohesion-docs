# Graph catalog

`Assimalign.Cohesion.Database.Graph.Catalog` provides durable discovery and definition metadata for one logical graph database.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`. Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

This catalog component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Graph`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](catalog-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/catalog.md)** — Source-backed usage and expected behavior.
- **[`GraphCatalog`](graph-catalog.md)** — Documented public type.
- **[`GraphCatalogException`](graph-catalog-exception.md)** — Documented public type.
- **[`GraphIndexMetadata`](graph-index-metadata.md)** — Documented public type.
- **[`GraphLabelMetadata`](graph-label-metadata.md)** — Documented public type.
- **[`GraphPropertyKeyMetadata`](graph-property-key-metadata.md)** — Documented public type.
- **[`GraphRelationshipTypeMetadata`](graph-relationship-type-metadata.md)** — Documented public type.

`Assimalign.Cohesion.Database.Graph.Catalog` provides durable discovery and definition metadata for
one logical graph database. It records node labels, relationship types, property keys, and named
node-property indexes. Labels and relationship types carry `DatabaseObjectOwner` and an optional
owning schema.

`Open` the catalog with `GraphCatalog.Open(storage, coordinator)` after the shared coordinator has
analyzed and scrubbed recovery. Reads take a `TransactionSnapshot`; writes take an active
`TransactionContext` and join that transaction without committing it. The engine holds the
definition lock and coordinates changes to graph data and physical indexes.

An initial definition can be marked schema-owned to exercise ownership enforcement. Subsequent
alteration, drop, property change, or index change throws `DatabaseObjectLockedException` naming the
definition, schema, and operation. Compiled graph schema provisioning remains a separate feature.

The implementation depends on the `Database` root for ownership contracts, `Graph.Storage` for the
file-set facade, and the shared `Storage`, Transactions, and Types packages. It has no reference to
the Graph engine, Hosting, or ApplicationModel. Physical B+Trees and their maintenance belong to
`Graph.Storage`. See [DESIGN.md](catalog-design.md) for the binary format, MVCC behavior, and division of
responsibilities.


[Parent: Assimalign.Cohesion.Database.Graph](index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
