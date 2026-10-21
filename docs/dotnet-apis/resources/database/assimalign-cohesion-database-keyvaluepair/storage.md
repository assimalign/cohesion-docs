# KeyValuePair storage

The key-value model's storage binding: `KeyValueStorage`, the model-typed storage file set (`StorageModel.KeyValue`) the key-value engine composes over the shared `Database.Storage` kernel.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Storage`. Assembly: `Assimalign.Cohesion.Database.KeyValuePair.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.KeyValuePair`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](storage-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/storage.md)** — Source-backed usage and expected behavior.

The key-value model's storage binding: `KeyValueStorage`, the model-typed storage file set
(`StorageModel.KeyValue`) the key-value engine composes over the shared `Database.Storage` kernel.

## Purpose

`KeyValueStorage` derives from the kernel's `Storage` base and exposes exactly the record surface
the key-value engine needs — entry-record insert/read/update/delete on the transactional page
substrate, with owner-chain inserts keyed by the engine's key-space object id (per-object page
locality). No physical machinery lives here: pages, buffer pool, free-space map, WAL, and recovery
are the kernel's, unchanged.

## Scope

- **`KeyValueStorage`** — factories (`Create`/`Open`, with the `checkpointOnOpen: false`
  open the engine's recovery analysis requires) and the entry-record operations.
- **Nothing else. `Entry` encoding** — (version stamps + key/value tuple) belongs to the
  engine (`Assimalign.Cohesion.Database.KeyValuePair`), mirroring the SQL family
  split where `SqlRowCodec` lives in `Database.Sql`, not `Sql.Storage`.

## Dependencies

- **`Assimalign.Cohesion.Database.Storage`** — the shared physical kernel (pages, WAL,
  recovery, transactional record surface).

## Usage

The engine's storage strategies (`Assimalign.Cohesion.Database.KeyValuePair`'s in-memory and
file-system strategies) create and open instances; consumers never compose this type directly.


[Parent: Assimalign.Cohesion.Database.KeyValuePair](index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Assimalign.Cohesion.Database.KeyValuePair.csproj`.
