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

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/ValueObjects/GraphPropertyKeyMetadata.cs`.
