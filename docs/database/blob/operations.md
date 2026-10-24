# Blob operations

Blob uses `CB` for binding, values, schema, transactions, storage and lifecycle failures, with shared `CK` diagnostics where applicable. See the [code scheme and registry](../error-codes.md).

The Blob wire server exposes read, write, delete, property lookup, and prefix listing operations.

> **Status:** Implemented.

Requests operate inside the database selected by the shared startup handshake. Each request names
a container and either an object or a prefix. Database and container provisioning remain host-side
engine operations. There is no Blob command grammar.

## Request and response shapes

These are protocol message sequences, not executable statements. Content chunks are acknowledged
individually; the [wire protocol](wire-protocol.md) defines their byte layouts.

| Operation | Request | Successful server response | Client API |
|---|---|---|---|
| Read | `Read(container, name)` | `TransferStart`, zero or more `Chunk` frames, `TransferComplete` | `DownloadAsync` |
| Write | `Write(container, name, overwrite)` followed by the client's content transfer | Chunk acknowledgements, then `TransferComplete` after commit | `UploadAsync` |
| Delete | `Delete(container, name)` | `OperationComplete(0)` if absent or `OperationComplete(1)` if deleted | `DeleteAsync` |
| Get properties | `GetProperties(container, name)` | `OperationComplete(0)` if absent; otherwise `Properties` then `OperationComplete(1)` | `GetPropertiesAsync` |
| List | `List(container, prefix)` | One `Properties` per match, then `OperationComplete(count)` | `GetBlobsAsync` |

Only one operation is active on a connection. A shared `Error` frame replaces successful
completion. A partially received listing is not a successfully completed result.

## Arguments

| Argument | Meaning |
|---|---|
| `container` | Nonempty case-sensitive container name in the selected database |
| `name` | Nonempty case-sensitive object name; slashes do not select a database |
| `overwrite` | Boolean replacement flag, encoded as byte `0` or `1` |
| `prefix` | Ordinal name prefix; an empty wire string selects every object |
| Transfer length | Exact nonnegative content byte count or `-1` for unknown length |
| Content type | Transfer metadata; an empty wire string means unspecified |

Each wire text field is limited to 65,535 UTF-8 bytes. Names and metadata must also fit one
storage record. No Unicode normalization is applied.

## Read

The server reads properties and content in one snapshot transaction, so concurrent replacement
cannot pair one version's metadata with another version's bytes. The client must read through a
validated `TransferComplete`. Premature end of file (EOF), malformed completion, and server
errors are failures. Already delivered bytes remain provisional until successful EOF.

A missing object fails a download; it does not produce an empty-object response. A real empty
object sends start and completion messages with no content chunks.

## Write

The client sends `TransferStart`, zero or more chunks, and `TransferComplete` after `Write`.
Each chunk has at most 65,536 bytes. The sender waits for the receiver's cumulative
`ChunkAcknowledgement` before reading further source data.

The server uses an explicit engine transaction. It validates the declared and actual lengths,
finishes the destination stream, commits, and only then sends its own `TransferComplete`.
Chunk acknowledgement confirms destination acceptance; it does not confirm publication.

Cancellation, disconnect, malformed completion, or storage failure before commit rolls back.
Failed new objects remain absent; failed replacements preserve the previous committed object.
If the connection fails after commit but before the final acknowledgement arrives, the upload
outcome is uncertain to the client. Verify properties before retrying when that distinction
matters. There is no exactly-once retry guarantee.

## Metadata and listing

`Properties` carries name, length, optional content type, a 64-bit entity tag, creation and
modification timestamps, and CRC-32. The timestamps use Coordinated Universal Time (UTC) ticks
since 0001-01-01, restricted to the `DateTime` range. Content-type presence distinguishes null
from an explicitly empty string.

Listing reads catalog metadata and returns objects in ordinal name order. The final count must
match the number of property frames. The typed client returns null for missing properties and
false for deletion of a missing object.

## Failure behavior

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CB06005` | Storage operation or open I/O failure; original cause retained | — |
| `CB06006` | Malformed/unsupported catalog metadata; additional model diagnostic for attached-index corruption | — |
| `CB02007` | Upload has failed or its transaction is no longer active; later writes/publication refused | — |
| `CB04001` | Explicit transaction is aborted; operations/commit refused until rollback | — |
| `CK06102` | Additional attached-index corruption diagnostic beside CK06003 and CB06006 | — |

Child-root storage and catalog failures cross the engine boundary as DatabaseException with
Error diagnostics and the original InnerException. Explicit and automatic transaction begin use
the same translation. Storage corruption leads with CK06003; attached-index corruption also
retains CK06102 and CB06006. Existing offline/unconfirmed-commit classifications remain in
force. An I/O open failure releases acquired resources, publishes no database and permits retry
after the condition is resolved.

A failed operation aborts an explicit transaction; roll it back before further work. The embedded
session can then be reused. CB04001 is a structured aborted-transaction diagnostic with an
ordinary embedded message. The wire adds the first Error code as a prefix when absent, sends
ExecutionFailure and closes the connection.

A failed upload or one whose transaction ended raises CB02007 on later writes rather than
accepting buffered content. It cannot publish partial content; disposal after rollback also
reports CB02007. Unrelated InvalidOperationException/ObjectDisposedException retain their
ordinary .NET types. See [streaming client](streaming-client.md#diagnostics).

Engine and storage errors send `ProtocolErrorCode.ExecutionFailure`; protocol violations send
`ProtocolErrorCode.ProtocolViolation`. The Blob server closes the session after an exchange
failure. A broken transport can prevent delivery of the error itself. The client then observes
truncation or transport failure rather than successful completion.

## Operation cleanup

After successful automatic commit, a snapshot-cleanup failure preserves success and emits
`StatementCleanupFailed`, event **14**, Warning, from `Assimalign.Cohesion.Database.Blob`.
Its payload is `database`, `exceptionType`, with no exception message or blob input.
`OutOfMemoryException` still propagates; cleanup failures before commit success or inside an
explicit transaction still fail the operation. The session hold is released in all cases.
This warning has no diagnostic code or SQLSTATE. A connection lost after commit can still lose
the completion frame. See [engine diagnostics](../../dotnet-apis/resources/database/assimalign-cohesion-database-blob/design.md#cleanup-diagnostics).

## Database lifecycle

Engine CreateDatabaseAsync/OpenDatabaseAsync require the original blob.dat, blob.log and blob.bak.
Missing journals/backups are refused without recreating files. Drop removes blob.dat first, so an
interrupted deletion cannot resurrect a database. Known incomplete creates require drop/recreate;
ambiguous or damaged file sets require preservation and complete-backup recovery. A torn uncommitted
tail is tolerated during WAL recovery. Transient open I/O failures ask callers to retry once the
condition is resolved, without restore or drop advice.

### Database file diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CB06004` | Missing journal/backup, incomplete creation or unrecoverable files on create/open; follow the refusal's specific recovery instruction | — |
| `CK06003` | Detected storage corruption; DatabaseCorruptionException names the database and optional page and retains the storage cause; refused opens also retain CB06004 | — |

DatabaseException.Diagnostics carries this Error; a directory without its data file remains
DatabaseNotFoundException with the diagnostic preserved. No SQLSTATE is assigned. See
[lifecycle APIs](../../dotnet-apis/resources/database/assimalign-cohesion-database-blob/design.md#database-creation-open-and-drop).

Corruption fails a read/operation rather than silently omitting a blob. Legitimate record
reclamation retains absence semantics. The code uses the existing wire execution-failure path;
stream failure still fails the read. Follow the message's advice to restore a verified backup.
See [storage recovery](../overview.md#database-file-sets-and-lifecycle) and
[DatabaseCorruptionException](../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

## Examples

The [streaming client example](streaming-client.md#example) demonstrates all five operations
using the sequence from the client overview. It assumes the database and container already exist.

## See also

- **Blob** — [Engine and container behavior](index.md).
- **Wire protocol** — [Message payloads and shared handshake](wire-protocol.md).
- **Database** — [Database documentation](../index.md).

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.Blob` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 15 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

The server drains complete shared terminal Errors and capacity refusals within its configured
ShutdownDrainTimeout, with TerminalDrainLinger bounding peer close after output completion.
TLS delivery remains limited by #1086. See [server options](../../dotnet-apis/resources/database/assimalign-cohesion-database-blob/blob-database-server-options.md).

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

## Sources

- **Storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/tests/StorageCorruptionTests.cs`.

- **File-set refusals and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/tests/BlobDatabaseNameTests.cs`.
- **Operation contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/DESIGN.md`.
- **Client contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/BlobConnection.cs`.
- **Behavior tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/tests/BlobClientTests.cs`.
