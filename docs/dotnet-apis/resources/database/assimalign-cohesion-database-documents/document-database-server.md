# DocumentDatabaseServer

Serves OQL and document/collection operations on a fixed Documents endpoint.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentDatabaseServer : DatabaseServer
{
    public static DocumentDatabaseServer Create(DocumentDatabaseEngine engine,
        DocumentDatabaseServerOptions options);
    public new DocumentDatabaseEngine Engine { get; }
    public override IReadOnlyCollection<DatabaseServerSession> Sessions { get; }
}
```

## Behavior and exceptions

The caller supplies an engine and listener; creation snapshots options. The server owns its listener
lifecycle; Stop terminally disposes the listener. It authenticates each database-bound session and
serves one exchange at a time. Model Failure completes a rejected statement without Complete; the
session stays ready. Protocol violations close it.

The formatter retains one encoded frame and flushes each result. TCP backpressure bounds queued
output; InMemory's output pipe does not apply backpressure, so queued bytes can grow if its reader
stops. Engine queries and client results remain materialized. Every individual JSON result fits the
shared 16 MiB frame limit. Malformed UTF-8 is rejected by the shared and model payload readers.

Null engine/options throw ArgumentNullException; missing listener, nonpositive session limit or
invalid authentication/idle timer durations throw ArgumentException. Nonpositive, infinite or oversized
ShutdownDrainTimeout/TerminalDrainLinger values throw ArgumentOutOfRangeException. Statement failures retain DatabaseException
codes/spans in the Documents Failure payload. No wire transaction command is implemented.

## Example

Given an existing engine, database and Connections listener:

```csharp
await using var server = DocumentDatabaseServer.Create(engine, new() { Listener = listener });
await server.StartAsync(CancellationToken.None);
// DocumentClient connections can now execute OQL and CRUD in the selected database.
await server.StopAsync(CancellationToken.None);
```

## Related reference

[Options](document-database-server-options.md) · [Protocol](../assimalign-cohesion-database-documents-protocol/document-protocol.md) · [Pooled client](../assimalign-cohesion-database-documents-client/index.md)

## Terminal delivery

Terminal shared Error delivery starts one ShutdownDrainTimeout deadline before the write. The
server writes and flushes the complete frame, completes connection Output, then consumes peer input
until peer close, TerminalDrainLinger or the outer deadline. The default drain budget is 30 seconds;
linger defaults to two seconds and starts after output completion. Capacity refusals use the same
sequence without creating a session, and are registered before I/O so Stop joins their cleanup.
Soft stop allows its terminal notice to drain, including a stop racing the final Ready flush.

Hard abort or outer expiry aborts blocked transport I/O. Teardown joins pending I/O and detaches
callbacks before disposing token sources, the transport and engine session. Every resource release
is attempted even when an earlier disposal fails. Final cleanup is joined without another timeout:
a transport/execution that ignores abort can delay Stop. ShutdownDrainTimeout bounds graceful work;
it does not abandon resources still owned by pending I/O. An unread idle pooled peer releases its
server slot after linger rather than occupying the whole outer budget.

Both ShutdownDrainTimeout and TerminalDrainLinger must be greater than zero and at most
4,294,967,294 milliseconds. Infinite values are refused with ArgumentOutOfRangeException at server
construction. Setters store values and creation snapshots them. A disposed input reader during
drain counts as transport closure. TLS terminal delivery remains a known gap (#1086): completing
the TLS output does not complete its inner TCP output, so a fatal frame can still be lost.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Server/DocumentDatabaseServer.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentServerDrainTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/docs/DESIGN.md`.
