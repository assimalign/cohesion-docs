# Blob operations

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

Engine and storage errors send `ProtocolErrorCode.ExecutionFailure`; protocol violations send
`ProtocolErrorCode.ProtocolViolation`. The Blob server closes the session after an exchange
failure. A broken transport can prevent delivery of the error itself. The client then observes
truncation or transport failure rather than successful completion.

## Examples

The [streaming client example](streaming-client.md#example) demonstrates all five operations
using the sequence from the client overview. It assumes the database and container already exist.

## See also

- **Blob** — [Engine and container behavior](index.md).
- **Wire protocol** — [Message payloads and shared handshake](wire-protocol.md).
- **Database** — [Database documentation](../index.md).

## Sources

- **Operation contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/DESIGN.md`.
- **Client contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/Abstractions/IBlobConnection.cs`.
- **Behavior tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/tests/BlobClientTests.cs`.
