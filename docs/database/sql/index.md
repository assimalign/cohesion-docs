# SQL

The relational Database engine parses, plans, and executes a measured SQL subset over the shared durable kernel.

> **Status:** Partial. The profile measures 92 of 93 declared language clauses against the live engine.

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
Non-NATURAL INNER/LEFT/RIGHT/FULL accept ON or USING; USING merges each named pair into one
common-typed visible column. [NATURAL](language/clauses/join.md#natural-joins) derives those names;
qualified SELECT wildcards expand each input's own columns.
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

The 92-of-93 measure includes aggregate [FILTER](language/clauses/filter.md), executable MERGE
and executable RECURSIVE and is not a percentage
of complete SQL. The
[unsupported matrix](language/unsupported.md) lists the one excluded named clause and the further
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
SQL storage format is 39; compiled schemas remain version 12.

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
canonical state with the complete target in one catalog commit. Every schema apply publishes
jointly, including deferred work, `RunMigrationsInline`, empty-table Build and `ApplySchemaAsync`.

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

### Typed migration health and recovery

Every schema apply, including deferred work, `RunMigrationsInline`, empty-table Build and
`ApplySchemaAsync`, prepares the whole target unpublished and publishes it in one catalog commit.
Classification is per table. Nullable append, DROP COLUMN, compatible facet changes, SET/DROP
DEFAULT and supported constraint/index changes keep the heap without rewriting rows; validation
or new tree builds still run when required. Every index has its own physical owner: drops and
same-name replacements keep the heap when its layout permits, and retire only the old tree.
Primary-key changes and required TYPE replacements rewrite. Script-declared
indexes survive unless explicitly dropped or replaced by a same-name schema declaration. See
[kept-table preparation](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#joint-tables-that-keep-storage).
Hosting health is
Degraded while it is queued/running. Every touched table, including affected foreign-key neighbors,
is unavailable to reads and writes until completion; untouched tables stay usable, subject to
ordinary checkpoint stalls. Definition DDL is refused database-wide throughout queueing and work.

Preparation/space failure reports `CS03402` through Provisioning, health and DeferredStepFailed,
releases reservations, discards the pending intent and leaves the database **online at the old
schema**, unless an independent storage latch fires. Nothing retries on its own. Resolve the
reported condition, then explicitly apply or Build the target again. A rolled-back application's
Build declaring the old schema opens normally. A successful apply or Build clears the earlier
failure, including an already-applied schema skip; health then returns to Healthy if no other
condition prevents it. Inline apply uses the same joint publication boundary.

After a crash before publication, only a Build declaring the same pending target resumes the
migration by replanning preparation. A different-target Build discards with `CS03402` and replans
from the stored schema. An in-process reopen discards even a matching intent with `CS03402`;
a no-target open discards without a provisioning observation or trace. Successful Verify clears
a prior observation. Abandoned owners are reclaimed. After publication, reopen sees
the new schema. Returning to an earlier applied declaration follows the
[history-based rollback policy](#applied-history-and-application-rollback): an unsafe reversal
holds Build online with CS03405 and refuses explicit apply with CS03105, regardless of old opt-in.

`CS03403` means publication is **unconfirmed**, with an unknown old/new outcome. It reports through
Provisioning and the thrown diagnostic, keeps the new trees and directory records for recovery,
and follows the storage offline policy. Reopen/recovery decides the schema; do not treat this as
"schema unchanged." `CS03404` protects reserved new names from session relation creation/table rename.

See [space admission, cleanup and storage offline exceptions](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/storage-design.md#joint-migration-space-admission)
and [migration diagnostics](language/diagnostics.md#typed-schema-migrations).

## Database lifecycle

Engine APIs create, open and drop databases. SQL can finish an interrupted create only when its
creation claim and read-only probes prove initial empty data and creation-only catalog metadata.
Pre-header files with no conflicting persisted state require CreateDatabaseAsync or declared Build;
plain open reports CS03401. Missing journals/backups and ambiguous lost catalogs are refused
without writing or recreating files. Follow the coded refusal's remedy and preserve ambiguous data.
Transient inspection failures are retryable after the condition is resolved, without restore advice.
Drop removes sql.dat before the remaining file set. The current format is 39.

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
Format 31 added partial-index predicates in index extension 2. Format 32 added per-column
direction and NULL placement in mandatory index extension 3 and retained TYPE default assignment coercion.
Format 33 fenced SQL/JSON names and ordinary-call quoting. Format 34 introduced joint typed
migration intent semantics. Format 35 introduced separate physical owners for kept-table index trees and durable per-table
classification. Format 36 changed effective index orders and defaults to ASC NULLS FIRST,
matching ORDER BY. Format 37 required joint publication for every deferred job and fenced
per-step intents. Format 38 added applied-schema history and never shipped alone. Current format 39
adds a physical owner for every index. Formats 37, 38 and all earlier formats refuse CS06041
before any write, on clean opens and after an abrupt stop.
Explicit earlier format markers and unmarked user catalogs are refused at open; there is no
upgrade path. Proven interrupted creation follows the completion rules above. Compiled schema format remains version 12.

## Applied history and application rollback

Every applied compiled schema records its hash, FirstAppliedGeneration and LastAppliedGeneration
atomically with the current schema marker. History is never evicted; pending work is not applied
history. Reapplying a hash updates its last generation while preserving its first.

An application's declaration is a rollback only when its first-applied generation precedes the
stored schema's first-applied generation. A later one is a roll-forward under ordinary forward
policy, including its own destructive opt-in. An ancestor never applied here counts as new.

A rollback applies only Safe steps that neither re-create dropped tables/columns nor validate
existing data. One refused step holds the entire plan before any step or declared script runs.
Build succeeds, keeping the old application online at the stored schema with Degraded health,
CS03405, both hashes and every refused step in Provisioning.Detail. A never-seen destructive
declaration without opt-in holds with CS03105. Verify remains strict (CS03103).
Explicit ApplySchemaAsync follows the same rollback policy and throws CS03105 even when the
earlier declaration opted into destructive changes. Accepting loss requires a new declaration
with its own opt-in. See the [step rules and index proof](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#applied-schema-history-and-rollback).

A hold survives offline/reopen, rewrite and refused explicit apply while the stored hash stays
unchanged. Redeploying the stored or a successfully applied newer declaration clears it. Reads
remain available; stored constraints and defaults govern writes. Held Build skips removed
application-function/domain validation, so a write needing an unavailable registration can
still fail with its existing code. Script checksum validation still precedes planning.

## Table maintenance

[SqlDatabase.RewriteTableAsync](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database.md)
compacts dropped-column ordinals into a fresh physical owner and rebuilds indexes with recoverable
atomic publication. Old read-only snapshots retain the original layout; affected writers retry
with CS04006. The operation takes the exclusive source-table lock before queueing; nonzero engine
LockTimeout bounds the wait with CS04044 (55P03). This maintenance adds no SQL syntax or format bump.

[SQL/JSON path functions](language/functions/json-path.md) and [COPY TO STDOUT](language/statements/copy-to.md)
execute with bounded path work and negotiated outbound streaming respectively.

## SQL format 39 and schema v12

SQL opens only data/catalog format **39** and compiled documents in
**cohesion/database-schema/v12**. Format 30 adds ON UPDATE action bytes in table extension 8.
Format 31 added partial-index predicate text and dependencies in index extension 2.
Format 32 added per-column direction and NULL placement in mandatory index extension 3.
Format 33 fences SQL/JSON contextual names and quoted ordinary function-call rendering.
Format 34 introduced the kind-9 pending-intent meaning to support joint typed TYPE migrations with one-commit
publication. Format 35 introduced separate physical owners for new kept-table index trees and
per-table KeepOwner/RewriteOwner classifications. Format 36 changed effective index-order values
and defaults to ASC NULLS FIRST, matching ORDER BY. Compiled schema v12 makes the same change;
omitted and zero-filled KeyOrders vectors are equivalent. Primary-key and UNIQUE indexes use
that default too.
Format 37 required joint publication for every deferred job and fenced old per-step intents.
Format 38 added chunked, never-evicted applied-schema history (kind 20) and never shipped alone.
Current format 39 adds per-index owners and registration kind 21. Formats 37 and 38, along with
all earlier formats, refuse CS06041 before any write, leaving clean files and crashed journals
unchanged. Every SQL database from batch 4 must be recreated.
Format 29 extended kind-18 sequence definitions with owning
table id, column name and identity ownership. Identity allocation uses kind-19 reservations;
kind 16 is retired and never decoded. Format 30 retains the TYPE default-assignment flag introduced
in format 27/table extension 7 and format 28's standalone-sequence records.
Format 38 and every earlier database format are refused before data recovery;
there is no upgrade path or legacy reader. Clean and crashed format refusals preserve every file
byte for byte; recovered read-only probes perform no lifecycle telemetry or recovery writes.
Accepted databases retain normal recovery. Compiled schema v12 retains v11's effective KeyOrders,
v10's optional Predicate and v9's always-serialized onUpdate member. V11 and earlier artifacts
are refused. SDK validation finishes before creating a migration file or its directory.

Format 26 introduced the temporal DEFAULT and frozen missing-value fence: an older INTERVAL
`'12'` meant days, while current input means seconds. Typed TimeSpan defaults render exact signed
total seconds with a `seconds` unit; that earlier change established schema v8.

Older catalogs are refused before stored expressions bind (CS06041); earlier schema artifacts,
applied markers and pending targets fail with SqlSchemaValidationException, InvalidDocument at
`schema.format`, before hash comparison. SDK migration inputs fail COHDBSDK206 without writing
a migration or changing inputs. Rebuild schema artifacts; export old data with the engine that
wrote it, recreate the database and reapply the rebuilt declaration before reloading. There is
no upgrade path or legacy reader (#1152). Shared physical storage remains format 4 and B-tree
pages require format 3; format-2 trees refuse CK06101 before any write. These have separate
version gates. If both the SQL marker and B-tree stamp are old, the marker refusal CS06041
comes first; see [combined format refusals](../error-codes.md#combined-format-refusals).

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
