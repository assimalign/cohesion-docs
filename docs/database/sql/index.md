# SQL

The relational Database engine parses, plans, and executes a measured SQL subset over the shared durable kernel.

> **Status:** Partial. The profile measures 51 of 57 declared language clauses against the live engine.

## Language reference

The [Language (SQL)](language/index.md) hub is the contract for accepted statements, clauses,
expressions, functions, types, and diagnostics. Start with
[SELECT](language/statements/select.md) for a fixture and complete typed-client query example.

The measured surface includes table and index data definition language (DDL), row mutations,
session transactions, column constraints, and queries over stored tables and virtual system
relations with N-way [INNER, LEFT, CROSS and comma joins](language/clauses/join.md).
INNER/LEFT accept ON or USING; USING merges each named pair into one common-typed visible column.
Queries support filtering, grouping, aggregates, aliases and ordinals in ordering, pagination,
collation, and bounded correlated and uncorrelated [subqueries](language/expressions/subqueries.md). `INSERT ... SELECT` uses transactional destination
validation. Literal added-column defaults resolve old-row reads without rewriting row bytes.

[Derived tables](language/clauses/from.md#derived-tables) execute in FROM/JOIN/comma inputs;
correlated and uncorrelated subqueries execute in UPDATE/DELETE WHERE over pre-statement values.
[WITH](language/clauses/with.md) defines non-recursive query relations before SELECT, INSERT,
UPDATE or DELETE. Outer ordering over derived outputs accepts per-key NULLS FIRST/LAST. [Identity columns](language/clauses/identity.md)
generate integers with ALWAYS/BY DEFAULT rules and durable reservations that can leave gaps.

[ON CONFLICT](language/clauses/on-conflict.md) executes DO NOTHING or targeted DO UPDATE SET/WHERE
with EXCLUDED over VALUES, DEFAULT VALUES and query sources. RETURNING includes inserted/updated
rows and counts. Snapshot conflicts abort retryably; ReadCommitted skips or updates the newest
committed row. Multi-row UPDATE checks foreign keys against its
[final replacements](language/constraints.md#update-final-row-examples), permitting consistent self-reference moves.

The 51-of-57 measure is not a percentage of complete SQL. The
[unsupported matrix](language/unsupported.md) lists the 6 excluded named clauses and the further
boundaries inside supported clauses. [Operators and predicates](language/expressions/operators.md)
describes PostgreSQL precedence, NULL-safe distinct predicates, Boolean truth tests and LIKE ESCAPE.

[EXPLAIN](language/statements/explain.md) returns chosen plans without execution.
[CAST](language/expressions/cast.md), [typed literals](language/expressions/literals.md),
[DEFAULT assignments](language/clauses/default.md) and 64-bit [pagination](language/clauses/limit-offset.md)
share the same behavior in process and through the supported wire codecs.

[ALTER TABLE](language/statements/alter-table.md) executes table/column renames and column
SET/DROP DEFAULT and SET/DROP NOT NULL. [Scalar functions](language/functions/builtin-functions.md)
execute 42 T1 names, including transaction-start clocks and day-time temporal operations.
SQL storage format is 25; compiled schemas remain version 7.

[Persisted views](language/statements/create-view.md) are read-only catalog relations, removable
with [DROP VIEW](language/statements/drop-view.md). [RETURNING](language/clauses/returning.md)
returns direct write rows and affected counts. [ORDER BY](language/clauses/order-by.md) supports
NULLS FIRST/LAST in every ordering position.

## Package responsibilities

| Package | Responsibility |
| --- | --- |
| `Assimalign.Cohesion.Database.Sql` | `SqlDatabaseEngine`, sessions, planner/executor, schema provisioner, `SqlDatabaseServer`, and SQL wire family |
| `Assimalign.Cohesion.Database.Sql.Language` | `SqlQueryParser`, statement/expression trees, `SqlLanguageProfile`, `SqlTypeNames`, and diagnostics |
| `Assimalign.Cohesion.Database.Sql.Catalog` | Durable table, column, constraint, index, collation, ownership, and applied-schema metadata |
| `Assimalign.Cohesion.Database.Sql.Schema` | `SqlSchema.Compile`, compiled relational shapes, canonical serialization, and migration planning |
| `Assimalign.Cohesion.Database.Sql.Storage` | SQL row operations over shared pages, record chains, and write-ahead logging |
| `Assimalign.Cohesion.Database.Sql.Tcp` | Optional `SqlDatabaseServerOptions.Listen(Uri)` Transmission Control Protocol (TCP) composition helper |
| `Assimalign.Cohesion.Database.Sql.Client` | Typed commands, parameters, materialized result sets, errors, and observer hooks over the shared client |

## Execution and lifetime

`SqlQueryParser` produces a statement tree; the internal `SqlPlanner` binds it against the
catalog, and `SqlPlanExecutor` runs the bound plan. Tables have stable object identities.
Each database owns a data file set and a separate catalog file set; shared storage supplies
durability and recovery. SELECT, UPDATE and DELETE share [index eligibility rules](language/statements/select.md#index-access);
the planner also supports indexed join probes. UPDATE/DELETE materialize targets before mutation.

Every session stays in the database that created it. SQL schema qualification never switches
databases; engine APIs own database creation, opening, dropping, and enumeration.
Statements auto-commit unless attached to an explicit session transaction. Row visibility uses
multi-version concurrency control (MVCC); DDL is self-committing and rejected inside explicit
transactions.

Compiled schemas own their tables and indexes. Session DDL cannot alter/drop schema-owned tables
or drop schema-owned indexes, although ordinary row DML remains available. Provisioning records
canonical state only after catalog convergence. Multi-statement destructive migration atomicity is
not claimed; reversible completed steps are compensated on failure.

## Hosting and client

`AddSql` records deferred intent on the application builder. Its callback configures the model
builder during `Build`; optional server factories are nested on that engine builder.
The built engine is usable immediately. Host startup performs provisioning before servers accept.
The [Database overview](../overview.md#getting-started) contains the complete host shape.

The SQL endpoint fixes `SqlProtocol.Family` for a connection. After the shared handshake, the
client submits one statement at a time. Row results are header, rows, then completion; other results
are completion alone. `ParseFailure` and `ExecutionFailure` end the exchange while preserving a
usable session. Protocol errors close it. There is no pipelining or multiplexing.

`SqlClient` sends text and parameters without parsing them and materializes complete result sets.
Transactions use `BEGIN`, `COMMIT`, and `ROLLBACK` on one connection rather than a separate
wire transaction payload.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Current format 25 adds distinct predicates, Boolean tests and LIKE ESCAPE.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## See also

[CREATE SCHEMA](language/statements/create-schema.md) and [DROP SCHEMA](language/statements/drop-schema.md)
manage persisted namespaces. dbo is seeded and protected; code-first provisioning stays in dbo.
[Set operations](language/clauses/set-operations.md) compose query bodies in every query position.

[Database](../index.md) · [Overview](../overview.md) · [Database .NET APIs](../../dotnet-apis/resources/database/index.md)

## Sources

- **Dialect and measurement** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Engine architecture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Engine surface** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`
- **Wire family** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/DESIGN.md`
- **Schema** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/DESIGN.md`
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Storage/docs/DESIGN.md`
- **TCP integration** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/docs/OVERVIEW.md`
- **Client** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
