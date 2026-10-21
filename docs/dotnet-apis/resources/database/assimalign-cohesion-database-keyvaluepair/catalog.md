# KeyValuePair catalog

The key-value model's metadata catalog: `IKeyValueCatalog`, opened over a database's dedicated catalog storage file set (`KeyValueCatalog.Open`).

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Catalog`. Assembly: `Assimalign.Cohesion.Database.KeyValuePair.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair`.

This catalog component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.KeyValuePair`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](catalog-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/catalog.md)** — Source-backed usage and expected behavior.

The key-value model's metadata catalog: `IKeyValueCatalog`, opened over a database's dedicated
catalog storage file set (`KeyValueCatalog.Open`).

## Purpose

Persists exactly what re-attaches a key-value database on open:

- **`Index` registrations** — the physical identity (object id, definition, root
  page id) of the database's primary key index, exported by the index manager and
  re-persisted at the engine's persistence points (root page ids drift on splits).
- **`Entry`-space format version** — the record-layout version marker; entry records
  are not self-describing across format changes, so the engine reads this at open
  and rejects formats newer than it understands.

## Scope

Deliberately minimal — the key-value model has no schemas, tables, columns, or constraints beyond
key uniqueness (enforced by the primary index itself, not described here). Deferred model features
with catalog surface area when they land: named key spaces, per-entry expiration (TTL) metadata.

## Dependencies

- **`Assimalign.Cohesion.Database`** — the area root (`DatabaseException` ancestry).
- **`Assimalign.Cohesion.Database.Storage` / `.KeyValuePair.Storage`** — the catalog
  file set the records persist on.
- **`Assimalign.Cohesion.Database.Types`** — the shared tuple codec records encode with.
- **`Assimalign.Cohesion.Database.Indexing`** — `BTreeIndexRegistration`.

## Usage

The engine (`Assimalign.Cohesion.Database.KeyValuePair`) opens one catalog per database over the
`<name>.catalog` file set; consumers never compose it directly.


[Parent: Assimalign.Cohesion.Database.KeyValuePair](index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Assimalign.Cohesion.Database.KeyValuePair.csproj`.
