# SQL

The relational Database engine parses, plans, and executes a measured SQL subset over the shared durable kernel.

> **Status:** Partial. The profile measures 87 of 89 declared language clauses against the live engine.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](../model-project-migration.md).

## Language reference

The [Language (SQL)](language/index.md) hub is the contract for accepted statements, clauses,
expressions, functions, types, and diagnostics. Start with
[SELECT](language/statements/select.md) for a fixture and complete typed-client query example.

The measured surface includes table and index data definition language (DDL), row mutations,
session transactions, column constraints, and queries over stored tables and virtual system
relations with N-way [INNER, LEFT, RIGHT, FULL, CROSS and comma joins](language/clauses/join.md).
INNER/LEFT/RIGHT/FULL accept ON or USING; USING merges each named pair into one common-typed visible column.
FULL preserves unmatched rows from both sides and requires a safe mandatory hash equality.
Queries support filtering, grouping, aggregates, aliases and ordinals in ordering, pagination,
collation, and bounded correlated and uncorrelated [subqueries](language/expressions/subqueries.md). `INSERT ... SELECT` uses transactional destination
validation. Literal added-column defaults resolve old-row reads without rewriting row bytes.

[Derived tables](language/clauses/from.md#derived-tables) execute in FROM/JOIN/comma inputs;
correlated and uncorrelated subqueries execute in UPDATE/DELETE WHERE over pre-statement values.
[WITH](language/clauses/with.md) defines acyclic query relations before SELECT, INSERT, UPDATE,
DELETE or MERGE. WITH RECURSIVE executes UNION ALL/DISTINCT fixpoints on demand, with shared
readers and accounted retained state; WITH RECURSIVE before MERGE remains excluded. Outer ordering over derived
outputs accepts per-key NULLS FIRST/LAST. [Identity columns](language/clauses/identity.md)
generate integers with ALWAYS/BY DEFAULT rules and durable reservations that can leave gaps.

[TRUNCATE](language/statements/truncate.md) empties table sets with CONTINUE/RESTART IDENTITY and
RESTRICT/recursive CASCADE. It is autocommit-only (CS04066 in an explicit transaction, preserving
prior work) and reports affected count 0. Older snapshots keep their rows through joins, views and
indexes until they end; retired storage is reclaimed afterwards.
[CREATE TABLE AS](language/statements/create-table.md#create-table-as) atomically creates and
optionally populates an inferred table.

[ON CONFLICT](language/clauses/on-conflict.md) executes DO NOTHING or targeted DO UPDATE SET/WHERE
with EXCLUDED over VALUES, DEFAULT VALUES and query sources. RETURNING includes inserted/updated
rows and counts. Snapshot conflicts abort retryably; ReadCommitted skips or updates the newest
committed row. Multi-row UPDATE checks foreign keys against its
[final replacements](language/constraints.md#update-final-row-examples), permitting consistent self-reference moves.

The 87-of-89 measure includes aggregate [FILTER](language/clauses/filter.md), executable MERGE
and executable RECURSIVE and is not a percentage
of complete SQL. The
[unsupported matrix](language/unsupported.md) lists the 2 excluded named clauses and the further
boundaries inside supported clauses. [Operators and predicates](language/expressions/operators.md)
describes PostgreSQL precedence, NULL-safe distinct predicates, Boolean truth tests and LIKE ESCAPE.

[Standalone sequences](language/statements/create-sequence.md) provide SQL DDL, session functions,
NEXT VALUE FOR and INFORMATION_SCHEMA.SEQUENCES. [Grouping sets](language/clauses/group-by.md)
provide ROLLUP/CUBE, empty sets and GROUPING masks. [Identity integration](language/clauses/identity.md), SERIAL aliases, OWNED BY, ALTER identity,
OVERRIDING and populated-table identity backfill execute through owned sequences.

[EXPLAIN](language/statements/explain.md) returns chosen plans without execution.
[MERGE](language/statements/merge.md) executes ordered actions with RETURNING, ReadCommitted
target rechecks and mixed-action final-state foreign keys. Statement-level uniqueness permits
key swaps and reuse of keys freed by its deletes; swaps differ from PostgreSQL's immediate checks.
[CAST](language/expressions/cast.md), [typed literals](language/expressions/literals.md),
[DEFAULT assignments](language/clauses/default.md) and 64-bit [pagination](language/clauses/limit-offset.md)
share the same behavior in process and through the supported wire codecs.

[ALTER TABLE](language/statements/alter-table.md) executes column TYPE with old-row USING,
table/column renames and column SET/DROP DEFAULT and SET/DROP NOT NULL. [Scalar functions](language/functions/builtin-functions.md)
execute 42 T1 names, including transaction-start clocks and day-time temporal operations.
SQL storage format is 32; compiled schemas remain version 11.

[OVER and WINDOW](language/clauses/window.md) provide ordered partitions, named inheritance,
[ranking/distribution functions and LAG/LEAD](language/functions/window-functions.md). They run
after HAVING and before DISTINCT, final ordering and paging. ROWS, RANGE with offsets, GROUPS,
EXCLUDE, aggregate windows with FILTER and IGNORE/RESPECT NULLS execute. Safe native moving
aggregates run in O(n) work; application/order-sensitive aggregates and restart-sensitive inputs
retain the [recomputation cap](language/clauses/window.md#recomputation-limit).

[Persisted views](language/statements/create-view.md) are read-only catalog relations, removable
with [DROP VIEW](language/statements/drop-view.md). [RETURNING](language/clauses/returning.md)
returns direct write rows and affected counts. [ORDER BY](language/clauses/order-by.md) supports
NULLS FIRST/LAST in every ordering position.

## Package responsibilities

| Package | Responsibility |
| --- | --- |
| `Assimalign.Cohesion.Database.Sql` | `SqlDatabaseEngine`, sessions, planner/executor, schema provisioner, `SqlDatabaseServer`, and SQL wire family |
| `Assimalign.Cohesion.Database.Sql.Language` | `SqlQueryParser`, statement/expression trees, `SqlLanguageProfile`, `SqlTypeNames`, and diagnostics |
| Catalog component in `Assimalign.Cohesion.Database.Sql` | Durable table, column, constraint, index, collation, ownership, and applied-schema metadata |
| `Assimalign.Cohesion.Database.Sql.Schema` | `SqlSchema.Compile`, compiled relational shapes, canonical serialization, and migration planning |
| Storage component in `Assimalign.Cohesion.Database.Sql` | SQL row operations over shared pages, record chains, and write-ahead logging |
| `Assimalign.Cohesion.Database.Tcp` | Optional `SqlDatabaseServerOptions.Listen(Uri)` Transmission Control Protocol (TCP) composition helper |
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

[BEGIN isolation options](language/statements/begin-transaction.md) and
[SET TRANSACTION](language/statements/set-transaction.md) select ReadCommitted or Snapshot.
SET must precede the first admitted data statement; Snapshot stays fixed at the original BEGIN.
SERIALIZABLE is refused. Optionless BEGIN and automatic statements default to Snapshot.

Commit and rollback observe caller cancellation before their end starts, leaving the transaction
usable when canceled at that point. A started commit ignores caller cancellation and returns its
real outcome. Automatic commit takes over after execution completes; self-committed DDL finishes
mandatory cleanup and delivers success despite late cancellation. A lost wire connection can
still lose that result. See [COMMIT](language/statements/commit-transaction.md).

Compiled schemas own their tables and indexes. Session DDL cannot alter/drop schema-owned tables
or drop schema-owned indexes, although ordinary row DML remains available. Provisioning records
canonical state only after catalog convergence. Multi-statement destructive migration atomicity is
not claimed; reversible completed steps are compensated on failure.

## Execution limits

[Statement and lock timeouts](language/diagnostics.md#statement-and-lock-timeouts) are disabled by
default. Engine options and nullable session overrides bound cooperative statement execution and
each contended lock wait. CS05043 (57014) and CS04044 (55P03) preserve the session and explicit
transaction after rolling back the failed statement. Started commits and mandatory cleanup after
self-committing DDL complete without these deadlines. Provisioning schema sessions disable both.

See [engine options](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md)
and [session overrides](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-session.md).


## Engine health
Background write-back, group-commit flush, version purge and checkpoint work run per database
on capped lanes: 64 non-stuck operations and at most 128 lane threads per engine/worker kind.
Admission is round-robin; stuck I/O releases its admission slot after four worker intervals,
clamped to 1–4 seconds, while keeping storage ownership. Idle checks dispatch no lane; parked
runners retire after 30 seconds. Saturation/stuck observations leave databases serving and emit
root warning 35; disposal joins in-flight I/O and emits warning 36 periodically. See
[background worker limits and operations](../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).
SQL/Key-Value index-maintenance bodies remain empty; they dispatch no production lane work.


Persistent version-purge refusals are reported once after three actual attempts spanning at least
one second on the engine clock. They set engine Faulted while the database stays in service and
HasFailingWorker remains false for the observation. ForeignStorage refusals report immediately as
operational failures. See [purge retries and reclamation recovery](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#version-purge-refusals)
and [shared engine health](../overview.md#engine-health-and-version-purge).

## Database lifecycle

Engine APIs create, open and drop databases. SQL can finish an interrupted create only when its
creation claim and read-only probes prove initial empty data and creation-only catalog metadata.
Pre-header files with no conflicting persisted state require CreateDatabaseAsync or declared Build;
plain open reports CS03401. Missing journals/backups and ambiguous lost catalogs are refused
without writing or recreating files. Follow the coded refusal's remedy and preserve ambiguous data.
Transient inspection failures are retryable after the condition is resolved, without restore advice.
Drop removes sql.dat before the remaining file set. The current format is 32.

See [lifecycle APIs and examples](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#database-creation-open-and-drop)
and [creation diagnostics](language/diagnostics.md#database-creation-and-open).

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

Writing statements prepare all result frames before commit, so an encoding failure rolls back
only that statement. Reads encode during delivery and can end a partial result with ExecutionFailure;
the client discards it. Result failures keep the connection and explicit transaction usable.
CS05045 identifies uncoded encoding/preparation failures and header/row size refusals (54000
for size refusals). Each header/row fits 16 MiB; there is no aggregate cap. Transport failures
during delivery are terminal. See [result diagnostics](language/diagnostics.md#result-encoding-and-delivery)
and [wire exchanges](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-protocol/design.md#statement-results).

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Format 30 added ON UPDATE action metadata in table extension 8.
Format 31 added partial-index predicates in index extension 2. Current format 32 adds per-column
direction and NULL placement in mandatory index extension 3 and retains TYPE default assignment coercion.
Explicit earlier format markers and unmarked user catalogs are refused at open; there is no
upgrade path. Proven interrupted creation follows the completion rules above. Compiled schema format remains version 11.

## Table maintenance

[SqlDatabase.RewriteTableAsync](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database.md)
compacts dropped-column ordinals into a fresh physical owner and rebuilds indexes with recoverable
atomic publication. Old read-only snapshots retain the original layout; affected writers retry
with CS04006. The operation takes the exclusive source-table lock before queueing; nonzero engine
LockTimeout bounds the wait with CS04044 (55P03). This maintenance adds no SQL syntax or format bump.

## SQL format 32 and schema v11

SQL opens only data/catalog format **32** and compiled documents in
**cohesion/database-schema/v11**. Format 30 adds ON UPDATE action bytes in table extension 8.
Format 31 added partial-index predicate text and dependencies in index extension 2.
Format 32 adds per-column direction and NULL placement in mandatory index extension 3.
Format 29 extended kind-18 sequence definitions with owning
table id, column name and identity ownership. Identity allocation uses kind-19 reservations;
kind 16 is retired and never decoded. Format 30 retains the TYPE default-assignment flag introduced
in format 27/table extension 7 and format 28's standalone-sequence records.
Format 31 and every earlier database format are refused at open before stored expressions bind;
there is no upgrade path or legacy reader. Compiled schema v11 carries effective KeyOrders,
retaining v10's optional Predicate and v9's always-serialized onUpdate member.

Format 26 introduced the temporal DEFAULT and frozen missing-value fence: an older INTERVAL
`'12'` meant days, while current input means seconds. Typed TimeSpan defaults render exact signed
total seconds with a `seconds` unit; that earlier change established schema v8.

Older catalogs are refused before stored expressions bind (CS06041); earlier schema artifacts,
applied markers and pending targets fail with SqlSchemaValidationException, InvalidDocument at
`schema.format`, before hash comparison. SDK migration inputs fail COHDBSDK206 without writing
a migration or changing inputs. Rebuild schema artifacts; export old data with the engine that
wrote it, recreate the database and reapply the rebuilt declaration before reloading. There is
no upgrade path or legacy reader (#1152). Shared physical storage remains format 4 and B-tree
pages remain format 2; these have separate version gates.

## See also

[CREATE SCHEMA](language/statements/create-schema.md) and [DROP SCHEMA](language/statements/drop-schema.md)
manage persisted namespaces. dbo is seeded and protected; code-first provisioning stays in dbo.
[Set operations](language/clauses/set-operations.md) compose query bodies in every query position.

[Database](../index.md) · [Overview](../overview.md) · [Database .NET APIs](../../dotnet-apis/resources/database/index.md)

## Query performance

[CREATE INDEX ... WHERE](language/statements/create-index.md#partial-indexes) maintains a partial
subset on every write and enforces partial uniqueness with distinct NULL keys. Untargeted
ON CONFLICT DO NOTHING uses membership; targeted inference accepts a WHERE proving the index
predicate. Query planning can select partial seeks, ranges, join probes and subset scans when
[the supported implication rules](language/statements/create-index.md#partial-indexes) prove eligibility.

[Probe costing, hash memory and plan analysis](performance.md) describe access selection and its limits.

## Sources

- **Format and temporal compatibility** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTemporalAssignmentConformanceTests.cs`, `SqlDataStorageFormatTests.cs` and `SqlSchemaFormatTests.cs` in the same directory.
- **SDK input preservation** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/CreateDatabaseMigrationTaskTests.cs`.
- **Dialect and measurement** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Engine architecture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Engine surface** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`
- **Wire family** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Schema** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/DESIGN.md`
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **TCP integration** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/docs/OVERVIEW.md`
- **Client** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
