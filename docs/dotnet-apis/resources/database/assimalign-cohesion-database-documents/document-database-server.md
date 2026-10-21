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
invalid timer durations throw ArgumentException. Statement failures retain DatabaseException
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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Server/DocumentDatabaseServer.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentServerDrainTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/docs/DESIGN.md`.
