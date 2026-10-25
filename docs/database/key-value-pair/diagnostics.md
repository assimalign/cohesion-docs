# Diagnostics

Key-Value Pair uses `CV` across syntax, binding, value, schema, transaction, execution, storage and protocol categories. See the [code scheme and registry](../error-codes.md).

Key-Value Pair distinguishes invalid commands, conditional misses, execution failures, and protocol failures.

> **Status:** Implemented.

The command parser raises `DatabaseParseException` with a coded diagnostic. The wire uses the shared `ProtocolErrorCode` values.
The typed client preserves the code in `KeyValueClientException.Code` and supplies a
model-specific category in `KeyValueClientException.Kind`.

Embedded command and materialized row results expose non-null DatabaseResult.Diagnostics,
empty when nothing is reported. DatabaseException.Diagnostics is also never null. The remote
KeyValueClientException.Diagnostics collection is always empty and non-null; codes remain in
Error text because this model does not negotiate structured diagnostics. Typed entry and write
outcomes have no Diagnostics member.

## Diagnostic codes

| Code | Meaning |
| --- | --- |
| `CV00001` | Empty command |
| `CV03002` | Attempted catalog mutation |
| `CV00003` | Unknown command verb |
| `CV00004` | Request-constructor validation failed |
| `CV00005` | Invalid PUT arity |
| `CV00006` | PUT requires IF |
| `CV00007` | Invalid DELETE arity |
| `CV00008` | DELETE requires IF |
| `CV00009` | SCAN clause operand missing |
| `CV00010` | Repeated SCAN clause |
| `CV00011` | Unknown SCAN clause |
| `CV02012` | Byte operand required |
| `CV02013` | Integer etag required |
| `CV02014` | Nonnegative LIMIT literal required |
| `CV02015` | Nonnegative integer LIMIT parameter required |
| `CV00016` | Parameter reference required |
| `CV01017` | Parameter value missing |
| `CV00018` | Command arity mismatch |
| `CV04019` | Execution failure without an engine diagnostic |
| `CV04101` | Aborted transaction refuses commands/commit; original coded cause retained |
| `CV06106` | Incomplete file-backed creation or missing catalog; refused before storage opens; no assigned SQLSTATE |
| `CV06105` | Storage/index or open I/O operation failed, including wrapped/aggregate causes; I/O SQLSTATE 58030 |
| `CK06102` | Additional index-corruption diagnostic beside CK06003 and CV06105; no assigned SQLSTATE |
| `CK06003` | Storage or attached-index corruption; database/page and original cause retained; SQLSTATE XX001 |

Parser refusals use ParseFailure and leave the session ready. Execution errors without an engine
code use CV04019. Wire Error text is CODE: message, with the existing layout; Key-Value does not
negotiate the core Diagnostics capability. For example GET without its operand reports CV00018;
PUT KEYSPACES @value reports CV03002. Conditional misses remain successful outcomes.

## Missing file sets and interrupted drop

CK06003 fails damaged reads and writes instead of returning a missing key. Legitimate reclaimed
entries still read as absent. Create/open and transaction operations use the same integrity
translation, including wrapped/aggregate causes. Wire errors retain the common code through the
existing ExecutionFailure path. Follow the message's verified-backup advice; see
[storage recovery](../overview.md#database-file-sets-and-lifecycle) and
[DatabaseCorruptionException](../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

File-backed open requires the original data and catalog file sets, including each journal and
backup. With the primary data file present, missing sidecars in an existing file set raise
DatabaseException with CV06104 in Diagnostics (Error, no
assigned SQLSTATE), before replacement files can be created. Preserve all files and restore the
complete backup, since the journal may contain acknowledged commits. Drop deletes keyvalue.dat
first, then other data files, then the remaining directory. It cannot resurrect a partially
dropped database by recreating a sidecar.

`CV06106` is an Error diagnostic with no assigned SQLSTATE. Before opening either storage,
the engine refuses file-backed data of at most one page as interrupted creation; its message
directs `DropDatabaseAsync`, then `CreateDatabaseAsync`. Otherwise, an entirely missing catalog
requires preserving storage and restoring the complete database from backup. This missing-catalog
gate also applies in memory. Refused opens create no files, leave surviving files byte-identical,
and publish no database. An existing catalog is validated before data recovery.

## Storage failures and transaction recovery

StorageException subclasses, IndexException and IOException, including inner and aggregate
causes, cross begin, command, commit/rollback and create/open boundaries with CV06105 when
no more specific classification applies. I/O maps to SQLSTATE 58030. Corruption takes precedence:
CK06003 maps to XX001; attached-index damage also retains CK06102 and CV06105, keeping the
index cause and page. Format refusals retain CK06001/CK06101 and their authored messages,
diagnostics and inner causes. Existing offline/unconfirmed-commit outcomes remain in force.

A storage, I/O or index failure aborts an explicit transaction after command admission is released. Further
commands and COMMIT are refused with CV04101 in Diagnostics, retaining the original coded
cause; roll back before continuing. Ordinary command errors keep statement atomicity and the
transaction usable. In autocommit the next command can run immediately after a recoverable
failure. Wire ExecutionFailure preserves the session. I/O open failures release acquired file
sets and publish no database, permitting retry after the condition is resolved.

Storage-failure, corruption and aborted-transaction refusal messages are ordinary unprefixed
embedded text. The refusal carries CV04101 as an Error in Diagnostics and retains the original
InnerException cause. The wire still adds `CV04101: ` once. No SQLSTATE is assigned to CV04101.

If abort cleanup also fails, the exact coded command exception remains primary, including its
corruption type. EventSource Assimalign.Cohesion.Database.KeyValuePair emits Warning event 11,
TransactionCleanupFailed, with transactionId, exceptionType and exceptionMessage (bounded to
1,024 characters). The cleanup failure does not replace the read diagnosis.

## Command failures

| Condition | Surface | Effect |
|---|---|---|
| Unknown verb, bad token count, or unsupported clause | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Missing parameter or wrong runtime operand type | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Duplicate scan clause or incompatible scan bounds | `DatabaseParseException` / `ParseFailure` (`5`) | Command does not execute; session remains ready |
| Conditional write misses | `applied=false` or affected count `0` | Successful outcome without mutation |
| Concurrent committed write to the same key | Retryable transaction abort / `ExecutionFailure` (`6`) | Transaction conflicts; session remains usable |
| Malformed frame, message, or ordering | `ProtocolViolation` (`8`) | Connection is broken |

The engine also exposes `DatabaseTransactionDeadlockException` for retryable deadlocks and
`DatabaseNotFoundException` when opening absent storage. Kernel exceptions are translated at
the model boundary.

## Read-only catalog diagnostic

Attempts such as `PUT KEYSPACES @value` or `delete keyspaces` produce the stable message:

```text
CV03002: The KEYSPACES catalog surface is read-only.
```

This check concerns the command text. A parameter-bound key containing those bytes remains
ordinary data.

## Client categories

`KeyValueClientErrorKind` defines `Internal`, `ConnectionFailure`, `AuthenticationFailure`,
`NotAuthorized`, `ParseFailure`, `ExecutionFailure`, `TransactionAborted`,
`ProtocolViolation`, `Unavailable`, and `MalformedResult`.

`KeyValueClientException.ConnectionUsable` is true for `ParseFailure`, `ExecutionFailure`,
`TransactionAborted`, `NotAuthorized`, and `MalformedResult`. It is false for the other
categories. Conditional misses never become this exception.

The shared wire enum also contains `Internal` (`0`), `UnsupportedVersion` (`1`),
`AuthenticationFailed` (`2`), `NotAuthorized` (`3`), `DatabaseNotFound` (`4`),
`TransactionAborted` (`7`), and `Unavailable` (`9`). A declared shared code does not imply
that every model path emits it; the key-value write-conflict contract uses `ExecutionFailure`.

## See also

- **[Key-Value Pair](index.md)** — engine behavior.
- **[Operand types](operand-types.md)** — accepted bindings.
- **[Unsupported features](unsupported.md)** — excluded grammar and engine features.

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.KeyValuePair` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 12 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

The server drains complete shared terminal Errors and capacity refusals within its configured
ShutdownDrainTimeout, with TerminalDrainLinger bounding peer close after output completion.
TLS delivery remains limited by #1086. See [server options](../../dotnet-apis/resources/database/assimalign-cohesion-database-keyvaluepair/key-value-database-server-options.md).

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
returns. These events add no diagnostic code or SQLSTATE. See [lane limits and worker health](../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).

## Failure tracing

The shared StatementFailed event takes its payload code from non-null Diagnostics: first Error, otherwise first entry, otherwise empty. It does not extract codes from messages or include statement/message text. Offline and unconfirmed-commit failures carry their model code in that carrier. See [failure-event selection](../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#structured-failure-event-codes).

## Additional execution conditions

CV05001 identifies a Put execution without an outcome row, CV05002 an unsupported executor request kind, CV06107 an unsupported entry-space format and CV07001 a session bound to another database. Shared CK identities cover common lifecycle/state/value guards. See the [registry](../error-codes.md).

## Sources

- **Storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIndexReadIntegrityTests.cs`.

- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/Execution/KeyValueCommandParser.cs`.
- **Engine errors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Wire codes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/Exceptions/ProtocolErrorCode.cs`.
- **Client categories** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/KeyValueClientErrorKind.cs`.
- **Client mapping** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Exceptions/KeyValueClientException.cs`.
- **Read-only conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueIntrospectionTests.cs`.
