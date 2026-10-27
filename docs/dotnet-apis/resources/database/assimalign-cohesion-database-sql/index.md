# Assimalign.Cohesion.Database.Sql

The SQL engine manages databases and executes SQL over shared storage, catalog, and transaction kernels.

> **Status:** Partial.

Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

Catalog and Storage ship in this assembly with their existing namespaces. The former satellite
packages are retired. Wire contracts ship separately in [Sql.Protocol](../assimalign-cohesion-database-sql-protocol/index.md).

## Reference pages

- **[SqlDatabase](sql-database.md)** — Recoverable table rewrite, ordinal compaction and snapshot/retry behavior.
- **[SqlObjectName](sql-object-name.md)** — Validated decoded schema/object name components.
- **[SqlDatabaseServerOptions](sql-database-server-options.md)** — Terminal drain deadline and peer-close linger.

- **[SqlDatabaseEngineOptions](sql-database-engine-options.md)** — Statement and per-lock execution budgets.
- **[SqlDatabaseSession](sql-database-session.md)** — Nullable timeout overrides, execution and retry behavior.
- **[SqlTimeoutException](sql-timeout-exception.md)** — CS05043/57014 and CS04044/55P03.

- **[SqlFunctionCollection](sql-function-collection.md)** — Registration, reserved T1 names and freezing.
- **[SqlFunctionContext](sql-function-context.md)** — Database, collation, transaction timestamp and cancellation context.
- **[SqlConstraintViolationException](sql-constraint-violation-exception.md)** — Named integrity failures and their single coded diagnostic.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The SQL model engine of the Cohesion Data Platform: `SqlDatabaseEngine` manages database lifecycles;
sessions execute real SQL through the planner (`Internal/SqlPlanner`) and plan executor
(`Internal/SqlPlanExecutor`) against shared storage, with DDL flowing through the relational catalog
(`Database.Sql.Catalog`) and transactions riding the storage write-ahead log.

## Scope

- **Engine lifecycle** — create/open/drop/enumerate databases over a storage
  strategy (file-backed or in-memory); each database owns a data file set and a
  dedicated catalog file set.
- **Sessions and transactions** — explicit transactions map to storage
  transactions (durable commit, page-image rollback); statements outside a
  transaction auto-commit.
- **`Database` scope (A5)** — every session stays bound to the database that
  created it. Qualified table references resolve only within that database's
  catalog; SQL cannot switch databases or manage the server. Conformance tests
  keep identically named tables in two databases isolated and reject attempts
  to select another database or create/drop databases through a session.
- **Compiled-schema provisioning** — `ISqlDatabase` diffs a validated
  `CompiledSchema`, renders deterministic table/column/index DDL into parsed
  `SqlQueryRequest`s, compensates completed reversible steps on failure, and
  records the canonical document/hash only after live-catalog convergence.
- **SQL execution** — the declared dialect (`Database.Sql.Language/docs/DIALECT.md`)
  planned rule-based and executed through scans or eligible index seeks: `SELECT` with `WHERE`,
  projection, N-way INNER/LEFT/RIGHT/FULL/CROSS/comma joins with ON/USING, grouping and aggregates,
  `ORDER BY`, `LIMIT/OFFSET`, `DISTINCT`;
  `INSERT` (multi-row, defaults, nullability and [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md));
  `UPDATE`/`DELETE` with accurate affected counts and DML RETURNING; `CREATE/ALTER/DROP TABLE`.
  NULL-safe distinct predicates, Boolean tests and LIKE ESCAPE follow PostgreSQL precedence.
  Multi-row UPDATE foreign keys validate final replacement rows. Unsupported dialect features fail
  at plan time with precise messages.
- **Typed rows** — rows encode with the shared self-describing tuple codec,
  prefixed by the owning table's object id (tables share one record space and
  scans filter by it).
- **The wire-protocol server** — `SqlDatabaseServer` (+ `SqlDatabaseServerOptions`)
  fronts one engine on a configured `Connections` listener that it binds at
  start and releases at stop; the session state
  machine, guardrails, and two-phase drain are implemented inside this package
  (servers are per-model and each model carries its own copy of the machinery —
  owner decision 2026-07-14; see DESIGN.md).

## Database lifecycle

Create/open complete only a provably empty interrupted creation with its creation claim. Missing
sidecars and ambiguous catalogs are preserved and refused with CS03401; pre-header retries use
CreateDatabaseAsync or declared Build. Drop removes the primary data file first. Format remains 26.
See [signatures, remedies and examples](design.md#database-creation-open-and-drop).

## Usage

See the [source-backed usage examples](examples/index.md).

### Registering on a database application

`AddSql` captures engine intent through the area root's dependency-free builder contract. The
application builds and owns the engine; its optional server belongs to that engine. The callback and
nested factory execute during `Build`:

See the [source-backed usage examples](examples/index.md).

The `Listen` helper ships in `Database.Tcp`. Omit `AddServer` for embedded SQL.
`SqlDatabaseEngine.Create(options)` also remains available for standalone use. See
[DESIGN.md](design.md) for the execution model and its decisions.

`SqlDatabaseServerOptions.Listener` takes an `IDuplexConnectionListener`, including TCP and InMemory listeners. The server owns accepted duplex connections and their protocol I/O.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Sql.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Execution` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database.Sql.Language` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Schema` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Additional reference pages

- **[Sql catalog design](catalog-design.md)** — Reference and usage.
- **[Sql catalog](catalog.md)** — Reference and usage.
- **[SqlCatalogColumn](sql-catalog-column.md)** — Reference and usage.
- **[SqlCatalogConstraint](sql-catalog-constraint.md)** — Reference and usage.
- **[SqlCatalogIdentity](sql-catalog-identity.md)** — Reference and usage.
- **[SqlCatalogNamespace](sql-catalog-namespace.md)** — Reference and usage.
- **[SqlCatalogSnapshot](sql-catalog-snapshot.md)** — Reference and usage.
- **[SqlCatalogView](sql-catalog-view.md)** — Reference and usage.
- **[SqlCatalog](sql-catalog.md)** — Reference and usage.
- **[Sql storage design](storage-design.md)** — Reference and usage.
- **[Sql storage](storage.md)** — Reference and usage.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
