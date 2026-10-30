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

The engine opens SQL data/catalog format 31 only, including table extension 8's ON UPDATE metadata,
retained TYPE default-assignment flag and sequence definitions/reservations (kinds 18/19). Kind 18 includes owned-sequence
table/column linkage and identity ownership; kind 16 is retired.
Format 30 and every earlier format are refused before expression rebinding;
compiled schema documents use v10. Shared physical storage format 4 and
B-tree page format 2 are independent. Follow the [export/recreate and rebuild procedure](../../../../database/sql/index.md#sql-format-31-and-schema-v10);
there is no upgrade reader.

Identity and standalone sequences share [durable reservations](storage-design.md#identity-and-sequence-reservations)
of at most 32 values, committed before delivery. Recovery skips unused reserved values and data
rollback leaves gaps. Sequence catalog types and entry points are internal; no SQL syntax exists yet.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
