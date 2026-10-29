# Diagnostics

Documents uses `CD` across syntax, binding, value, schema, transaction, execution, storage, protocol and lifecycle categories. See the [code scheme and registry](../../error-codes.md).

OQL reports coded parser, planner and execution failures with available source locations.

## Parser codes

| Code | Meaning |
| --- | --- |
| `CK00001` | Recognized form outside the executable SQL++ subset |
| `CD00001` | Empty/comment-only statement |
| `CD00002` | Malformed syntax, identifier, path, parameter, literal LIKE escape or multiple statements |
| `CD00003` | Unterminated string, quoted identifier or block comment |
| `CD00004` | Malformed numeric literal syntax |
| `CD00005` | More than 128 recursive expression parse levels |
| `CD00006` | Invalid function arity or star operand; only COUNT accepts star |
| `CD00007` | Invalid LIMIT/OFFSET count at its operand, during parsing or planning; NULL is invalid |
| `CD00008` | Unknown function name; span covers its name after arguments are parsed |
| `CD00010` | Duplicate name in one FROM/UNNEST or quantifier binding list; span covers the repeated name or inferred final path step |
| `CD00009` | Index exceeds 32 key paths; located at the 33rd path |
| `CD00011` | ALL/DISTINCT on a scalar call or star, including COUNT(ALL *) and COUNT(DISTINCT *) |
| `CD00012` | GROUP AS without preceding GROUP BY terms; at GROUP; no assigned SQLSTATE |
| `CD00013` | NULLS without FIRST or LAST; at the invalid option or end of input; no assigned SQLSTATE |

CD00014 reports a direct IN subquery without SELECT RAW/VALUE (no assigned SQLSTATE).
See [subqueries](expressions/subqueries.md) for evaluation, CD05015 depth and CD05018 memory limits.

Numeric tokens retain their digits in a JsonElement. Valid syntax that is out of decimal range,
underflows, or loses nonzero fractional digits fails during planning/evaluation as CD02007,
rather than being rounded by the parser.

## Engine codes

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `CD05001` | Invalid statement, index definition, expression, metadata or grouping context | — |
| `CD01002` | Collection does not exist | — |
| `CD01003` | Duplicate projected name; supply distinct aliases | — |
| `CD04004` | System collection is read-only | — |
| `CD01005` | Referenced parameter has no value | — |
| `CD02006` | Division or remainder by zero | 22012 |
| `CD02007` | Numeric value cannot fit decimal exactly, scalar overflow, or string result/work buffer exceeds 1,048,576 UTF-16 units | 22003 |
| `CD02008` | Invalid operand/parameter type, document key or JSON value | — |
| `CD01009` | Caller keys collide after one leading parameter sigil is stripped | — |
| `CD03010` | CREATE COLLECTION finds an existing collection without IF NOT EXISTS | 42P07 |
| `CD03011` | DROP COLLECTION targets a schema-owned collection, including IF EXISTS | — |
| `CD03012` | INSERT finds an existing key, or INSERT/UPSERT repeats a key in one statement | — |
| `CD02013` | Evaluated LIKE escape is invalid or pattern ends in its escape character | — |
| `CD01014` | SELECT DISTINCT ORDER BY key is absent from the projection after alias rewriting; reported at that key | — |
| `CD05015` | SELECT/RETURNING object/array constructor result exceeds 256 levels, including nested values and a named projection wrapper; reported at the constructor | — |
| `CD01026` | Empty RETURNING output name in a directly constructed AST; before mutation | — |
| `CD01027` | Empty variable name in a directly constructed ARRAY/FIRST comprehension; before data access | — |
| `CD01029` | Invalid aggregate quantifier enum, scalar quantifier or quantified star in a directly constructed call AST | — |
| `CD01030` | Invalid/empty/duplicate grouping alias, collision with FROM/UNNEST, or malformed direct-AST alias list | — |
| `CD01031` | GROUP AS referenced in an aggregate's row input | — |
| `CD01032` | Direct ordering AST has an invalid null-placement enum | — |
| `CD05018` | Accounted SELECT/group staging, ordered candidate buckets/keys/identities, wrappers/arrays, aggregates, projection/order storage, DML pending buffers or RETURNING exceeds StatementMemoryBudget; pending-buffer exhaustion precedes writes, no partial result | — |
| `CD05019` | Serialized RETURNING value exceeds 256 levels, including a named wrapper; inside the transaction | — |
| `CD02016` | Invalid UPDATE target, root replacement, UNSET ending in an index, incompatible traversal, missing array or out-of-range index | — |
| `CD01017` | Duplicate or prefix-overlapping UPDATE targets after alias normalization, including SET/UNSET overlap | — |
| `CD02018` | Patched UPDATE JSON is invalid or exceeds storage depth 128 | — |
| `CD02019` | Scalar argument outside its domain: negative SQRT, invalid POWER domain, or fractional ROUND/TRUNC scale | — |
| `CD03020` | Duplicate valued scalar UNIQUE key during a write/build | 23505 |
| `CD02021` | Array/object UNIQUE key component | — |
| `CD01023` | Direct SELECT AST repeats a FROM/UNNEST binding name or supplies an empty UNNEST name | — |
| `CD03022` | Index path count outside 1..32 in a direct AST/catalog call | — |
| `CD03024` | CREATE INDEX finds an existing name without IF NOT EXISTS | — |
| `CD03025` | Index has multiple array keys, is a UNIQUE array index, or includes MISSING with a leading array key | — |
| `CD03026` | Encoded index definition exceeds 8,092 bytes; before posting extraction/tree creation | — |
| `CD01025` | DROP INDEX finds no index without IF EXISTS | — |
| `CD02026` | Encoded composite index key exceeds 1,024 bytes during build or mutation | — |

[Scalar functions](functions/scalar-functions.md) validate literal kinds and contexts with
CD05001 before reading documents. A wrong-kind stored argument produces NULL. Fractional
REPLACE counts use CD02008; CD02019 has no assigned SQLSTATE. Evaluation failures abort an
explicit transaction.

For example, repeating `INSERT INTO items (KEY, VALUE) VALUES ('a', 1)` on the same visible
collection reports CD03012. A malformed static escape such as ESCAPE 'ab' is CD00002 instead
of CD02013. Ordinary NULL and MISSING results are values, not diagnostics.

CD02016, CD01017 and CD02018 have no assigned SQLSTATE. [UPDATE](statements/update.md) reports target locations
where available and validates all patched documents before the first write.

INSERT/UPSERT repeated-key failures use `Document key 'a' appears more than once in one statement.`
(CD03012, no SQLSTATE). Other shared key/value, binding and JSON validation messages name the
written verb, including UPSERT. See [UPSERT diagnostics](statements/upsert.md#diagnostics).

Paging count errors retain CD00007 and the operand span over the wire; unbound parameters retain
CD01005. Neither code has an assigned SQLSTATE. [LIMIT/OFFSET](clauses/limit-offset.md)
distinguishes syntax errors (CD00002) and the unsupported comma spelling (CK00001).

[CREATE INDEX](statements/create-index.md) uses CD03020 for duplicate valued scalar keys during
writes/builds, CD02021 for array/object UNIQUE components, and CD03022 for direct AST/catalog
path counts outside 1..32. CD02021/CD03022 and CD00009 have no assigned SQLSTATE. Constraints are
immediate per row; a failed explicit transaction requires RollbackAsync. These constraint failures
are distinct from DocumentDuplicateKeyException/CD03012 (document identity collisions).

[Array bindings](clauses/unnest.md) reserve the unaliased collection name for duplicate checks.
CD00010 and CD01023 have no assigned SQLSTATE. META on an UNNEST/quantified variable, including
one shadowing the source alias, uses CD05001. Quantifiers may shadow outer bindings.

[Comprehensions](expressions/comprehensions.md) validate all children even on empty input.
Aggregates inside them and META on an inner binding use CD05001; retired ARRAY()/FIRST() calls
use CK00001. These codes and CD01027 have no assigned SQLSTATE.

Index-name/key-size failures use DatabaseException with structured CD03024, CD01025 and CD02026 diagnostics
and the original DocumentCatalogException. They abort an explicit transaction; roll back before
continuing. In autocommit the next statement can run. No SQLSTATE is assigned.

## Typed array index conditions

The typed [array index API](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-key.md)
uses CD03025/CD03026 as DocumentCatalogException conditions. Unsupported typed WHEN constants,
including Single/Double, or predicates above 32 levels/128 nodes use uncoded ArgumentException.
OQL DISTINCT ARRAY DDL executes; ALL ARRAY remains CK00001. V3 index records are required; v1/v2 are
refused before recovery writes with CD06105 and a recreate remedy. No SQLSTATE is assigned.

OQL array DDL adds located CD01028 (projection/WHEN path outside its variable), CD02027
(unsupported WHEN scalar constant), CD03027 (excluded source/projection/WHEN expression) and
CD03028 (WHEN above 32 levels/128 nodes), before catalog writes. None has an assigned SQLSTATE.
See [CREATE INDEX](statements/create-index.md).

## Locations and wire behavior

[Native aggregates and ALL/DISTINCT](functions/aggregate-functions.md) use CD00011 in parsed
scalar/star calls and CD01029 for invalid direct AST quantifiers. Neither has an assigned SQLSTATE.
Invalid arity retains CD00006 and invalid placement CD05001. Boolean/statistics aggregates skip
wrong-kind stored values; SUM/AVG retain CD02008 for valued nonnumbers. Final statistic overflow
uses CD02007 (22003). Aggregate CD05018 failures retain the call span and leave an autocommit
session reusable; an explicit transaction requires rollback. Existing JSON, Boolean and Decimal
wire values/headers carry results without a protocol change.

Embedded command and query results expose a non-null DatabaseResult.Diagnostics collection,
empty when nothing is reported. DatabaseException.Diagnostics and DocumentClientException.Diagnostics
are also non-null, possibly empty. Remote DocumentResultSet has no Diagnostics member; remote
failure details continue to arrive through the model's Failure message and client exception.

Remote failures use [DocumentClientException](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-client-exception.md), which derives directly from
System.Exception and declares its own Diagnostics snapshot. Catch that client exception for remote
operations; catching DatabaseException now covers only engine-side failures.

DatabaseException.Diagnostics is never null and orders located entries by source start before
unlocated entries. Offsets are zero-based UTF-16 with exclusive ends; lines are one-based.
Planner/binder failures retain available AST spans. Query requests reject all parser errors
before a recovered AST reaches planning. In-process messages are code-free where the carrier
supplies a separate diagnostic; the server composes CODE: message from the first Error entry.

The Documents model sends terminal Failure (72), carrying the wire error category and coded
diagnostics with spans. This remains its structured diagnostic path on protocol 2.1; it does not
negotiate the core Diagnostics frame. Statement refusal preserves the connection; malformed
UTF-8/payloads and message order are protocol violations. The client exception retains diagnostics.

## Database files

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CD06104` | Storage operation or open I/O failure, including I/O inside catalog/index-registration wrappers; original cause retained | — |
| `CD06105` | Catalog/index failure; additional model diagnostic for attached-index corruption | — |
| `CD04101` | Explicit transaction is aborted; operations/commit refused until rollback | — |
| `CK06102` | Additional attached-index corruption diagnostic beside CK06003 and CD06105 | — |
| `CD06103` | Missing journal/backup, incomplete creation or unrecoverable files during create/open; preserve ambiguous files and restore the complete file set, or drop/recreate only a known incomplete create as directed | — |
| `CK06003` | Detected storage corruption; DatabaseCorruptionException names the database and optional page and retains the storage cause; refused opens also retain CD06103 | — |

CD06103 is a database lifecycle diagnostic carried by DatabaseException.Diagnostics, rather than a
query evaluation error. A leftover directory without its data file remains DatabaseNotFoundException
with the same diagnostic. Missing sidecars are never recreated. Transient open I/O failures ask for
a retry once resolved and carry no restore/drop advice. No SQLSTATE is assigned to CD06103.

CK06003 also fails damaged reads/statements and transaction operations instead of silently
omitting a document. Legitimate reclamation keeps its absence semantics. The existing wire
execution-failure path carries the diagnostic and leaves the connection usable. Follow the
message's verified-backup advice. See [storage recovery](../../overview.md#database-file-sets-and-lifecycle)
and [DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

Storage, catalog and index failures use structured model diagnostics with their original
InnerException. Wrapped I/O remains CD06104. Attached-index corruption leads with CK06003
and retains CK06102 and CD06105. Begin uses the same translator as operations. An I/O open
failure releases resources and publishes no database, permitting retry after resolution. An
operation failure aborts an explicit transaction; rollback permits embedded-session reuse.
The wire returns ExecutionFailure with diagnostics and preserves the connection. CD04101
is in Diagnostics; only the wire adds its message prefix.

## Operation cleanup

A content chunk failure or execution cancellation aborts through the engine's transaction end
gate, once; storage does not abort the borrowed context as well. An explicit session then
refuses commands/commit with CD04101 until rollback, retaining the original cause. Earlier
uncommitted chunks and writes are undone; no partial document becomes visible.

After successful automatic commit, a snapshot-cleanup failure preserves success and emits
`StatementCleanupFailed`, event **13**, Warning, from `Assimalign.Cohesion.Database.Documents`.
Its payload is `database`, `exceptionType`, with no exception message or document input.
`OutOfMemoryException` still propagates; cleanup failures before commit success or inside an
explicit transaction still fail the operation. The session hold is released in all cases.
This warning has no diagnostic code or SQLSTATE. See [engine diagnostics](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#cleanup-diagnostics).

## See also

[Language (OQL)](index.md) · [Parameters](expressions/parameters.md) · [MISSING](expressions/missing.md) · [Unsupported](unsupported.md)

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.Documents` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 14 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

The server drains complete shared terminal Errors and capacity refusals within its configured
ShutdownDrainTimeout, with TerminalDrainLinger bounding peer close after output completion.
TLS delivery remains limited by #1086. See [server options](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-database-server-options.md).

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

## Catalog and lifecycle conditions

CD04102 is a catalog snapshot conflict; CD04103 an expected-version mismatch; CD05016 a result byte/frame limit; CD07001 a session bound to another database; CD09001 a stale collection handle. These supplement the existing CD condition families and shared CK guards. See the [code scheme and registry](../../error-codes.md).

## Text transactions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04004 | Nested BEGIN/START; faults the existing transaction | — |
| CD04104 | COMMIT/ROLLBACK without an explicit transaction | — |
| CD04105 | OQL collection/index DDL inside an explicit transaction, including conditional no-ops | — |
| CD07002 | Text transaction control without the negotiated transaction-status capability | — |

CD04101 also covers work and COMMIT after a statement faults the transaction. ROLLBACK recovers
the session; a faulted COMMIT ends it without committing. See [transactions](statements/transactions.md).

## Application function conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD05017 | Application callback failed, result violated its declaration, or aggregate state was null/reused/invalid | — |
| CK07003 | Same-session callback reentry refused before nested execution | — |

CD02008 retains fractional typed integer conversion failures; CD02007 (22003) covers conversion outside the declared CLR integer range.
Application I/O, timeout and callback-thrown conversion/overflow failures use CD05017,
retaining the original cause. DatabaseException, out-of-memory, insufficient stack and actual
statement cancellation keep their identity. Unknown names retain CD00008 but ordinary engine
requests resolve them during binding/execution (ExecutionFailure, no StatementParseFailed);
standalone parsers retain unknown-name parse diagnostics. See [functions](functions/index.md#application-functions).

## Sources

- **Storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/StorageCorruptionTests.cs`.

- **File-set refusals and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDatabaseNameTests.cs`.
- **Carriers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlDiagnosticTests.cs` and `OqlDiagnosticsFixPassTests.cs` in the same directory.
- **Numbers and duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
- **Failures** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolFailureMessage.cs`.
