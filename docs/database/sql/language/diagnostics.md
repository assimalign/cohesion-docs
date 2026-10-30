# Diagnostics (Cohesion SQL)

SQL uses `CS` across syntax, binding, value, schema, transaction, execution and storage categories. See the [code scheme and registry](../../error-codes.md).

SQL reports coded parser, binding, planning and execution diagnostics.

> **Status:** Implemented.

## Partial-index conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03082 | A primary-key definition has a partial predicate | 42P16 |
| CS03083 | ALTER COLUMN TYPE targets a predicate column, including widening; drop and recreate the index | 0A000 |
| CS03084 | Blank or inconsistent index predicate text/dependency metadata | 42P17 |
| CS05091 | Predicate contains a prohibited expression or a call/conversion not proven immutable | 42P17 |

An unresolved predicate after a registration change lets the database open and serve reads and
DROP INDEX, but writes to its table refuse with CS01009 (42883), even with empty input.
See [partial indexes](statements/create-index.md#partial-indexes) for admission and recovery.

## Language diagnostics

| Code | Meaning |
| --- | --- |
| `CK00001` | Recognized clause, function, operator or modifier outside the executable profile |
| `CS00001` | Empty statement |
| `CS00002` | Unknown leading command |
| `CS00003` | Malformed syntax, invalid parameter form, leftover text, bare DEFAULT outside its DML positions, or explicit NULL conflicting with NOT NULL/PRIMARY KEY/IDENTITY |
| `CS00004` | Retired; unknown type names now resolve in the engine and use CS02010 |
| `CS00005` | Invalid type modifiers or rejected spelling, including TIME WITH TIME ZONE, BIT and SMALLDATETIME |
| `CS00006` | Configured expression nesting limit exceeded; default 256 |
| `CS00007` | Parsing thread has insufficient stack; engine text execution translates to CS05004 |
| `CS00008` | Malformed or overflowing typed temporal, interval or X binary literal; span covers the literal |
| `CS00009` | Information: optional terminating semicolon omitted; `CS00100` is retired and reserved |
| `CS00010` | GROUPING SETS inside ROLLUP or CUBE, including through a composite; planning refusal, SQLSTATE 42601 |

For example, `SELECT DATE '2024-02-30' FROM t;` reports CS00008 before execution.
[Ranking, navigation and value windows](functions/window-functions.md), aggregate windows, ROWS
and RANGE with offsets, GROUPS, EXCLUDE and IGNORE/RESPECT NULLS execute with OVER. A missing
OVER uses CS05076 (42P20); FROM FIRST/LAST and aggregate-window DISTINCT retain CK00001, including
over an empty table. The [42 T1 scalars](functions/builtin-functions.md) execute; incompatible signatures use
CS01006 before reading rows.
Only executable functions appear in the language profile's function table.

## Planning and execution diagnostics

A dash means the source does not assign a SQLSTATE here. SQLSTATE is a reference category;
the structured diagnostic carries Code, Message, Severity and location fields.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CS02001` | Division or remainder by zero | 22012 |
| `CS01062` | GROUPING argument does not match a grouping expression at its associated query level | 42803 |
| `CS01063` | GROUPING requires 1–31 non-star arguments | 54023 |
| `CS05074` | GROUPING outside grouped output or in grouping/aggregate inputs | 42803 |
| `CS05075` | More than 4096 expanded grouping sets before DISTINCT | 54001 |
| `CS02002` | Numeric domain/range/overflow, catalog identity or noncycling sequence exhaustion, non-finite float to integer or scalar result, temporal arithmetic overflow, negative SUBSTRING length or string result/work-buffer cap | 22003 |
| `CS02003` | Invalid operand type, including set-operation type mismatch (names output position/types), unsupported temporal arithmetic pairs, nonnumeric arithmetic, non-Boolean WHERE/HAVING/ON/CHECK/domain CHECK/CASE WHEN/ON CONFLICT WHERE/FILTER condition or AND/OR/NOT/truth-test operand | 42804 |
| `CS05004` | Statement needs more stack during parsing/planning/evaluation | 54001 |
| `CS01005` | Column reference where no columns are in scope: VALUES or LIMIT/OFFSET/FETCH/TOP | Class 42 |
| `CS01006` | No function overload accepts arity/types; unknown or incompatible temporal field value; constant/direct parameter fields fail planning, row-dependent DATE_TRUNC fields fail evaluation | 42883 |
| `CS05007` | Function or registered conversion implementation failed | 38000 |
| `CS01008` | Ambiguous overload; CAST an argument to choose | 42725 |
| `CS01009` | Missing function overload requested by DROP, or a required implementation is unavailable or incompatible | 42883 |
| `CS02010` | Unknown type/domain or unavailable stored domain | 42704 |
| `CS02011` | No conversion for a CAST pair; known pairs checked during planning | 42846 |
| `CS02012` | Character assignment exceeds length with non-space overflow | 22001 |
| `CS05013` | Invalid statement/expression/DDL/query shape; scalar/IN/ANY/SOME/ALL subquery width, derived width/name error, rename collision or incompatible DEFAULT function result type | — |
| `CS01014` | Table does not exist for reads or DML, including a missing namespace | 42P01 |
| `CS01015` | Unknown/ambiguous column, hidden/unknown/out-of-scope qualifier, new CHECK naming another table/schema, missing DDL column, or missing/repeated/ambiguous/incompatible-type USING column | 42703 for unknown columns |
| `CS01016` | Unknown function absent from executable and recognized-unsupported vocabulary | 42883 |
| `CS05017` | Invalid aggregate placement/grouping, including RETURNING or a non-grouping outer column referenced from a completed group | — |
| `CS02018` | Invalid ordering ordinal, row count or CAST declaration | — |
| `CS01019` | Duplicate INSERT/SET target column, key-list column (including FOREIGN KEY referenced columns), PRIMARY KEY declaration or table binding; ambiguous output alias | — |
| `CS01020` | Missing parameter, checked before reading or writing rows | — |
| `CS04021` | Mutation or table DDL targets a read-only system relation or persisted view | 25006 |
| `CS02022` | Invalid non-temporal CAST or stored-column assignment text | 22P02 |
| `CS01023` | Parameter keys collide after stripping one sigil, including unused keys | — |
| `CS03024` | NOT NULL column receives NULL or DEFAULT without a usable default | 23502 |
| `CS03025` | Identity definition, assignment or ALTER violates its generation contract; allocator exhaustion retains CS02002 | 428C9 (`generated_always`) for explicit ALWAYS assignments; other identity failures share the local code |
| `CS01026` | Duplicate CTE name or unresolved reference to a later sibling; binding fails before reads/writes | 42712 / 42P01 |
| `CS03027` | ADD COLUMN has a volatile DEFAULT and live rows; a physical rewrite would be required | — |
| `CS01028` | Missing namespace in schema/table/index DDL; DROP TABLE IF EXISTS remains a no-op | 3F000 |
| `CS03029` | CREATE SCHEMA finds an existing namespace without IF NOT EXISTS | 42P06 |
| `CS03030` | DROP SCHEMA RESTRICT finds dependent objects; message offers CASCADE | 2BP01 |
| `CS03031` | DROP SCHEMA targets dbo, required by code-first provisioning | — |
| `CS03033` | View declaration/replacement failure, changed bound column identity, invalid dependency/cycle, oversized record, wrong DROP VIEW target or recoverable uncoded body bind failure | — |
| `CS05032` | DISTINCT/ALL/ORDER BY on a non-aggregate function, or DISTINCT input ordering key not structurally present in the argument list | — |
| `CS02034` | LIKE ESCAPE has more than one Unicode scalar or the pattern ends in its escape; fixed/parameter patterns validate eagerly, dynamic patterns at evaluation | 22025 |
| `CS06041` | Storage/index operation failed at an engine boundary, including wrapped/aggregate causes; original cause retained | 58030 for I/O |
| `CS05043` | Server-side statement execution deadline expired; failed statement effects are rolled back | 57014 (`query_canceled`) |
| `CS04044` | Individual contended lock wait expired, including unique-index key waits and RewriteTableAsync's pre-queue exclusive table lock | 55P03 (`lock_not_available`) |
| `CS05045` | Uncoded result encoding/preparation failure, or an encoded header/row above the wire frame limit; see [result delivery](#result-encoding-and-delivery) | 54000 for size refusals; otherwise — |
| `CS05042` | Encoded row, index key or catalog definition exceeds its size limit, or catalog DDL generation is exhausted | 54000 (`program_limit_exceeded`) |
| `CS05040` | Invariant LIKE/NOT LIKE exceeds its per-match work budget; located ExecutionFailure leaves the connection usable | — |
| `CS03035` | ON CONFLICT lacks a matching unique arbiter, names a nonunique/missing target, or DO UPDATE omitted its target; planning failure | 42P10 |
| `CS05036` | ON CONFLICT repeats an inserted/updated row; MERGE selects a second modification of one physical target, including after rechecks | 21000 |
| `CS03037` | UNIQUE, PRIMARY KEY or unique-index violation, including a second unique index during ON CONFLICT DO UPDATE | 23505 |
| `CS03038` | Table CHECK or domain-check violation on a write, CAST to a domain, or function domain argument/result | 23514 |
| `CS03039` | FOREIGN KEY violation: missing parent on INSERT/UPDATE or a referenced key protected by NO ACTION/RESTRICT on DELETE/UPDATE, including final-row checks | 23503 |
| `CS02046` | Scalar subquery returns more than one row | 21000 (`cardinality_violation`) |
| `CS02047` | Malformed temporal text in assignment, DEFAULT or CAST | 22007 (`invalid_datetime_format`) |
| `CS02048` | Calendar, clock or offset field out of range in assignment, DEFAULT or CAST | 22008 (`datetime_field_overflow`) |
| `CS03049` | Table or index name already exists, including provisioning collisions | 42P07 (`duplicate_table`) |
| `CS03050` | Constraint or catalog non-function SQL definition name already exists; SQL CREATE DOMAIN duplicates retain CS03301 | 42710 (`duplicate_object`) |
| `CS01051` | Missing table during execution/DDL | 42P01 (`undefined_table`) |
| `CS01052` | Missing constraint, index, collation or other catalog object; missing schemas retain CS01028 / 3F000 | 42704 (`undefined_object`) |
| `CS03053` | Invalid table definition, including nullable primary key or dropping the last column | 42P16 (`invalid_table_definition`) |
| `CS03054` | Invalid FOREIGN KEY column lists or referenced key is not unique | 42830 (`invalid_foreign_key`) |
| `CS02055` | Incompatible store value, DEFAULT, FOREIGN KEY column type, CHECK function result or compared values | 42804 (`datatype_mismatch`) |
| `CS03056` | DDL refused because constraints or indexes depend on the object; namespace RESTRICT retains CS03030 | 2BP01 (`dependent_objects_still_exist`) |
| `CS05057` | Unsupported execution/DDL operation, including a subquery encountered by CHECK admission | 0A000 (`feature_not_supported`) |
| `CS06058` | Damaged or incompatible loaded persisted expression or missing-value literal | XX001 (`data_corrupted`) |
| `CS03059` | Expression cannot be stored as a valid deterministic row definition | 42P17 (`invalid_object_definition`) |
| `CS05060` | Executor/catalog invariant failure, including unavailable constraint indexes or freshly rendered CREATE VIEW text that cannot reload | XX000 (`internal_error`) |
| `CS05068` | UPDATE target has multiple source associations; before SET evaluation or writes, even for identical source values | 21000 (`cardinality_violation`) |
| `CS05069` | Direct RIGHT, FULL or unaliased parenthesized source in UPDATE FROM / DELETE USING; before reads or writes; derived SELECT containing RIGHT or FULL executes | — |
| `CS05070` | FULL JOIN lacks a safe mandatory hash equality or proven non-throwing ON evaluation; before either input is read, even when empty | — |
| `CS05072` | FILTER on a scalar/non-aggregate call | 42809 |
| `CS05073` | Same-scope aggregate inside a FILTER condition | 42803 |
| `CS03061` | Column already exists | 42701 (`duplicate_column`) |
| `CS02062` | INTERVAL duration overflow in assignment, DEFAULT or CAST | 22015 (`interval_field_overflow`) |
| `CS02063` | FOREIGN KEY columns have incompatible collations | 42P21 (`collation_mismatch`) |
| `CS04064` | DDL target replacement, TRUNCATE closure change, or MERGE target recheck/apply-time modified-row version conflict (including Snapshot stale targets); retry the statement or aborted transaction | 40001 (`serialization_failure`) |
| `CS03067` | TRUNCATE RESTRICT omits a referencing table, even an empty one | 0A000 (`feature_not_supported`) |
| `CS04066` | TRUNCATE inside an explicit transaction; prior work and active state kept | 25001 (`active_sql_transaction`) |
| `CS03069` | Invalid resolved sequence type/options or restart/current value | 22023 |
| `CS03070` | Occupied sequence/table/view relation name involving a sequence; existing kind named | 42P07 |
| `CS03071` | Sequence namespace does not exist | 3F000 |
| `CS03072` | Encoded sequence definition exceeds 8,092 bytes | 54000 |
| `CS03073` | Named sequence or object identity is absent | 42P01 |
| `CS03074` | Unsupported persisted sequence operation/REGCLASS (constant column allocation defaults execute), or sequence DDL in a declared provisioning script | 0A000 |
| `CS03075` | setval value is outside the sequence bounds | 22003 |
| `CS03076` | Unsupported sequence evaluation context, including NEXT VALUE FOR in RETURNING, or detached evaluation | 0A000 |
| `CS03077` | Malformed sequence reference or invalid runtime call argument | 22023 |
| `CS04067` | Dropped/replaced sequence incarnation used for allocation or mutation | 40001 |
| `CS04068` | currval is undefined for this sequence in this session | 55000 |
| `CS04069` | lastval is undefined or its sequence was dropped | 55000 |
| `CS04070` | DISCARD ALL inside an explicit transaction | 25001 |
| `CS06061` | Malformed sequence definition, directory or reservation | XX001 |
| `CS03065` | Function signature already exists | 42723 (`duplicate_function`) |
| `CS03066` | Compiled-schema ownership refuses session DDL | 42501 (`insufficient_privilege`) |

Each SqlConstraintViolationException carries exactly one Error diagnostic. Messages name the
constraint and schema-qualified table without key values; domain CAST and function checks name
the domain without a table. The session attaches the enclosing statement span when available;
construction without source text leaves the diagnostic unlocated. NOT NULL retains CS03024
(23502), now with the statement span when source text is available. SQLSTATE is a documented mapping,
not a separate field on the diagnostic or SQL exception.

[Predicate precedence](expressions/operators.md) rejects non-associative chains such as `a < b < c`
with CS00003 at the second operator. Completed IN lists/subqueries may chain. ILIKE, SIMILAR TO,
ISNULL, NOTNULL, IS JSON/OF/NORMALIZED, binary bitwise operators and general `::` casts use CK00001;
the sequence-specific `name::regclass` form executes.
Invalid fixed LIKE patterns fail binding even on empty input or with a NULL matched value;
this eager check intentionally differs from PostgreSQL. Truth tests require BOOLEAN or NULL.

[Set operations](clauses/set-operations.md) use CS05013 for unequal widths, CS02018 for
combined ORDER BY expressions/qualified names/invalid ordinals, and CS01019 for ambiguous
output names. CORRESPONDING uses CK00001. [Schema DDL](statements/create-schema.md) rejects
AUTHORIZATION and nested schema elements with CK00001. DROP revalidation after a concurrent
dependency change uses CS05013; schema ownership retains DatabaseObjectLockedException.

For example, `SELECT t.id FROM t AS x;` reports located CS01015 because the alias hides t.
NOT NULL and other column-assignment failures retain the statement span when source text is available.
Correlated references resolve through enclosing query scopes. A real derived sibling reference
uses `CK00001` naming LATERAL; unknown, ambiguous or hidden names retain `CS01015`.
An aggregate assigned to an outer level uses `CK00001` before grouped-column validation.
For WITH, a later sibling's name reads a stored table when one exists; only a missing future
relation receives `CS01026`. The documented SQLSTATE mappings are 42712 for duplicate CTE names and 42P01 for missing later-sibling relations.

[Aggregate modifiers and text concatenation](functions/aggregate-functions.md) use CS00003 for
quantified star or malformed LISTAGG ordering, CS01006 (42883) for arity/type mismatch,
CS02002 (22003) for output above 1,048,576 UTF-16 units, and CK00001 for nonconstant
delimiters or ON OVERFLOW. CS05032 has no assigned SQLSTATE.

The [LIKE work limit](expressions/operators.md#like-and-escape) is
`64 × (input UTF-16 length + 1) × (raw pattern UTF-16 length + 1)` with checked 64-bit arithmetic.
CS05040 refuses the match rather than estimating TRUE/FALSE; cancellation takes precedence.

Non-Boolean search conditions use CS02003 (42804) before reading even an empty input when
the type is known, and during evaluation otherwise. CASE/COALESCE alternatives in Boolean
positions all qualify or fail, including unreachable alternatives. Untyped NULL is UNKNOWN;
valid Boolean input string literals coerce, while text columns/parameters keep their types.
Stored SQL function bodies with legacy invalid Boolean operands or changed application return
types allow the database to open and fail with CS02003 when invoked.

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

CS05043 (57014) and CS04044 (55P03) roll back only the failed statement's effects. The session
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


## Result encoding and delivery

Each encoded result header and each row must fit **16 MiB (16,777,216 bytes)**. Equality is
accepted. There is no aggregate encoded-result cap. Sizes include encoding and framing payload
overhead, including UTF-8 bytes and zero escapes; a size refusal reports encoded bytes and the
frame limit. These are wire payload limits, separate from stored-row and index-key limits below.

Writing statements encode and validate all result frames before commit or explicit-transaction
statement completion. DML result enumeration and disposal finish inside its undo bracket, including
INSERT SELECT, ON CONFLICT, UPDATE, DELETE and RETURNING. A result failure rolls back only the
statement, retaining earlier explicit-transaction work. DDL and transaction control prepare their
fixed completion before execution or commit. No result frame precedes a write-preparation failure.

Read-only SELECT, EXPLAIN, system views and SHOW encode during delivery after execution has
materialized the result. A late encoding, enumeration or disposal failure ends the exchange with
ExecutionFailure, even after partial result frames. The client throws and discards all partial
columns and rows. Both paths preserve the connection and explicit transaction on result failures.
Already-coded failures retain their diagnostics; other supported result failures use CS05045.
SQLSTATE is a reference mapping, not a new wire field or exception property.

Writes retain prepared payloads until delivery; reads retain only the current encoded frame,
in addition to materialized values. Embedded execution allocates no wire buffers. A transport
failure during delivery is terminal; a committed write is never reported as an ordinary
statement failure. See [result exchanges](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-protocol/design.md#statement-results)
and the [client recovery example](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-client/sql-connection.md#result-failure-example).

## Row and index-key limits

An encoded row must fit **8,092 bytes**, including its MVCC header and physical column layout.
Every unique or nonunique index key must fit **1,024 bytes**, including encoded components,
NULL markers, delimiters and collation transforms. Equality with either limit is accepted.
These are byte limits on the encoded representation, not character limits.

INSERT VALUES, INSERT SELECT, UPDATE and ON CONFLICT DO UPDATE validate final rows and write
keys before physical apply. Referential SET NULL/SET DEFAULT actions validate the final child
rows before changing either parents or children. `CS05042` leaves all statement rows unchanged,
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

Unique builds finish construction before checking adjacent current live keys in bounded batches.
CS05042 (54000) during construction can therefore take precedence over an earlier duplicate
in storage order (CS03037, 23505). Inline CREATE UNIQUE INDEX reports the smallest duplicated
live key in encoded index order. A refused ALTER leaves the old schema, constraints and rows
intact; unpublished indexes are removed. No new diagnostic code is introduced by bounded validation.

## DDL dependencies


| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CS03301` | Invalid, duplicate domain, recursive or volatility-incompatible definition, or absent domain requested by DROP; duplicate functions use CS03065 and missing functions CS01009; non-Boolean CHECK uses CS02003 | — |
| `CS03303` | DROP FUNCTION/DOMAIN targets an engine-registered overload/type | 0A000 |
| `CS03302` | View dependency blocks DROP TABLE/FUNCTION/DOMAIN/VIEW RESTRICT, table rename, or drop/rename/type-facet changes to a referenced column; also ALTER TABLE RENAME retained by a declared script; names the dependent view/script | 2BP01 |
| `CS03205` | Declared migration script attempts DROP SCHEMA CASCADE over typed schema objects; declare the change in the typed schema; rejected before opening or writing | — |

ALTER uses `CS03027` for volatile ADD COLUMN defaults on live rows (no SQLSTATE). SET NOT NULL checks stored and frozen missing values with CS03024 (23502);
DROP NOT NULL on a primary key uses CS00003. SET/DROP DEFAULT and DROP NOT NULL on identity
columns use CS03025. Stable/Volatile defaults execute per assignment; subqueries remain CK00001.

## Database creation and open

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CS06041` | Storage/index or open I/O operation failed; release resources and retry after resolution | 58030 for I/O |
| `CK06102` | Attached-index corruption retained beside CK06003 and CS06041 | — |
| `CS03401` | Interrupted creation cannot be completed through plain open: pre-header files require a safe create/Build retry; missing sidecars, ambiguous catalogs or conflicting persisted state are preserved and refused | — |
| `CK06003` | Detected storage corruption during create/open or execution; DatabaseCorruptionException names the database and optional page and retains the storage/index cause | XX001 |

CS03401 is an Error in DatabaseException.Diagnostics. A safely restartable pre-header open uses
DatabaseNotFoundException with that diagnostic preserved. Follow the message: ambiguous lost
catalogs/missing sidecars require preservation and complete-backup recovery; short files beside
other state cannot safely be restarted. No SQLSTATE is assigned. Transient inspection I/O asks for
an OpenDatabaseAsync retry after resolution, without restore/drop advice. Format remains 25;
this completion path does not upgrade old databases. See
[lifecycle APIs](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/design.md#database-creation-open-and-drop).

CK06003 fails the operation rather than omitting an unreadable row. Follow the message's advice
to restore a verified backup. Over the wire, it is ExecutionFailure and leaves the connection
usable. Legitimately reclaimed rows retain absence semantics. See
[storage recovery](../../overview.md#database-file-sets-and-lifecycle) and
[DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

Storage/index failure classification searches inner exceptions and every aggregate branch.
Corruption takes precedence over I/O; attached-index damage retains CK06102 and model code
CS06041 beside the leading CK06003. Specific format/offline refusals keep their codes.
A recoverable I/O open failure publishes no database and permits retry after resolution.

## Transactions

| Code | Meaning |
| --- | --- |
| `CS04001` | BEGIN/START TRANSACTION while a transaction is active |
| `CS04002` | COMMIT/END/ROLLBACK without an active transaction |
| `CS04003` | DDL inside an explicit transaction |
| `CS04006` | Bound target changed/dropped/replaced, or DML/foreign-key work needs a retired layout after rewrite; retry the whole transaction in a fresh snapshot; no SQLSTATE assigned |
| `CS04005` | Transaction was aborted or ended beneath its caller; later statements/commit refused with the original coded cause |
| `CS04065` | Hard-deadlock lock refusal; SQLSTATE `40P01` (`deadlock_detected`); caller rolls back the victim transaction |

A malformed transaction spelling reports CS00003 before changing transaction state.

Storage, I/O and index failures abort an explicit transaction; roll back before continuing.
Ordinary constraint/evaluation/search-condition failures keep statement atomicity and leave the
transaction usable. In autocommit the next statement can run after a recoverable failure.
The wire returns ExecutionFailure with a usable connection. Secondary cleanup failure keeps
the original coded exception and emits Warning event 25, TransactionCleanupFailed, on
Assimalign.Cohesion.Database.Sql, carrying transactionId, exceptionType and exceptionMessage
(bounded to 1,024 characters). An aborted-transaction refusal carries an Error diagnostic
CS04005 with ordinary unprefixed Message text and the original InnerException cause.
The wire still starts with `CS04005: ` exactly once. No SQLSTATE is assigned to CS04005.

[ON CONFLICT](clauses/on-conflict.md) uses DatabaseTransactionAbortedException for a newer
committed Snapshot conflict or invalidated unique-index directory, and DatabaseTransactionDeadlockException
for residual cycles, including multi-row upserts reaching keys or retained rows in opposite orders.
Contended-row arbitration releases new grants before waiting, retains the wait's row grant and
retries internally. Roll back and retry a deadlock victim's entire transaction, including earlier
statements. Hard-deadlock failures carry `CS04065` / SQLSTATE `40P01` on embedded and wire paths.
The refusal does not roll back an explicit victim transaction: statement atomicity retains its
earlier work and locks until the caller rolls it back, allowing surviving waiters to proceed.
ReadCommitted instead skips/updates the newest
committed conflict. Ordinary constraint failures and CS03035/CS05036 roll back the statement;
their wire ExecutionFailure leaves the connection usable. MySQL ON DUPLICATE KEY UPDATE,
SQLite INSERT OR IGNORE/REPLACE, unsupported inference and subqueries in conflict actions use CK00001.

A changed target during an object-lock wait reports CS04006 through
DatabaseTransactionAbortedException. Adjacent foreign-key definition changes rebuild before row
reads; changing the target invalidates the bound plan. Roll back an explicit transaction and retry
the statement in a fresh transaction. No SQLSTATE is assigned to CS04006.


## Locations and wire behavior

Execution, constraint and DDL failures carry stable codes independently of their message text.
The session preserves a precise location from the current statement, or attaches its enclosing
statement span to an unlocated failure. Located offsets always fall inside the statement text.
Stored CHECK/DEFAULT/function fragments and synthetic schema-provisioning expressions lose their
foreign offsets before that fallback, even when those offsets numerically fit the current text.
The parser's root span may omit a SELECT query tail; this does not promise a full-text span.
Open-time failures have no executing statement. DEFAULT wrappers preserve the underlying code and
cause. Already-translated storage failures retain their exception identity and may stay unlocated.

For example, execute `CREATE TABLE required_value (v INT NOT NULL);`, then
`INSERT INTO required_value VALUES (NULL);`. The insert reports CS03024 (23502), names `v`,
and carries the insert's span. A wire client receives `CS03024: message` once, and can use the
same connection for `SELECT 1;`. Previously uncoded execution/DDL failures now also have a
a complete `CS` code followed by a colon wire prefix. No frame or protocol-version change accompanies these codes.

Embedded SQL command and row results expose a non-null DatabaseResult.Diagnostics collection,
empty when nothing is reported. Remote SqlResultSet.Diagnostics and SqlClientException.Diagnostics
are also never null, including when no diagnostics capability was negotiated. Empty diagnostics
do not cause a Diagnostics frame to be sent.

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
leading prefix plus `CK07001` with exact omission counts; it preserves the original outcome and
usable session. Negotiated 2.0 retains Error text and baseline results without that frame.

ParseFailure and ExecutionFailure end the exchange while leaving the session ready. Malformed
payloads, UTF-8, message order or Diagnostics frames are protocol violations. Typed client failures
retain the wire category, structured diagnostics and connection usability.

## See also

[Language (SQL)](index.md) · [Unsupported syntax](unsupported.md) · [CAST](expressions/cast.md) · [Parameters](expressions/parameters.md)

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.Sql` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 26 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

The server drains complete shared terminal Errors and capacity refusals within its configured
ShutdownDrainTimeout, with TerminalDrainLinger bounding peer close after output completion.
TLS delivery remains limited by #1086. See [server options](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-server-options.md).

## Maintenance worker events

Provider `Assimalign.Cohesion.Database` reports maintenance lane delays for this model:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 35 | WorkerLaneDelayed | Warning | workerName, workerKind, database, reason, elapsedSeconds |
| 36 | WorkerLaneDisposalWaiting | Warning | workerName, workerKind, database, elapsedSeconds |

Event 35 names `capacity` deferral (zero elapsed time) or a `stuck` operation (its runtime),
limited to once per second per database/worker. The accompanying worker failure is observational:
engine Faulted can coexist with a serving database, and the report does not advance the offline
policy or replace an unresolved operational fault. Event 36 repeats once per second for each
running lane while disposal waits, measuring elapsed disposal time. Storage stays open until I/O
returns. These events add no diagnostic code or SQLSTATE. See [lane limits and worker health](../../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).

## Failure tracing

The shared StatementFailed event takes its payload code from non-null Diagnostics: first Error, otherwise first entry, otherwise empty. It does not extract codes from messages or include statement/message text. Offline and unconfirmed-commit failures carry their model code in that carrier. See [failure-event selection](../../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#structured-failure-event-codes).

## Hash-build refusal

CS05067 reports exhausted statement memory for hash builds, retained joined-DML state or CTAS source/copy buffers
(SQLSTATE 53200, out_of_memory). Its diagnostic message is unprefixed. Configure
SqlDatabaseEngineOptions.StatementMemoryBudget; there is no spill. See
[hash joins](clauses/join.md#hash-joins-and-memory) and [CTAS](statements/create-table.md#create-table-as).

## CREATE TABLE AS conditions

CTAS adds CS03068 for missing, blank, duplicate or wrong-width output names (42601, syntax_error).
It reuses CS03049 for destination collisions (42P07), CS04003 for explicit transactions (25001),
and CS05067 for its retained source/copy memory budget (53200). A guarded name collision returns
success with zero affected rows.

## MERGE conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01053 | merge_action() outside MERGE RETURNING | — |
| CS05071 | WHEN follows an unconditional clause of the same match kind, including DO NOTHING | — |
| CS05070 | FULL MERGE association lacks safe hash equality or a complete ON safety proof | — |

[MERGE](statements/merge.md) executes ordered actions and RETURNING atomically. CS05036 (21000)
refuses repeated physical target modifications, including after target rechecks. Direct/referential
overlaps compose without that refusal. merge_action arguments or
modifiers use CS01006 (42883). Scope, assignment, parameter and identity failures retain their
ordinary codes. No SQLSTATE is assigned to the new CS01053 or CS05071 conditions.

ReadCommitted locks and rechecks a concurrently committed modifying target against the original
ON and ordered WHEN clauses. Snapshot refuses it with retryable CS04064 (40001). In MERGE's
row-concurrency path, CS04064 covers target recheck and apply-time modified-row version conflicts;
cascade snapshot crosschecks, referenced-parent version checks, host-ended transactions and
abandoned owners retain uncoded transaction aborts. Roll back and retry the aborted transaction.

Direct-proposal uniqueness (CS03037, 23505) precedes final-state FK failures (CS03039, 23503)
and NOT NULL/CHECK/domain/identity failures from referential SET actions. An UPDATE removed by
a cascade still checks surviving children of its old parent key. A retained constraint failure
skips further parent-row locking, preserving that diagnostic even if a referenced parent is stale.
Wire constraint/RETURNING failures undo only the statement and keep the connection usable;
wire BEGIN's default Snapshot target conflict requires ROLLBACK.

## ALTER COLUMN TYPE conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS02064 | Old value, USING result or dependent default has no assignment/implicit cast to the target | 42804 |
| CS05013 | Full-definition ALTER changes the resulting default, or changes collation without explicit COLLATE | — |

[TYPE](statements/alter-table.md#alter-column-type) reuses CS03025 for invalid identity targets/
ranges, CS03302 (2BP01) for dependent views, CS02055 (42804) and CS02063 (42P21) for FK type/
collation mismatch, CS03037 (23505) for rebuilt unique-key collisions, CS03038 (23514) for CHECK/
domain failures and CS03039 (23503) for invalid final FK keys. NOT NULL retains CS03024 (23502).
Lock waits use CS04044 (55P03); stale writers after publication use CS04006 (no SQLSTATE).
Conversions retain their ordinary numeric/text/temporal codes. USING subqueries/aggregates/
windows retain CK00001. DDL remains self-committing with CS04003 (25001) in explicit transactions.

## Catalog metadata conditions

CS06059 reports malformed persisted SQL catalog metadata (XX001); CS06060 reports an unsupported catalog metadata format/version (0A000). Shared CK identities cover common session, transaction, database-presence and value-access conditions; see the [registry additions](../../error-codes.md).

## Joined DML conditions

UPDATE ... FROM and DELETE ... USING execute. CS05068 (21000, cardinality_violation) rejects
multiple source associations for one UPDATE target before SET evaluation or any write, even
when the source values agree. DELETE instead deduplicates physical targets. CS05068 is a
statement failure: earlier explicit work and the client connection remain usable.

UPDATE FROM, DELETE USING and MERGE retain CS05067 (53200) for hash builds, target identities,
source/intermediate buffers, associations, selected values, encoded replacements and joined
cascade state. Exhaustion refuses before any write; reservations release on every exit. Constraints and
RETURNING retain their ordinary codes and undo the whole statement. Concurrent committed target
changes raise DatabaseTransactionAbortedException through the existing first-updater-wins
check, preserving physical target identity. Optional SELECT/plain EXPLAIN cost probes that
observe committed definition changes retain the bound path; mutation execution retains its
mandatory CS04006 definition guard (no SQLSTATE).

[Quantified comparisons](expressions/quantified-comparisons.md) reuse CS05013 for single-column
shape, CS01015 for binding and CS02055 (42804) for incompatible compared non-NULL values;
no new code is allocated.

## WITH RECURSIVE conditions

[WITH RECURSIVE](clauses/with.md) binds all list names in dependency order. The non-recursive
term fixes output width, types, modifiers, domains and collations; use the cast or COLLATE hint
to preserve that contract. Recursive UNION ALL/DISTINCT fixpoints execute on demand through shared
readers. Single-pass scans reclaim consumed history; retained rows, DISTINCT state and consumer
scratch share StatementMemoryBudget. All definitions bind, but unused definitions do not execute.
Mutual recursion, recursive UNION ORDER BY/LIMIT/FETCH/OFFSET, SELECT locking tails and WITH
RECURSIVE ... MERGE remain CK00001. Plain WITH ... MERGE executes. No format changes apply.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01054 | Recursive definition lacks top-level anchor UNION [ALL] recursive term | 42P19 |
| CS01055 | Self-reference in the non-recursive term | 42P19 |
| CS01056 | Self-reference inside a scalar, IN, quantified or EXISTS subquery | 42P19 |
| CS01057 | Self-reference on the nullable side of an outer join | 42P19 |
| CS01058 | Self-reference under INTERSECT ALL | 42P19 |
| CS01059 | Self-reference on either side of EXCEPT ALL or the right of EXCEPT DISTINCT | 42P19 |
| CS01060 | Recursive term does not have exactly one self-reference | 42P19 |
| CS01061 | Aggregate at the query level containing the self-reference | 42P19 |
| CS02065 | Anchor, recursive output or declared column-list width mismatch | 42601 |
| CS02066 | UNION changes the anchor type, domain or modifiers; cast hint | 42804 |
| CS02067 | UNION changes or conflicts with anchor collation; COLLATE hint | 42P21 |
| CS05067 | Retained recursive tables, buffers, DISTINCT state or consumer scratch exceed statement memory | 53200 |
| CS05090 | Internal invariant: a CTE opens more readers than its plan counted | XX000 |

## SQL window functions

See [OVER and WINDOW](clauses/window.md) for placement, inheritance and memory rules.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00011 | Window argument count or call modifier | 42601 |
| CS01064 | Duplicate WINDOW name | 42P20 |
| CS01065 | Unknown/self/forward window reference | 42704 |
| CS01066 | Copied PARTITION BY, overridden ORDER BY or copying an explicit frame | 42P20 |
| CS02068 | Non-integer offset or bucket count | 42804 |
| CS02069 | Incompatible navigation value/default types | 42804 |
| CS02070 | Noninteger ROWS frame offset | 42804 |
| CS02071 | NULL frame offset | 22004 |
| CS02072 | Negative frame offset | 22013 |
| CS02073 | Noninteger NTH_VALUE index | 42804 |
| CS02074 | Unsupported offset RANGE ordering-key domain | 0A000 |
| CS02075 | Incompatible RANGE offset domain | 0A000 |
| CS02076 | Negative or NaN RANGE offset | 22013 |
| CS05076 | Window placement or missing OVER | 42P20 |
| CS05077 | Nested window | 42P20 |
| CS05078 | Nonpositive NTILE count | 22014 |
| CS05079 | Window state exceeds statement memory grant | 54000 |
| CS05080 | Invalid frame boundary categories | 42P20 |
| CS05081 | Row variable, aggregate or window in frame offset | 42P20 |
| CS05082 | Nonpositive NTH_VALUE index | 22016 |
| CS05083 | OVER on a scalar function | 42809 |
| CS05084 | Application/order-sensitive or restart-path frame recomputation exceeds 1,000,000 candidate visits per statement | 54000 |
| CS05086 | Offset RANGE has other than one ORDER BY key | 42P20 |
| CS05087 | GROUPS without ORDER BY | 42P20 |
| CS05088 | NULL treatment on a function other than LAG/LEAD/FIRST_VALUE/LAST_VALUE/NTH_VALUE | 42601 |

## Identity integration

[Identity columns](clauses/identity.md) use owned sequences and early-bound column defaults.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03078 | ADD GENERATED requires NOT NULL | 55000 |
| CS03079 | Column has no identity for the ALTER operation | 55000 |
| CS03080 | Sequence required by identity or column default | 2BP01 |
| CS03081 | Invalid OWNED BY target or identity ownership change | 55000 |

Allocator exhaustion retains CS02002 (22003). A disappearing identity incarnation or an unplanned
live referential sequence-default dependency uses retryable CS04067 (40001); retry to replan.
Backfill memory exhaustion uses CS05067 (53200) before publication.

## Foreign-key update declarations

ON UPDATE CASCADE, SET NULL and SET DEFAULT execute on stored-key changes. RESTRICT is immediate;
NO ACTION checks the statement-final state.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05085 | Different referential-action origins propose conflicting replacements for one column | 27000 |

All closures, including plain DELETE CASCADE, charge StatementMemoryBudget (CS05067 / 53200).
Duplicate/incomplete action syntax retains CS00003. Invalid catalog actions use CS06059 (XX001),
unsupported table extensions CS06060 (0A000), and earlier catalog formats CS06041. Compiled v8 artifacts
are refused with InvalidDocument at schema.format. See [constraints](constraints.md#on-update-declarations).

## Window frames and aggregate execution

[ROWS, RANGE offsets, GROUPS and EXCLUDE](clauses/window.md) execute, with NULL treatment for
navigation and frame value functions. Safe native moving aggregates run in O(n) work.
Application/order-sensitive aggregates retain CS05084; volatile or subquery arguments/FILTER
use restart semantics and that fallback can also reach CS05084 for native aggregates. Visits
count before FILTER/arguments across functions, partitions and stages, including under LIMIT.
Equal cached frames spend none. ANALYZE reports frame_visits, transitions, inverse_transitions
and boundary_visits. CS05079 covers retained window state; admitting earlier CTE/subquery caches
can use CS05067 (53200). See the linked memory section for default-grant fixture limits.

## SQL transaction isolation

[SET TRANSACTION](statements/set-transaction.md) changes the current block before its first admitted data statement,
including plain EXPLAIN and failed data/DDL attempts. Parse/control failures leave configuration
open. READ UNCOMMITTED maps to ReadCommitted; REPEATABLE READ maps to Snapshot, fixed at the
original BEGIN even after early SET. This differs from PostgreSQL's first-query snapshot.
Outside-block SET changes no default, including for SERIALIZABLE. Refusals preserve the block.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00012 | Malformed or incomplete transaction isolation mode | 42601 |
| CS04071 | SET outside a block: warning and successful no-op | 25000 |
| CS04072 | SET after the first admitted data statement or during an operation | 25001 |
| CS04073 | SERIALIZABLE in a state-valid BEGIN or active usable block | 0A000 |

## Sources

- **Batch 14** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `c3dc498bf`, checked against the batch-14 code and tests.

- **Batch 13** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `4ef359bad`, checked against code and tests.

- **Batch 12** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `4607aa81f`, checked against code and test cases.

- **Batch 11** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `ae48b04cb`, checked against the executable tests.

- **Execution/DDL corpus and span provenance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExecutionDiagnosticConformanceTests.cs`, `SqlExecutionDiagnosticTests.cs` and `SqlExecutionDiagnosticInterleavingTests.cs` in the same directory.
- **Both wire transports** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlExecutionDiagnosticWireTests.cs`.
- **Unique-build validation order and cleanup** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUniqueIndexBuildTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
- **Result preparation and wire delivery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlResultDeliveryConformanceTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlResultDeliveryWireConformanceTests.cs`.
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
