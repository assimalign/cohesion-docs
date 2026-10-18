# Assimalign.Cohesion.Database.Sql.Catalog

The relational catalog of the SQL model: schemas, tables, columns, and constraints (`SqlCatalogTable`/`SqlCatalogColumn`), persisted through the storage kernel so metadata gets the same page/WAL durability as data, plus persistence for the physical index registrations exported by `Database.Indexing`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[SqlCatalog](sql-catalog.md)** — Durable identity allocation without changing DDL generation.
- **[SqlCatalogIdentity](sql-catalog-identity.md)** — Immutable mode, start and increment.
- **[SqlCatalogColumn](sql-catalog-column.md)** — Column metadata and identity facet.

The relational catalog of the SQL model: schemas, tables, columns, and constraints
(`SqlCatalogTable`/`SqlCatalogColumn`), persisted through the storage kernel so metadata gets the
same page/WAL durability as data, plus persistence for the physical index registrations exported by
`Database.Indexing`.

## Scope

- **Metadata model** — table descriptions with stable `ObjectId`s, ordered columns
  typed by the shared type system (`DatabaseTypeInfo`), nullability, default
  literals, and primary-key constraints.
- **Transactional DDL** — create/drop table, add/drop column; each operation is a
  self-committing storage transaction (durable when the call returns).
- **`Index` registration persistence** — `SaveIndexRegistrationsAsync` /
  `GetIndexRegistrations` store the `BTreeIndexRegistration` set so indexes
  re-attach when the database reopens.
- **Applied-schema state** — `SchemaState` / `SaveSchemaStateAsync` atomically
  retain the canonical `CompiledSchema` document and deterministic hash used for
  idempotent provisioning and live-catalog reconciliation.

## Dependencies

`Database` (root), `Database.Storage`, `Database.Sql.Storage` (the record facade its records
persist through), `Database.Types` (type identities + the record codec), `Database.Indexing`
(registration types). Deliberately language-free: translating parsed DDL
(`SqlCreateTableExpression`) into catalog calls is the planner's job.

## Usage

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the persistence model and decisions.

The catalog supports [atomic table/column rename](sql-catalog.md#rename-members) and a separate
[frozen missing-value literal](sql-catalog-column.md) for ADD COLUMN history.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Current format boundary

The integration branch uses SQL row-storage format 18 and compiled schema version 7.
Format 17 introduced the insert-default/frozen-missing-value split (table extension 6); format 18
separately changes quoting for newly executable function names in persisted canonical SQL.
The formats preserve resolved shared type identity/modifiers across contextual multi-word and
SQL Server lexical aliases. Typed temporal, interval and binary defaults use canonical literal
text. These unreleased formats have no older-format compatibility reader; use this current
boundary when interpreting historical layout notes above.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/Assimalign.Cohesion.Database.Sql.Catalog.csproj`.
