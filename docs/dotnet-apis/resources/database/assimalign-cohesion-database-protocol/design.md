# Assimalign.Cohesion.Database.Protocol design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Protocol`.

> **Status:** Partial.

## Mechanism and model policy

This child root owns the frame envelope, shared payload primitives, startup and authentication
exchange, session lifecycle, error taxonomy, and version negotiation. It has no model dependencies.
As with `Database.Language`'s `TokenLexer`, `QueryLanguageProfile` and `QueryParser`, and `Database.Types`' `Diagnostic`, mechanism
is shared while each model supplies its own grammar: here that grammar is its message family and
payload codecs.

The dependencies point from model policy toward the shared mechanism.

```mermaid
flowchart LR
    Sql["Database.Sql.Protocol"] --> Core["Database.Protocol"]
    Kv["Database.KeyValuePair.Protocol"] --> Core
    Blob["Database.Blob.Protocol"] --> Core
    Docs["Database.Documents.Protocol"] --> Core
    Graph["Database.Graph.Protocol"] --> Core
    Client["Database.Client"] --> Core
```

Each model Protocol package owns message identifiers and encode/decode routines. Engine servers and
client exchanges implement the model ordering rules; each Client package owns result materialization. `Database.Client` owns pooling, handshake, framed
exchanges, and connection health. Each model server continues to own its accept loop and session
state machine.

## Envelope and primitives

Every frame begins with a five-byte header. `Header` bytes 0–3 (bits 0–31) are the unsigned 32-bit
payload length in big-endian order, and byte 4 (bits 32–39) is the unsigned 8-bit message type. The
payload starts at byte 5, and the declared length counts only those payload bytes, not the header.
Payloads are limited to 16,777,216 bytes. Readers check the length before allocation, require the
entire declared payload, distinguish clean EOF between frames from truncation, and reject oversized
declarations. Writers apply the same bound. Framing imposes no logical-object buffering or transfer
policy. The packet view below shows the two fixed header fields.

```mermaid
packet-beta
0-31: "Payload length (u32, big-endian)"
32-39: "Message type (u8)"
```

`ProtocolPayload` provides signed i32/i64, unsigned u16, and strings encoded as
`i32 UTF8-byte-length + UTF8 bytes`. Negative lengths and out-of-bounds reads fail with
`ProtocolException`. Codecs are direct span operations, with no reflection, Microsoft.Extensions
dependencies, serialization discovery, or generated runtime code.

## Numbering and the family seam

The endpoint selects the model before accept. Startup has no model discriminator. A connection
serves one model and one database throughout its lifetime. A client must choose the endpoint for the
family it speaks; the protocol cannot discover a misconfigured endpoint without the deliberately
deferred handshake discriminator.

| Identifier | Owner / meaning |
| --- | --- |
| 0 | Reserved, invalid |
| 1 | Core: Startup |
| 2 | Core: Authenticate |
| 3 | Core: AuthenticateResponse |
| 4 | Core: Ready |
| 5–9 | Model family, preserving deployed identifiers |
| 10 | Core: Error |
| 11 | Core: Ping |
| 12 | Core: Pong |
| 13 | Core: Terminate |
| 14 | Core Diagnostics, gated by agreement |
| 15–63 | Reserved for future shared mechanism |
| 64–255 | Model family |

A single contiguous model range would have renumbered deployed messages unnecessarily. Instead, 5–9
remain endpoint-scoped legacy slots, and new families use 64–255. Different endpoints may assign the
same byte differently. There is never a union of families or a per-message registry lookup on one
session.

`ProtocolMessageFamily` copies its identifier set, rejects duplicate and reserved identifiers, and
has no mutators. `ProtocolChannel` permanently binds one family to one stream and validates incoming
and outgoing identifiers against that family and the core. Model server sessions create the channel
exactly once. Client pools select one family when composed and reject exchanges belonging to another
family. Payload interpretation belongs exclusively to that endpoint's model codec. Unknown
identifiers are protocol violations; session code reports Error(ProtocolViolation) and closes.

The low-level `ProtocolFraming` reader/writer remain available for diagnostics and framing tests. They
deliberately do not dispatch payloads or infer model semantics. Applications use `ProtocolChannel` for
their endpoint's validated exchange.

## Terminal delivery and frame writes

ProtocolFrameWriter.Create emits header and payload in one Stream.WriteAsync, including empty
frames. Nonempty frames use contiguous rented storage returned with clearArray: true after write
completion, including cancellation/failure. A stream may flush during WriteAsync (DuplexPipeStream
does), so explicit FlushAsync does not promise batching. This makes no change to frame bytes or
message identifiers. FrameWritten follows the completed stream write, before any explicit flush.

All five model servers complete Output after a terminal shared Error before disposing its transport.
One ShutdownDrainTimeout deadline starts before writing and covers write, flush, completion and
peer-input drain. TerminalDrainLinger (two seconds by default) starts after output completion and
bounds peer close, including capacity refusals. Both are positive, finite and at most
4,294,967,294 milliseconds. Refusals are tracked before I/O and joined by Stop. Soft stop permits
its shutdown notice to drain. Outer expiry/hard abort releases blocked I/O, and teardown joins it
before releasing registrations, connections and sessions. Final cleanup has no second timeout;
transports/execution ignoring abort can delay Stop completion.

Each model emits its own TerminalDrainTimedOut Warning and total-terminal-drain-timeouts counter
for terminal-session deadline aborts. Linger expiry and server hard abort do not count as that event.
Clients inspect queued terminal responses before new materialized/streaming request writes, retain
the Error code and discard the connector. Unexpected/malformed terminal input is ProtocolViolation;
clean EOF without Error remains Internal. A handshake EOF specifically names startup.

Loopback TCP tests require at least 200 consecutive missing-database refusals for SQL, Documents,
Graph, Key-Value and Blob through real clients. SQL/Key-Value additionally cover authentication,
capacity, unsupported-major startup, idle eviction and graceful-stop notices. Controlled transports
cover blocked writes, flush, output completion, abort and cleanup joining for every model.

**Known gap (#1086):** TLS terminal delivery is not guaranteed. The production TLS decorator
completes its SslStream-backed output without completing inner TCP output; the SQL TLS refusal
regression remains skipped because the Database-only fix still loses the expected terminal frame.

## Shared exchange and payloads

### Bounded batches and borrowed payloads

`ProtocolFrameWriter.CreateBatched` coalesces complete frames into one pooled batch. Defaults
are 64 KiB including headers and 32 frames; a larger frame is sent alone. Every payload
retains the 16,777,216-byte limit and there is no aggregate result cap. `ProtocolChannel`
selects these defaults with batchWrites: true. Callers serialize writes, flushes and disposal,
flush at result and latency boundaries, and await transport writes for backpressure. Disposal
discards an unfinished batch. Buffers are cleared before return, after pending I/O is joined.

```csharp
public static ProtocolFrameWriter CreateBatched(Stream stream, bool leaveOpen = false,
    int maxBatchBytes = 64 * 1024, int maxBatchFrames = 32);
```

A null stream throws ArgumentNullException. maxBatchBytes must be between 5 and 16,777,221
inclusive and maxBatchFrames at least 1, otherwise ArgumentOutOfRangeException is thrown.
WriteFrameAsync still enforces the per-frame limit with ProtocolException. A failed transport
write, including cancellation during a send before any bytes were copied, faults the batched
writer. Later writes and flushes throw IOException. Disposal returns cleared storage without
retrying or sending subsequent frames. A separate flush failure after a successful write does
not fault it, preserving the session's ability to attempt its soft-stop terminal notice.

`ReadFrameAsync` continues to return owned payloads. The optional borrowed-read API instead
scopes pooled storage to a synchronous callback:

```csharp
// ProtocolFrameReader
protected ProtocolFrameReader(bool supportsBorrowedPayloads = false);
public bool SupportsBorrowedPayloads { get; }
public ValueTask<bool> ReadBorrowedFrameAsync(Action<ProtocolFrame> consume,
    CancellationToken cancellationToken = default);
protected virtual ValueTask<bool> ReadBorrowedFrameCoreAsync(Action<ProtocolFrame> consume,
    CancellationToken cancellationToken);
```

The stream reader and ProtocolChannel's family reader support borrowed payloads; unsupported
decorators refuse them. ReadBorrowedFrameAsync returns true after consuming one frame and
false at clean EOF. A null callback throws ArgumentNullException; an unsupported reader throws
NotSupportedException. Malformed/incomplete frames throw ProtocolException, and cancellation
and stream I/O failures propagate. Decode or copy retained values inside the callback. The
reader clears and returns its payload rental before the read completes, including when the
callback throws. Length validation precedes renting, and only the declared slice is exposed.
Callers serialize reads and disposal and join an active read before disposing the reader.

This example follows ProtocolBatchTests and writes twenty complete empty frames in one batch:

```csharp
using System;
using System.IO;

using Assimalign.Cohesion.Database.Protocol;

using var stream = new MemoryStream();
await using var writer = ProtocolFrameWriter.CreateBatched(stream, leaveOpen: true);
for (int i = 0; i < 20; i++)
{
    await writer.WriteFrameAsync(new ProtocolFrame(ProtocolMessageType.Pong,
        ReadOnlyMemory<byte>.Empty));
}
await writer.FlushAsync();
stream.Position = 0;
await using var reader = ProtocolFrameReader.Create(stream, leaveOpen: true);
bool read = await reader.ReadBorrowedFrameAsync(frame => Console.WriteLine(frame.Type)); // Pong
Console.WriteLine(read); // True; copy/decode any retained payload inside the callback
```

### Protocol EventSource

Provider `Assimalign.Cohesion.Database.Protocol` retains Verbose FrameRead and FrameWritten
events under Frames. FrameWritten is emitted once per enclosed frame after a successful
transport write, before any explicit flush. Its polling counters are created when enabled,
with backing values maintained even before a listener attaches:

| Counter | Meaning |
| --- | --- |
| transport-write-calls | Attempted Stream writes by batched writers, including failed attempts |
| transport-flush-calls | Attempted explicit Stream flushes by batched writers, including failed attempts |
| pooled-buffers-in-use | Currently rented batched-writer and borrowed-read buffers |

Ordinary writers, including SQL's per-row writer, do not update these process-wide counters;
their temporary arrays are excluded from the pool gauge. Generic streams do not expose their
internal flush policy. DuplexPipeStream flushes on each write, so the sum of write and explicit
flush counts gives its pipe flush requests. These are cumulative operation counts and a buffer
gauge, not frame counts, bytes, rates or network packets. Graph's temporary encoding rental is
also outside this pool gauge. See [Graph phases](../assimalign-cohesion-database-graph/design.md#path-performance-counters).

### Exchange sequence

The shared handshake precedes every model-specific exchange, as shown here.

```mermaid
sequenceDiagram
    participant Client as "Client core"
    participant Server as "Model endpoint"
    Client->>Server: Startup(version, database, principal)
    Server-->>Client: Authenticate
    Client->>Server: AuthenticateResponse
    Server-->>Client: Ready
    Note over Client,Server: Fixed endpoint family owns subsequent requests and results
    Client->>Server: Model request
    Server-->>Client: Model response messages
    Client->>Server: Ping
    Server-->>Client: Pong
    Client->>Server: Terminate
```

Startup is `u16 major + u16 minor + string database + string principal`. In its fixed prefix, bytes
0–1 (bits 0–15) are the big-endian unsigned 16-bit major version, bytes 2–3 (bits 16–31) are the
big-endian unsigned 16-bit minor version, and bytes 4–7 (bits 32–63) are the database string's
nonnegative big-endian signed 32-bit UTF-8 byte length `N`. The `N` database bytes start at byte 8;
the principal's nonnegative big-endian signed 32-bit UTF-8 byte length follows at byte `8 + N`,
followed by that many principal bytes. The packet view below shows the startup payload's fixed
eight-byte prefix.

```mermaid
packet-beta
0-15: "Major version (u16, big-endian)"
16-31: "Minor version (u16, big-endian)"
32-63: "Database UTF-8 byte length N (nonnegative i32, big-endian)"
```

The existing trust handshake sends an empty Authenticate and AuthenticateResponse; authenticators
receive response evidence as opaque bytes. Ready is empty on negotiated 2.0; 2.1 carries the agreement. Ping, Pong, and Terminate have
empty payloads. Error is `u16 code + string message`. Codes are append-only: Internal=0,
UnsupportedVersion=1, AuthenticationFailed=2, NotAuthorized=3, DatabaseNotFound=4, ParseFailure=5,
ExecutionFailure=6, TransactionAborted=7, ProtocolViolation=8, Unavailable=9. Statement failures can
leave a session ready; framing, handshake, or ordering violations terminate it.

## Version decision

Current is 2.1; different majors fail and the lower minor is selected. 2.0 retains empty Ready.

## Extending a model

Define a model-owned identifier enum, a single immutable family, codecs, and its ordering rules.
`Bind` that family at endpoint accept and client pool composition. `Use` shared `ProtocolPayload`
primitives where appropriate, and document the exact wire payload in the owning model package. `Add`
transport-independent conformance coverage through Connections.InMemory. Models do not add payload
policy to this core.

`ProtocolException` is this child root's independent exception type. Servers translate it to the
shared wire taxonomy; the client core translates it to `DatabaseClientException`. Transports remain
outside this package and outside engine packages.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Protocol 2.1 negotiation


The shared protocol is 2.1. Startup optionally appends the client's capability names;
Ready returns the agreed version and the two-sided intersection. A negotiated 2.0
session retains empty Ready and the existing model and Error bytes. An empty Ready
from an older endpoint means 2.0 with no capabilities. Different majors are refused.
Capability names are ordinal and append-only; model names are scoped with the
lowercase endpoint family followed by a dot. The core owns `diagnostics` and frame 14.

Graph also registers [graph.composite-values](../assimalign-cohesion-database-graph-protocol/graph-protocol.md),
a payload extension under the same 2.1 negotiation with no new frame types. Diagnostics negotiates
independently; baseline Graph scalar bytes remain when composites are not agreed.

SQL and Graph register `ProtocolCapability.Diagnostics`. When agreed, one Diagnostics
frame carries the bounded Warning/Error list immediately before Error on failure,
or immediately before ResultComplete (PathsComplete for graph paths) on success.
Information, including SQL0100, is never sent. Error and result payloads are unchanged.
Statement failures leave the server session ready. Model clients validate diagnostic
ordering and preserve every code, severity, message, nullable offset and source line.

### Startup extension and Ready

The unchanged Startup prefix is `u16 major + u16 minor + string database + string principal`.
For 2.1, append `u16 capabilityCount + capabilityCount * string name`. An absent
extension is an empty list. The 2.0 decoder stops after principal, accepting a 2.1
Startup; a 2.0 client sends the original four fields. Ready in 2.1 is
`u16 major + u16 minor + u16 capabilityCount + capabilityCount * string name`.
Both lists allow at most 64 unique names of 1..128 ASCII characters from
`a-z`, `0-9`, `.` and `-`; the extension/Ready bound is 16,384 bytes. Trailing
bytes, duplicates, malformed UTF-8, unsupported versions and unsolicited agreements
are ProtocolViolation; the client discards a failed handshake's transport.

`ProtocolMessageFamily` registers immutable capability descriptors alongside model
identifiers. A model capability begins with its lowercase family name and a dot;
it can gate only that family's identifiers. `ProtocolChannel.Negotiate` selects
the intersection, and `SetAgreement` applies it exactly once. Read and write gates
reject Diagnostics, and any model capability's added frames, until agreed. A
capability may describe a payload extension with no new frame. Existing frames
must keep their baseline encoding when the relevant capability was not agreed.

### Diagnostics payload

All numbers are big-endian. The payload is `i32 entryCount`, followed by entries:
`u8 severity + string code + string message + i32 start + i32 end + i32 line`.
Severity is Warning=1 or Error=2; other values are invalid. Both offsets are -1
for an unlocated diagnostic, otherwise 0 <= start <= end; end is exclusive and
line is one-based. Entry order is the producer's source order. The maximum is
4,096 entries and 1,048,576 encoded bytes. Counts, truncation, invalid strings,
invalid spans/lines, trailing bytes and size violations throw ProtocolException,
translated by the endpoint/client to ProtocolViolation. Outbound overflow follows the explicit prefix-and-notice policy below.

Outbound SQL and Graph diagnostics are preflighted before any result or Error output.
If the filtered Warning/Error list exceeds 4,096 entries or 1,048,576 encoded bytes,
the endpoint retains the longest leading prefix that fits both limits with a final
Warning notice, code COHDBP001. The notice counts toward both limits and states the
retained count, original filtered count and omitted count. Once an entry does not fit,
it and every following entry are omitted, even if a later entry would fit. Entries
are never shortened. A 4,097-entry list of small diagnostics therefore keeps the first
4,095 and the notice; a first entry larger than the byte limit leaves only the notice.
The original failure still sends exactly one Error with its original error category
and message. Success still sends its result and normal completion with the bounded
warning list. Overflow alone never terminates the session or changes transaction state.
Malformed incoming diagnostic frames remain fatal protocol violations.

Null diagnostic codes and messages encode as zero-length strings; zero-length strings
decode as null. An empty diagnostic code is never a valid code. Unlocated offsets
remain null. Ready.Capabilities and Diagnostics.Entries reject null at construction
and through init/with assignment. Capability-name byte lengths are checked against
the 128-character ASCII bound before decoding or allocating a name string.

Protocol owns ProtocolDiagnostic and ProtocolDiagnosticSeverity with no Language
or Types reference. Servers translate their carrier; the shared client maps the
wire values into the existing DatabaseException.Diagnostics collection. Success
result objects expose the ProtocolDiagnostic values directly. Codecs use checked
span reads and static encoding, with no reflection or runtime discovery.


## Sources

- **Batching, borrowing and counters** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolBatchTests.cs`.
- **Public framing contracts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolFrameWriter.cs` and `ProtocolFrameReader.cs` in the same directory.
- **Frame write** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolFramingTests.cs`.
- **Terminal client/transport evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlClientTerminalPathTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlServerTlsTerminalTests.cs`.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/Assimalign.Cohesion.Database.Protocol.csproj`.
