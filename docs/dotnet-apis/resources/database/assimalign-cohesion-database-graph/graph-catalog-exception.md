# GraphCatalogException

The `GraphCatalogException` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Implemented.

Reports invalid graph metadata or a conflicting catalog definition.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

`Read` methods accept a visibility snapshot. Save and delete methods join an active logical
transaction and require the caller to serialize definition changes. Saving a new marked schema
definition is supported; changing it later throws `DatabaseObjectLockedException`. Malformed
persisted records and conflicting definitions throw `GraphCatalogException`.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/Exceptions/GraphCatalogException.cs`.
