# DocumentClientOptions

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentClientOptions
{
    public DatabaseConnectionSettings? Settings { get; set; }
    public IDuplexConnectionFactory? ConnectionFactory { get; set; }
}
```

## Behavior and exceptions

Settings selects database, endpoint, principal and pool limits. ConnectionFactory is the statically
composed transport driver. The setters store values; DocumentClient.Create validates that both are
present and the settings are valid, throwing ArgumentException when they are not.

## Example

```csharp
var options = new DocumentClientOptions {
    Settings = DatabaseConnectionSettings.Parse("Database=app;Endpoint=127.0.0.1:5440"),
    ConnectionFactory = new TcpConnectionFactory()
};
await using var client = DocumentClient.Create(options);
```

Use the same namespace imports and endpoint setup as [DocumentClient](document-client.md).

## Related reference

[Assembly](index.md) · [DocumentClient](document-client.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentClientOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
