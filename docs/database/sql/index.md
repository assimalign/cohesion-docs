# SQL

The relational Database engine parses, plans, and executes a measured SQL subset over the shared durable kernel.

> **Status:** Partial. Phase 22 measures 33 of 49 declared language clauses against the live engine.

## Language reference

The [Language (SQL)](language/index.md) hub is the contract for accepted statements, clauses,
expressions, functions, types, and diagnostics. Start with
[SELECT](language/statements/select.md) for a fixture and complete typed-client query example.

The measured surface includes table and index data definition language (DDL), row mutations,
session transactions, column constraints, and queries over one stored table, one virtual
system relation, or exactly two stored tables joined with `INNER JOIN ... ON`.
Queries support filtering, grouping, aggregates, aliases and ordinals in ordering, pagination,
collation, and bounded uncorrelated subqueries. `INSERT ... SELECT` uses transactional destination
validation. Literal added-column defaults resolve old-row reads without rewriting row bytes.

The 33-of-49 measure is not a percentage of complete SQL. The
[unsupported matrix](language/unsupported.md) lists the excluded 16 named clauses and the further
boundaries inside supported clauses. Later comparison work is documented in
[Operators and predicates](language/expressions/operators.md).

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
durability and recovery. The planner can use applicable secondary indexes and supported join probes.

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

## See also

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
