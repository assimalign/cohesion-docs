# Assimalign.Cohesion.Database.Sql.Catalog

The relational catalog of the SQL model: schemas, tables, columns, and constraints (`SqlCatalogTable`/`SqlCatalogColumn`), persisted through the storage kernel so metadata gets the same page/WAL durability as data, plus persistence for the physical index registrations exported by `Database.Indexing`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

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

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/Assimalign.Cohesion.Database.Sql.Catalog.csproj`.
