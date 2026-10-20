# Sql catalog

The relational catalog of the SQL model: schemas, tables, columns, and constraints (`SqlCatalogTable`/`SqlCatalogColumn`), persisted through the storage kernel so metadata gets the same page/WAL durability as data, plus persistence for the physical index registrations exported by `Database.Indexing`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`. Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

This catalog component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Sql`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[SqlCatalogNamespace](sql-catalog-namespace.md)** — Persisted namespace and compiled ownership.
- **[SqlCatalogSnapshot](sql-catalog-snapshot.md)** — Atomic immutable capture, including empty namespaces.

- **[Design](catalog-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/catalog.md)** — Source-backed usage and expected behavior.
- **[SqlCatalog](sql-catalog.md)** — Durable identity allocation without changing DDL generation.
- **[SqlCatalogIdentity](sql-catalog-identity.md)** — Immutable mode, start and increment.
- **[SqlCatalogConstraint](sql-catalog-constraint.md)** — Persisted references/checks and five delete actions.
- **[SqlCatalogColumn](sql-catalog-column.md)** — Column metadata and identity facet.

The relational catalog of the SQL model: schemas, tables, columns, and constraints
(`SqlCatalogTable`/`SqlCatalogColumn`), persisted through the storage kernel so metadata gets the
same page/WAL durability as data, plus persistence for the physical index registrations exported by
`Database.Indexing`.

[SqlCatalogConstraint](sql-catalog-constraint.md) persists five delete actions. Its constructor
retains a Restrict default; SQL syntax defaults to NoAction. Compiled schema version 7 still
models only Restrict/Cascade.

- **[SqlCatalogView](sql-catalog-view.md)** — Canonical view body, frozen columns and resolved dependencies.

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

See the [source-backed usage examples](examples/catalog.md).

See [DESIGN.md](catalog-design.md) for the persistence model and decisions.

The catalog supports [atomic table/column rename](sql-catalog.md#rename-members) and a separate
[frozen missing-value literal](sql-catalog-column.md) for ADD COLUMN history.


[Parent: Assimalign.Cohesion.Database.Sql](index.md)

## Current format boundary

The integration branch uses SQL row-storage format 25 and compiled schema version 7.
Format 17 introduced the insert-default/frozen-missing-value split (table extension 6); format 18
separately changes quoting for newly executable function names in persisted canonical SQL.
The formats preserve resolved shared type identity/modifiers across contextual multi-word and
SQL Server lexical aliases. Typed temporal, interval and binary defaults use canonical literal
text. These unreleased formats have no older-format compatibility reader; use this current
boundary when interpreting historical layout notes above.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Current format 25 adds distinct predicates, Boolean tests and LIKE ESCAPE.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
