# Sql storage

The SQL model's storage facade: `SqlStorage` is the concrete `Storage` implementation (model `StorageModel.Sql`) the SQL engine composes per database file set.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.Sql.Storage`. Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Sql`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](storage-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/storage.md)** — Source-backed usage and expected behavior.

The SQL model's storage facade: `SqlStorage` is the concrete `Storage` implementation (model
`StorageModel.Sql`) the SQL engine composes per database file set. It exposes row-grain operations
— insert/read/update/delete by `(PageId, SlotIndex)` — over the shared page/WAL substrate from
`Assimalign.Cohesion.Database.Storage`, plus the factory methods (`Create`/`Open`) the engine's
storage strategies call.

- **Consumed by:** `Assimalign.Cohesion.Database.Sql` (the engine, per database:
  one data file set + one `.catalog` file set) and `Assimalign.Cohesion.Database.Sql.Catalog`
  (metadata records on the catalog file set).
- **Depends on:** `Assimalign.Cohesion.Database.Storage` only.
- **Docs:** design record in [DESIGN.md](storage-design.md); the substrate's invariants
  (WAL ordering, recovery, buffer pool, per-owner record chains) are documented in
  the `Storage` project's `docs/DESIGN.md`.


[Parent: Assimalign.Cohesion.Database.Sql](index.md)

## Format boundary

The engine opens SQL data/catalog format 27 only, including table extension 7's TYPE default
assignment flag. Format 26 and every earlier format are refused before expression rebinding;
compiled schema documents remain v8. Shared physical storage format 4 and
B-tree page format 2 are independent. Follow the [export/recreate and rebuild procedure](../../../../database/sql/index.md#sql-format-27-and-schema-v8);
there is no upgrade reader.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
