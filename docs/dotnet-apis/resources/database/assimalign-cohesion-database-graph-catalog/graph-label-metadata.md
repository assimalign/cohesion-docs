# GraphLabelMetadata

The `GraphLabelMetadata` type is part of the documented `Assimalign.Cohesion.Database.Graph.Catalog` API.

> **Status:** Implemented.

A node-label definition and the authority that owns it.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

## Documented behavior

`GraphCatalog.Open` creates an `IGraphCatalog` over graph storage and its transaction coordinator.
`GraphLabelMetadata` and `GraphRelationshipTypeMetadata` carry stable GUIDs, case-sensitive names,
ownership, and owning schema. `GraphPropertyKeyMetadata` declares optional value type and
requiredness. `GraphIndexMetadata` names a node-property index.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Catalog`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Catalog/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Catalog/src/GraphLabelMetadata.cs`.
