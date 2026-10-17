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
queries, zero for DDL, inserted count for INSERT). Parameters follow case-sensitive bare-name
binding and accept JSON scalars/JsonElement values. Requests snapshot parameter/content bytes
before their first await. Get returns null when absent; Put replaces/upserts and returns a new
version; Delete returns whether a document existed. Expected versions must match visible content.
Collection operations return names; lists use ordinal name order. Explicit transactions are absent.

Blank statement/names or invalid parameters throw ArgumentException; invalid wire JSON can throw
ProtocolException. Disposed rentals throw ObjectDisposedException, concurrent exchanges
InvalidOperationException and cancellation OperationCanceledException. An unfinished exchange is
discarded; a canceled mutation may already have committed. A fully decoded model Failure raises
DocumentClientException and permits reuse, even if partial results preceded it; partial results
are discarded. Malformed traffic and shared Error discard the connection. Dispose returns a healthy
rental exactly once; Abort discards it. Repeated disposal cannot return a later rental.

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

## Related reference

[Results](document-result-set.md) · [Exceptions](document-client-exception.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
