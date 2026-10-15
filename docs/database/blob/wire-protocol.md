# Blob wire protocol

Blob uses the shared database envelope with a fixed endpoint message family and bounded content chunks.

> **Status:** Implemented. The documented wire version is 1.0.

`BlobProtocol.Family` binds a `ProtocolChannel` to Blob for its whole lifetime. The endpoint
selects the model before startup; startup has no model discriminator. The database selected
during startup cannot change through object requests.

## Shared envelope

Each frame contains a five-byte header followed by exactly the declared payload bytes. All
integers described here use big-endian order.

| Frame offset | Width | Field |
|---|---|---|
| 0 | 4 bytes | Unsigned 32-bit payload byte count, excluding the header |
| 4 | 1 byte | Message identifier |
| 5 | Declared byte count | Message payload |

The shared payload limit is 16,777,216 bytes. Readers enforce it before allocation and reject
truncated frames. Blob content chunks have a stricter 65,536-byte limit. Object size is not
limited to a single frame.

## Startup and shared messages

Complete `Startup` → `Authenticate` → `AuthenticateResponse` → `Ready` before sending requests.
In the table, `text` means a signed 32-bit UTF-8 byte length followed by those bytes.

| Identifier | Message | Payload |
|---|---|---|
| 1 | `Startup` | Unsigned 16-bit major, unsigned 16-bit minor, `text database`, `text principal` |
| 2 | `Authenticate` | Empty for the trust handshake |
| 3 | `AuthenticateResponse` | Opaque authenticator evidence; empty for trust |
| 4 | `Ready` | Empty in version 1.0 |
| 10 | `Error` | Unsigned 16-bit code, `text message` |
| 11 | `Ping` | Empty |
| 12 | `Pong` | Empty |
| 13 | `Terminate` | Empty |

Version negotiation rejects a major other than 1 and selects the minimum peer/server minor.
Version 1.0 `Ready` contains no negotiated-version field. `Ping` and `Pong` are available
between operations.

## Blob messages

For Blob payloads, `text` is a signed 32-bit UTF-8 byte length followed by exactly that many
strict UTF-8 bytes. Each text field is limited to 65,535 bytes. Negative lengths, invalid
UTF-8, missing bytes, and trailing bytes are violations. Names must be nonempty. Signed
64-bit counts are nonnegative unless a field explicitly permits `-1`.

| Identifier | Message | Direction | Payload fields in order |
|---|---|---|---|
| 64 | `Read` | Client to server | `text container`, `text name` |
| 65 | `Write` | Client to server | `text container`, `text name`, unsigned byte overwrite (`0` or `1`) |
| 66 | `TransferStart` | Content sender to receiver | Signed 64-bit length (`-1` means unknown), `text contentType` |
| 67 | `Chunk` | Content sender to receiver | 1–65,536 raw bytes, with no inner prefix |
| 68 | `TransferComplete` | Sender to receiver; server to client after upload commit | Signed 64-bit actual byte count |
| 69 | `ChunkAcknowledgement` | Content receiver to sender | Signed 64-bit cumulative accepted byte count |
| 70 | `Delete` | Client to server | `text container`, `text name` |
| 71 | `GetProperties` | Client to server | `text container`, `text name` |
| 72 | `List` | Client to server | `text container`, `text prefix` |
| 73 | `Properties` | Server to client | `text name`, signed 64-bit length, unsigned byte content-type presence, optional `text contentType`, unsigned 64-bit entity tag, signed 64-bit creation ticks, signed 64-bit modification ticks, unsigned 32-bit CRC |
| 74 | `OperationComplete` | Server to client | Signed 64-bit result count |

`TransferStart` has an eight-byte length at payload offset 0, a four-byte content-type byte
count at offset 8, and the text at offset 12. Its payload is exactly 12 plus the text byte
count. `TransferComplete` and `ChunkAcknowledgement` are exactly eight payload bytes each.

The empty `TransferStart` content-type string means unspecified. `Properties` instead uses
a presence byte (`0` or `1`), distinguishing null from empty content type. Its entity tag
retains all 64 bits. Timestamps are ticks since 0001-01-01 Coordinated Universal Time (UTC),
limited to the `DateTime` range. The cyclic redundancy check (CRC) is CRC-32.

## Transfer sequence

On download, `Read` is followed by server `TransferStart`, zero or more chunks, and
`TransferComplete`. On upload, the client sends `Write` followed by the same content sequence.

After each chunk, the receiver writes the bytes to its destination and acknowledges the
cumulative accepted count. The sender waits for that acknowledgement before reading more
source content. This keeps at most one 64 kibibyte (KiB) chunk in flight, including on transports
without automatic backpressure. Incorrect, duplicate, or out-of-order acknowledgement counts fail.

For a declared nonnegative length, the receiver rejects excess content before writing the
excess chunk and checks the final count against both the declared and received lengths. An
unknown-length transfer still requires the final count to match received bytes. Empty objects
send start and completion without chunks.

The server sends a second `TransferComplete` after an upload has been validated and committed.
That final server message confirms publication. A chunk acknowledgement never does.

`BlobProtocolTransfer.SendAsync` and `ReceiveAsync` implement the content sequence. They do
not perform authentication, request dispatch, storage commit, final upload acknowledgement,
channel closure, or caller-stream disposal. The Blob server supplies those responsibilities.

## Errors and connection state

The shared error taxonomy is append-only.

| Code | `ProtocolErrorCode` member |
|---|---|
| 0 | `Internal` |
| 1 | `UnsupportedVersion` |
| 2 | `AuthenticationFailed` |
| 3 | `NotAuthorized` |
| 4 | `DatabaseNotFound` |
| 5 | `ParseFailure` |
| 6 | `ExecutionFailure` |
| 7 | `TransactionAborted` |
| 8 | `ProtocolViolation` |
| 9 | `Unavailable` |

This is the shared vocabulary, not a claim that Blob emits every code. Blob has no language
parser. Its server reports engine/storage failures as `ExecutionFailure` and malformed
exchanges as `ProtocolViolation`, closing the session afterward.

There is one active request or transfer per connection, with no transfer identifiers or
multiplexing. Unexpected frames, premature closure, malformed payloads, stream failures, and
cancellation abort the exchange. Discard the connection after failure. The protocol provides
no transfer resumption or exactly-once retry guarantee.

## See also

- **Blob** — [Engine overview](index.md).
- **Operations** — [Request and result behavior](operations.md).
- **Streaming client** — [Client connection ownership](streaming-client.md).
- **Database** — [Database documentation](../index.md).

## Sources

- **Shared framing and handshake** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/DESIGN.md`.
- **Blob family and transfer specification** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/DESIGN.md`.
- **Message identifiers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/src/Protocol/BlobProtocolMessageType.cs`.
- **Transfer implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/src/Protocol/BlobProtocolTransfer.cs`.
