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

Direct catalog calls retain this exception. Engine and GraphSchema operations translate it by
cause: snapshot-changed metadata uses DatabaseTransactionAbortedException with COHDBG007;
deterministic identity reuse/rename, name/identity mismatch, index-property retargeting or missing
owner uses DatabaseException with COHDBG003; malformed records use COHDBG006. The original
catalog exception remains the cause. These codes have no assigned SQLSTATE. Storage/index/I/O
failure takes precedence when it accompanies a conflict or disposal.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/Exceptions/GraphCatalogException.cs`.
