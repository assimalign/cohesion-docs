# Diagnostics (Cohesion SQL)

SQL reports coded parser, binding, planning and execution diagnostics.

> **Status:** Implemented.

## Language diagnostics

| Code | Meaning |
| --- | --- |
| `COHDBL001` | Recognized clause, function, operator or modifier outside the executable profile |
| `SQL0001` | Empty statement |
| `SQL0002` | Unknown leading command |
| `SQL0003` | Malformed syntax, invalid parameter form, leftover text, bare DEFAULT outside its DML positions, or explicit NULL conflicting with NOT NULL/PRIMARY KEY/IDENTITY |
| `SQL0004` | Retired; unknown type names now resolve in the engine and use COHSQLE010 |
| `SQL0005` | Invalid type modifiers or rejected spelling, including TIME WITH TIME ZONE, BIT and SMALLDATETIME |
| `SQL0006` | Configured expression nesting limit exceeded; default 256 |
| `SQL0007` | Parsing thread has insufficient stack; engine text execution translates to COHSQLE004 |
| `SQL0008` | Malformed or overflowing typed temporal, interval or X binary literal; span covers the literal |
| `SQL0100` | Information: optional terminating semicolon omitted |

For example, `SELECT DATE '2024-02-30' FROM t;` reports SQL0008 before execution.
Recognized unsupported window functions such as RANK report COHDBL001, including over an empty
table. The [42 T1 scalars](functions/builtin-functions.md) execute; incompatible signatures use
COHSQLE006 before reading rows.
Only executable functions appear in the language profile's function table.

## Planning and execution diagnostics

A dash means the source does not assign a SQLSTATE here. SQLSTATE is a reference category;
the structured diagnostic carries Code, Message, Severity and location fields.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHSQLE001` | Division or remainder by zero | 22012 |
| `COHSQLE002` | Numeric domain/range/overflow, non-finite float to integer or scalar result, temporal overflow, negative SUBSTRING length or string result/work-buffer cap | 22003 |
| `COHSQLE003` | Invalid operand type, including set-operation type mismatch (names output position/types), unsupported temporal arithmetic pairs, nonnumeric arithmetic, non-Boolean WHERE/HAVING/ON/CHECK/domain CHECK/CASE WHEN/ON CONFLICT WHERE condition or AND/OR/NOT/truth-test operand | 42804 |
| `COHSQLE004` | Statement needs more stack during parsing/planning/evaluation | 54001 |
| `COHSQLE005` | Column reference where no columns are in scope: VALUES or LIMIT/OFFSET/FETCH/TOP | Class 42 |
| `COHSQLE006` | No function overload accepts arity/types; unknown or incompatible temporal field value; constant/direct parameter fields fail planning, row-dependent DATE_TRUNC fields fail evaluation | 42883 |
| `COHSQLE007` | Function or registered conversion implementation failed | 38000 |
| `COHSQLE008` | Ambiguous overload; CAST an argument to choose | 42725 |
| `COHSQLE009` | Persisted call's required function implementation is unavailable or incompatible | 42883 |
| `COHSQLE010` | Unknown type/domain or unavailable stored domain | 42704 |
| `COHSQLE011` | No conversion for a CAST pair; known pairs checked during planning | 42846 |
| `COHSQLE012` | Character assignment exceeds length with non-space overflow | 22001 |
| `COHSQLE013` | Invalid statement/expression/DDL/query shape; derived width/name error, rename collision or incompatible DEFAULT function result type | — |
| `COHSQLE014` | Table does not exist for reads or DML, including a missing namespace | 42P01 |
| `COHSQLE015` | Unknown/ambiguous column, hidden/unknown/out-of-scope qualifier, new CHECK naming another table/schema, or missing/repeated/ambiguous/incompatible-type USING column | — |
| `COHSQLE016` | Unknown function absent from executable and recognized-unsupported vocabulary | — |
| `COHSQLE017` | Invalid aggregate placement/grouping, including RETURNING or a non-grouping outer column referenced from a completed group | — |
| `COHSQLE018` | Invalid ordering ordinal, row count or CAST declaration | — |
| `COHSQLE019` | Duplicate INSERT/SET target column, key-list column (including FOREIGN KEY referenced columns), PRIMARY KEY declaration or table binding; ambiguous output alias | — |
| `COHSQLE020` | Missing parameter, checked before reading or writing rows | — |
| `COHSQLE021` | Mutation or table DDL targets a read-only system relation or persisted view | 25006 |
| `COHSQLE022` | Invalid CAST input text representation | 22P02 |
| `COHSQLE023` | Parameter keys collide after stripping one sigil, including unused keys | — |
| `COHSQLE024` | NOT NULL column receives NULL or DEFAULT without a usable default | 23502 |
| `COHSQLE025` | Identity definition, assignment, ALTER, backfill or allocation violates its generation contract, including range exhaustion | 428C9 (`generated_always`) for explicit ALWAYS assignments; other identity failures share the local code |
| `COHSQLE026` | Duplicate CTE name or unresolved reference to a later sibling; binding fails before reads/writes | — |
| `COHSQLE027` | ADD COLUMN has a volatile DEFAULT and live rows; a physical rewrite would be required | — |
| `COHSQLE028` | Missing namespace in schema/table/index DDL; DROP TABLE IF EXISTS remains a no-op | 3F000 |
| `COHSQLE029` | CREATE SCHEMA finds an existing namespace without IF NOT EXISTS | 42P06 |
| `COHSQLE030` | DROP SCHEMA RESTRICT finds dependent objects; message offers CASCADE | 2BP01 |
| `COHSQLE031` | DROP SCHEMA targets dbo, required by code-first provisioning | — |
| `COHSQLE033` | View declaration/replacement failure, changed bound column identity, invalid dependency/cycle, oversized record, wrong DROP VIEW target or recoverable uncoded body bind failure | — |
| `COHSQLE032` | DISTINCT/ALL/ORDER BY on a non-aggregate function, or DISTINCT input ordering key not structurally present in the argument list | — |
| `COHSQLE034` | LIKE ESCAPE has more than one Unicode scalar or the pattern ends in its escape; fixed/parameter patterns validate eagerly, dynamic patterns at evaluation | 22025 |
| `COHSQLE041` | Storage/index operation failed at an engine boundary, including wrapped/aggregate causes; original cause retained | 58030 for I/O |
| `COHSQLE043` | Server-side statement execution deadline expired; failed statement effects are rolled back | 57014 (`query_canceled`) |
| `COHSQLE044` | Individual contended lock wait expired, including unique-index key waits | 55P03 (`lock_not_available`) |
| `COHSQLE042` | Encoded row or index key exceeds its write-size limit; names the table, actual byte size and limit | 54000 (`program_limit_exceeded`) |
| `COHSQLE040` | Invariant LIKE/NOT LIKE exceeds its per-match work budget; located ExecutionFailure leaves the connection usable | — |
| `COHSQLE035` | ON CONFLICT lacks a matching unique arbiter, names a nonunique/missing target, or DO UPDATE omitted its target; planning failure | 42P10 |
| `COHSQLE036` | ON CONFLICT DO UPDATE would affect a row already inserted or updated by this statement | 21000 |
| `COHSQLE037` | UNIQUE, PRIMARY KEY or unique-index violation, including a second unique index during ON CONFLICT DO UPDATE | 23505 |
| `COHSQLE038` | Table CHECK or domain-check violation on a write, CAST to a domain, or function domain argument/result | 23514 |
| `COHSQLE039` | FOREIGN KEY violation: missing parent on INSERT/UPDATE or a referenced key protected by NO ACTION/RESTRICT on DELETE/UPDATE, including final-row checks | 23503 |

Each SqlConstraintViolationException carries exactly one Error diagnostic. Messages name the
constraint and schema-qualified table without key values; domain CAST and function checks name
the domain without a table. The session attaches the enclosing statement span when available;
construction without source text leaves the diagnostic unlocated. NOT NULL retains COHSQLE024
(23502); its column-assignment failures can remain unlocated. SQLSTATE is a documented mapping,
not a separate field on the diagnostic or SQL exception.

[Predicate precedence](expressions/operators.md) rejects non-associative chains such as `a < b < c`
with SQL0003 at the second operator. Completed IN lists/subqueries may chain. ILIKE, SIMILAR TO,
ISNULL, NOTNULL, IS JSON/OF/NORMALIZED, binary bitwise operators and `::` casts use COHDBL001.
Invalid fixed LIKE patterns fail binding even on empty input or with a NULL matched value;
this eager check intentionally differs from PostgreSQL. Truth tests require BOOLEAN or NULL.

[Set operations](clauses/set-operations.md) use COHSQLE013 for unequal widths, COHSQLE018 for
combined ORDER BY expressions/qualified names/invalid ordinals, and COHSQLE019 for ambiguous
output names. CORRESPONDING uses COHDBL001. [Schema DDL](statements/create-schema.md) rejects
AUTHORIZATION and nested schema elements with COHDBL001. DROP revalidation after a concurrent
dependency change uses COHSQLE013; schema ownership retains DatabaseObjectLockedException.

For example, `SELECT t.id FROM t AS x;` reports located COHSQLE015 because the alias hides t.
NOT NULL failures name the column; ordinary column-assignment failures have no execution-time source span.
Correlated references resolve through enclosing query scopes. A real derived sibling reference
uses `COHDBL001` naming LATERAL; unknown, ambiguous or hidden names retain `COHSQLE015`.
An aggregate assigned to an outer level uses `COHDBL001` before grouped-column validation.
For WITH, a later sibling's name reads a stored table when one exists; only a missing future
relation receives `COHSQLE026`. No SQLSTATE is assigned to that code.

[Aggregate modifiers and text concatenation](functions/aggregate-functions.md) use SQL0003 for
quantified star or malformed LISTAGG ordering, COHSQLE006 (42883) for arity/type mismatch,
COHSQLE002 (22003) for output above 1,048,576 UTF-16 units, and COHDBL001 for nonconstant
delimiters or ON OVERFLOW. COHSQLE032 has no assigned SQLSTATE.

The [LIKE work limit](expressions/operators.md#like-and-escape) is
`64 × (input UTF-16 length + 1) × (raw pattern UTF-16 length + 1)` with checked 64-bit arithmetic.
COHSQLE040 refuses the match rather than estimating TRUE/FALSE; cancellation takes precedence.

Non-Boolean search conditions use COHSQLE003 (42804) before reading even an empty input when
the type is known, and during evaluation otherwise. CASE/COALESCE alternatives in Boolean
positions all qualify or fail, including unreachable alternatives. Untyped NULL is UNKNOWN;
valid Boolean input string literals coerce, while text columns/parameters keep their types.
Stored SQL function bodies with legacy invalid Boolean operands or changed application return
types allow the database to open and fail with COHSQLE003 when invoked.

## Statement and lock timeouts

The engine's StatementTimeout and LockTimeout default to TimeSpan.Zero (disabled). Nullable
[session overrides](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-session.md)
inherit the engine snapshot when null; zero disables an inherited timer. Positive durations up to
2,147,483,647 milliseconds apply to subsequent statements. Configure these through .NET objects;
SQL SET syntax is unavailable.

StatementTimeout starts after text parsing and cooperatively cancels execution, including planning,
functions, scans, cascade preparation and physical apply. LockTimeout starts a fresh budget for
each contended wait, including unique-key waits. The earlier deadline that ends the wait fixes its
error even if another timer expires before the continuation runs. Caller cancellation retains
OperationCanceledException semantics.

COHSQLE043 (57014) and COHSQLE044 (55P03) roll back only the failed statement's effects. The session
and explicit transaction remain usable. New lock resources are released after physical rollback;
earlier grants and upgrades of previously held resources remain until transaction end. Offline or
abandoned writers retain grants for recovery. Ordinary caller cancellation keeps its existing
transaction-duration locking behavior.

Transaction-control requests arm neither timer. A started commit is never canceled, and completed
execution hands off to automatic commit without a timer token. Self-committed DDL finishes mandatory
cleanup and returns its committed result despite late expiration. Provisioning schema sessions,
including deferred index builds, disable both timers and still observe caller/lifecycle cancellation.
[SqlTimeoutException](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-timeout-exception.md)
exposes SqlState in process. The wire uses ExecutionFailure and the existing negotiated diagnostic
code/span carrier; SQLSTATE is not a new wire field.


## Row and index-key limits

An encoded row must fit **8,092 bytes**, including its MVCC header and physical column layout.
Every unique or nonunique index key must fit **1,024 bytes**, including encoded components,
NULL markers, delimiters and collation transforms. Equality with either limit is accepted.
These are byte limits on the encoded representation, not character limits.

INSERT VALUES, INSERT SELECT, UPDATE and ON CONFLICT DO UPDATE validate final rows and write
keys before physical apply. Referential SET NULL/SET DEFAULT actions validate the final child
rows before changing either parents or children. `COHSQLE042` leaves all statement rows unchanged,
earlier successful statements intact, and an explicit transaction active. The session and wire
connection remain usable; the wire reports ExecutionFailure with the existing diagnostic carrier.
SQLSTATE 54000 is a documented mapping, not a new wire field.

[ON CONFLICT](clauses/on-conflict.md) does not size-check a proposal it discards. DO NOTHING can
skip an oversized proposal; DO UPDATE can replace one with a fitting row, including when the
proposed row or a non-arbiter unique key was oversized. Final writes still have to fit. Old-value
lock hashes, arbiter lookups and deletion tombstones do not enforce new-write limits: existing
rows can be deleted or updated to fitting replacements.

Index builds use the same key limit, including indexes for ADD CONSTRAINT and ADD COLUMN.
A failed build leaves its index unpublished and cleans up the unpublished tree. See
[CREATE INDEX](statements/create-index.md) and [constraints](constraints.md).

## DDL dependencies


| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHSQLD001` | Invalid, duplicate, recursive or volatility-incompatible definition, or absent function/domain requested by DROP; non-Boolean CHECK uses COHSQLE003 | — |
| `COHSQLD003` | DROP FUNCTION/DOMAIN targets an engine-registered overload/type | — |
| `COHSQLD002` | View dependency blocks DROP TABLE/FUNCTION/DOMAIN/VIEW RESTRICT, table rename, or drop/rename/type-facet changes to a referenced column; also ALTER TABLE RENAME retained by a declared script; names the dependent view/script | — |
| `COHSQLS005` | Declared migration script attempts DROP SCHEMA CASCADE over typed schema objects; declare the change in the typed schema; rejected before opening or writing | — |

ALTER uses `COHSQLE027` for volatile ADD COLUMN defaults on live rows (no SQLSTATE). SET NOT NULL checks stored and frozen missing values with COHSQLE024 (23502);
DROP NOT NULL on a primary key uses SQL0003. SET/DROP DEFAULT and DROP NOT NULL on identity
columns use COHSQLE025. Stable/Volatile defaults execute per assignment; subqueries remain COHDBL001.

## Database creation and open

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHSQLE041` | Storage/index or open I/O operation failed; release resources and retry after resolution | 58030 for I/O |
| `COHDBI002` | Attached-index corruption retained beside COHDBS003 and COHSQLE041 | — |
| `COHSQLC001` | Interrupted creation cannot be completed through plain open: pre-header files require a safe create/Build retry; missing sidecars, ambiguous catalogs or conflicting persisted state are preserved and refused | — |
| `COHDBS003` | Detected storage corruption during create/open or execution; DatabaseCorruptionException names the database and optional page and retains the storage/index cause | XX001 |

COHSQLC001 is an Error in DatabaseException.Diagnostics. A safely restartable pre-header open uses
DatabaseNotFoundException with that diagnostic preserved. Follow the message: ambiguous lost
catalogs/missing sidecars require preservation and complete-backup recovery; short files beside
other state cannot safely be restarted. No SQLSTATE is assigned. Transient inspection I/O asks for
an OpenDatabaseAsync retry after resolution, without restore/drop advice. Format remains 25;
this completion path does not upgrade old databases. See
[lifecycle APIs](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#database-creation-open-and-drop).

COHDBS003 fails the operation rather than omitting an unreadable row. Follow the message's advice
to restore a verified backup. Over the wire, it is ExecutionFailure and leaves the connection
usable. Legitimately reclaimed rows retain absence semantics. See
[storage recovery](../../overview.md#database-file-sets-and-lifecycle) and
[DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

Storage/index failure classification searches inner exceptions and every aggregate branch.
Corruption takes precedence over I/O; attached-index damage retains COHDBI002 and model code
COHSQLE041 beside the leading COHDBS003. Specific format/offline refusals keep their codes.
A recoverable I/O open failure publishes no database and permits retry after resolution.

## Transactions

| Code | Meaning |
| --- | --- |
| `COHSQLT001` | BEGIN/START TRANSACTION while a transaction is active |
| `COHSQLT002` | COMMIT/END/ROLLBACK without an active transaction |
| `COHSQLT003` | DDL inside an explicit transaction |
| `COHSQLT006` | Bound target table changed, was dropped or replaced during lock preparation; retry in a fresh transaction; no SQLSTATE assigned |
| `COHSQLT005` | Transaction was aborted or ended beneath its caller; later statements/commit refused with the original coded cause |

A malformed transaction spelling reports SQL0003 before changing transaction state.

Storage, I/O and index failures abort an explicit transaction; roll back before continuing.
Ordinary constraint/evaluation/search-condition failures keep statement atomicity and leave the
transaction usable. In autocommit the next statement can run after a recoverable failure.
The wire returns ExecutionFailure with a usable connection. Secondary cleanup failure keeps
the original coded exception and emits Warning event 25, TransactionCleanupFailed, on
Assimalign.Cohesion.Database.Sql, carrying transactionId, exceptionType and exceptionMessage
(bounded to 1,024 characters). An aborted-transaction refusal carries an Error diagnostic
COHSQLT005 with ordinary unprefixed Message text and the original InnerException cause.
The wire still starts with `COHSQLT005: ` exactly once. No SQLSTATE is assigned to COHSQLT005.

[ON CONFLICT](clauses/on-conflict.md) uses DatabaseTransactionAbortedException for a newer
committed Snapshot conflict or invalidated unique-index directory, and DatabaseTransactionDeadlockException
for residual cycles, including multi-row upserts reaching keys or retained rows in opposite orders.
Contended-row arbitration releases new grants before waiting, retains the wait's row grant and
retries internally. Roll back and retry a deadlock victim's entire transaction, including earlier
statements. ReadCommitted instead skips/updates the newest
committed conflict. Ordinary constraint failures and COHSQLE035/036 roll back the statement;
their wire ExecutionFailure leaves the connection usable. MySQL ON DUPLICATE KEY UPDATE,
SQLite INSERT OR IGNORE/REPLACE, unsupported inference and subqueries in conflict actions use COHDBL001.

A changed target during an object-lock wait reports COHSQLT006 through
DatabaseTransactionAbortedException. Adjacent foreign-key definition changes rebuild before row
reads; changing the target invalidates the bound plan. Roll back an explicit transaction and retry
the statement in a fresh transaction. No SQLSTATE is assigned to COHSQLT006.


## Locations and wire behavior

Remote failures use [SqlClientException](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-client/sql-client-exception.md), which derives directly from
System.Exception and declares its own Diagnostics snapshot. Catch that client exception for remote
operations; catching DatabaseException now covers only engine-side failures.

`DatabaseException.Diagnostics` is never null. Supplied diagnostics are copied, ordered by
zero-based UTF-16 start offset, with unlocated entries last and stable ordering for ties.
End offsets are exclusive; line numbers are one-based. Binding rejections retain available AST
spans. Typed requests and text execution reject parser errors before planning.

Servers compose `CODE: message` from the first Error diagnostic. Negotiated protocol 2.1 with
`diagnostics` also sends Warning/Error entries in frame 14 before Error or successful completion.
Information diagnostics are omitted. At most 4,096 entries and 1 MiB are sent. Overflow retains a
leading prefix plus `COHDBP001` with exact omission counts; it preserves the original outcome and
usable session. Negotiated 2.0 retains Error text and baseline results without that frame.

ParseFailure and ExecutionFailure end the exchange while leaving the session ready. Malformed
payloads, UTF-8, message order or Diagnostics frames are protocol violations. Typed client failures
retain the wire category, structured diagnostics and connection usability.

## See also

[Language (SQL)](index.md) · [Unsupported syntax](unsupported.md) · [CAST](expressions/cast.md) · [Parameters](expressions/parameters.md)

## Sources

- **Timeout and FK conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`, `SqlTimeoutRegressionTests.cs`, `SqlCascadeCatalogTests.cs`, `SqlForeignKeyBindingTests.cs` and `SqlReferenceLockTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Wrapped/index failures and cleanup evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/IndexFailureTranslationTests.cs`, `SqlIndexReadIntegrityTests.cs` and `StorageOpenFailureTests.cs` in the same directory.

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.
- **Stored function invocation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPersistedDefinitionTests.cs`.

- **Storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlIndexReadIntegrityTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseCorruptionTests.cs`.

- **Codes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
- **Creation and open refusals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCreateRecoveryTests.cs` and `SqlDatabaseNameTests.cs` in the same directory.
- **Spans and carriers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDiagnosticTests.cs` and `SqlDiagnosticsFixPassTests.cs` in the same directory.
- **NOT NULL** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefaultAssignmentTests.cs`.
- **Constraints and wire delivery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlConstraintDiagnosticTests.cs`, `SqlLanguageConformanceTests.cs` in the same directory, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Negotiation and bounds** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/DESIGN.md`.
