# DocumentConnection

An authenticated database-bound rental supporting one active operation at a time.

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentConnection : IAsyncDisposable
{
    public string Database { get; }
    public bool IsOpen { get; }
    public ValueTask<DocumentResultSet> QueryAsync(string statement,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<long> ExecuteAsync(string statement,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<Document?> GetAsync(string collection, DocumentId id, CancellationToken cancellationToken = default);
    public ValueTask<Document> PutAsync(string collection, DocumentId id, ReadOnlyMemory<byte> content,
        DocumentVersion? expectedVersion = null, CancellationToken cancellationToken = default);
    public ValueTask<bool> DeleteAsync(string collection, DocumentId id,
        DocumentVersion? expectedVersion = null, CancellationToken cancellationToken = default);
    public ValueTask<string> CreateCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<string> GetCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask DropCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<string>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    public ValueTask AbortAsync();
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

QueryAsync materializes rows or whole documents; ExecuteAsync returns the affected count (-1 for
queries, zero for DDL, written count for INSERT/UPSERT, deleted count for DELETE, matched count for UPDATE). Parameters follow case-sensitive bare-name
binding and accept JSON scalars/JsonElement values. Requests snapshot parameter/content bytes before
their first await. Get returns null when absent; Put replaces/upserts and returns a new version;
Delete returns whether a document existed. Expected versions must match visible content. Collection
operations return names; lists use ordinal name order. Explicit transactions are absent.

Blank statement/names or invalid parameters throw ArgumentException; invalid wire JSON can throw
ProtocolException. Disposed rentals throw ObjectDisposedException, concurrent exchanges
InvalidOperationException and cancellation OperationCanceledException. An unfinished exchange is
discarded; a canceled mutation may already have committed. A fully decoded model Failure raises
DocumentClientException and permits reuse, even if partial results preceded it; partial results are
discarded. Malformed traffic and shared Error discard the connection. Dispose returns a healthy
rental exactly once; Abort discards it. Repeated disposal cannot return a later rental.

The wrapper caches Database at rental time, so that name remains readable after disposal; IsOpen
returns false. Its shared DatabaseConnection lease permanently rejects execution after return.
SQL and Graph negotiate server-reported transaction status. Pool return rolls back open or failed transactions before reuse; only an Idle session is pooled. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

## Example

```csharp
await connection.CreateCollectionAsync("items");
var saved = await connection.PutAsync("items", "one", "{\"amount\":2}"u8.ToArray());
var read = await connection.GetAsync("items", "one");
Console.WriteLine(read!.Value.Version == saved.Version); // True
bool deleted = await connection.DeleteAsync("items", "one", saved.Version);
Console.WriteLine(deleted); // True
```

connection is rented from [DocumentClient](document-client.md); DocumentId has an implicit string conversion.

After putting `{"amount":2}` under key one, this partial update affects one document:

```csharp
await connection.PutAsync("items", "one", "{\"amount\":2}"u8.ToArray());
long matched = await connection.ExecuteAsync("UPDATE items SET amount = amount + 1");
Console.WriteLine(matched); // 1; stored amount is 3
```

UPDATE emits no result rows. Invalid path/overlap/stored-JSON results raise DocumentClientException
with CD02016/CD01017/CD02018, respectively, and a completed Failure permits reuse. See
[UPDATE](../../../../database/document/language/statements/update.md) for NULL/MISSING and version rules.

ExecuteAsync accepts [UPSERT](../../../../database/document/language/statements/upsert.md) and returns
the number of replaced/inserted pairs, with no rows. CD03012 rejects repeated input keys; decoded
Failure preserves the rental. Generated UTF-8 JSON bytes agree with in-process and InMemory results.

## Related reference

[Results](document-result-set.md) · [Exceptions](document-client-exception.md) · [Design](design.md)

## Queued terminal errors

Before the next admitted operation writes a request, the shared connector checks buffered input or
a closed transport and consumes a queued terminal Error once. Its original code reaches
DocumentClientException and the connector is discarded. This also applies to streaming startup;
IsOpen continues to describe physical transport health. Unexpected/malformed terminal frames use
ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are not implemented.
A clean handshake EOF has its own message naming the startup handshake. See
[shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors)
and [terminal delivery/TLS limits](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Sources

- **Terminal client conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
