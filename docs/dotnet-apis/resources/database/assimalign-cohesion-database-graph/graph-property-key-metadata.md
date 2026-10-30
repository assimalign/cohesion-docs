# GraphPropertyKeyMetadata

The `GraphPropertyKeyMetadata` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Implemented.

A property key declared by a label or relationship type.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

`GraphCatalog.Open` creates a `GraphCatalog` over graph storage and its transaction coordinator.
`GraphLabelMetadata` and `GraphRelationshipTypeMetadata` carry stable GUIDs, case-sensitive names,
ownership, and owning schema. `GraphPropertyKeyMetadata` declares optional value type and
requiredness. `GraphIndexMetadata` names a node-property index.

GraphSchema.GetPropertyKeysAsync returns declared property metadata only. Named
[type and existence constraints](graph-constraint-metadata.md) remain independent definitions;
reading and saving property metadata does not copy their effects into an unnamed rule.
[SHOW PROPERTY KEYS](../../../../database/graph/language/statements/show.md) reports the effective
type/requiredness after combining declarations and named rules. Incompatible declared/named type
rules use DatabaseException/CG03009 (no assigned SQLSTATE). Dropping a named rule preserves an
identical unnamed declaration.

The complete UTF-8 metadata record must fit 8,092 bytes, including its property name, header and
type facets. Overflow is GraphElementTooLargeException at the catalog and structured CG02009
through GraphSchema/engine operations; exact fit is accepted. See [metadata limits](catalog-design.md#on-disk-format).

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/ValueObjects/GraphPropertyKeyMetadata.cs`.
