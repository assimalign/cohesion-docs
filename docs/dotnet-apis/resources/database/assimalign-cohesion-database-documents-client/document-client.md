# DocumentClient

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentClient : IAsyncDisposable
{
    public static DocumentClient Create(DocumentClientOptions options);
    public DatabaseConnectionSettings Settings { get; }
    public ValueTask<DocumentConnection> ConnectAsync(CancellationToken cancellationToken = default);
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

Create composes DatabaseClient with DocumentProtocol.Family and opens connections on demand.
ConnectAsync rents an authenticated, database-bound connection; disposal returns a healthy rental.
Client disposal closes idle connections and closes outstanding connections when returned.

Null options throw ArgumentNullException; missing/invalid settings or transport factory throw
ArgumentException. A disposed client throws ObjectDisposedException. Cancellation throws
OperationCanceledException. Dial/handshake failures become DocumentClientException with the shared
protocol code and original shared-client failure retained as inner exception.

## Example

```csharp
await using var client = DocumentClient.Create(new DocumentClientOptions {
    Settings = DatabaseConnectionSettings.Parse("Database=app;Endpoint=127.0.0.1:5440"),
    ConnectionFactory = new TcpConnectionFactory()
});
await using var connection = await client.ConnectAsync();
await connection.CreateCollectionAsync("items");
await connection.PutAsync("items", "a", "{\"name\":\"Ada\"}"u8.ToArray());
var rows = await connection.QueryAsync("SELECT name FROM items");
Console.WriteLine(rows[0]["name"]); // Ada
```

Reference Connections.Tcp and import its namespace alongside Database.Client and Documents.Client.
The example assumes a Documents endpoint with database app already created.

## Related reference

[Options](document-client-options.md) · [Connection](document-connection.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentClient.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
