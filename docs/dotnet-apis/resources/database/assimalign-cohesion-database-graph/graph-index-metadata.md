# GraphIndexMetadata

The `GraphIndexMetadata` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Implemented.

A named nonunique secondary node-property index definition.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

`GraphCatalog.Open` creates a `GraphCatalog` over graph storage and its transaction coordinator.
`GraphLabelMetadata` and `GraphRelationshipTypeMetadata` carry stable GUIDs, case-sensitive names,
ownership, and owning schema. `GraphPropertyKeyMetadata` declares optional value type and
requiredness. `GraphIndexMetadata` names a node-property index.

The complete UTF-8 metadata record must fit 8,092 bytes, including index and property names.
Overflow is GraphElementTooLargeException at the catalog and structured CG02009 through
GraphSchema/engine operations; exact fit is accepted. Saving an existing index name for another
property is a deterministic CG03003 engine error. See [catalog boundaries](graph-catalog-exception.md).

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/ValueObjects/GraphIndexMetadata.cs`.
