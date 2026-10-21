# Assimalign.Cohesion.Database.Sql

The SQL engine manages databases and executes SQL over shared storage, catalog, and transaction kernels.

> **Status:** Partial.

## Reference pages

- **[SqlFunctionCollection](sql-function-collection.md)** — Registration, reserved T1 names and freezing.
- **[SqlFunctionContext](sql-function-context.md)** — Database, collation and cancellation context.
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
  projection, N-way INNER/LEFT/CROSS/comma joins with ON/USING, grouping and aggregates,
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
sidecars and ambiguous catalogs are preserved and refused with COHSQLC001; pre-header retries use
CreateDatabaseAsync or declared Build. Drop removes the primary data file first. Format remains 25.
See [signatures, remedies and examples](design.md#database-creation-open-and-drop).

## Usage

See the [source-backed usage examples](examples/index.md).

### Registering on a database application

`AddSql` captures engine intent through the area root's dependency-free builder contract. The
application builds and owns the engine; its optional server belongs to that engine. The callback and
nested factory execute during `Build`:

See the [source-backed usage examples](examples/index.md).

The `Listen` helper ships in `Database.Sql.Tcp`. Omit `AddServer` for embedded SQL.
`SqlDatabaseEngine.Create(options)` also remains available for standalone use. See
[DESIGN.md](design.md) for the execution model and its decisions.

`SqlDatabaseServerOptions.Listener` takes an `IDuplexConnectionListener`, including TCP and InMemory listeners. The server owns accepted duplex connections and their protocol I/O.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Catalog` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Schema` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
