# Examples

Use a Documents endpoint with database app already created. The client owns its pool; each rental
runs one operation at a time. Reference Documents.Client and Connections.Tcp.

## Query, insert and distinguish MISSING

```csharp
using System;
using Assimalign.Cohesion.Connections.Tcp;
using Assimalign.Cohesion.Database.Client;
using Assimalign.Cohesion.Database.Documents.Client;

await using var client = DocumentClient.Create(new() {
    Settings = DatabaseConnectionSettings.Parse("Database=app;Endpoint=127.0.0.1:5440"),
    ConnectionFactory = new TcpConnectionFactory()
});
await using var connection = await client.ConnectAsync();
await connection.ExecuteAsync("CREATE COLLECTION IF NOT EXISTS items");
await connection.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('a', {'name':'Ada'})");
var rows = await connection.QueryAsync("SELECT name, absent AS m, NULL AS n FROM items");
Console.WriteLine(rows[0]["name"]);    // Ada
Console.WriteLine(rows[0].IsMissing(1)); // True
Console.WriteLine(rows[0].IsNull(2));    // True
try {
    await connection.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('a', 2)");
} catch (DocumentClientException failure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHDOC012
}
var plan = await connection.QueryAsync("EXPLAIN SELECT name FROM items");
Console.WriteLine(plan.Columns.Count); // 8; the explained SELECT was not executed
long removed = await connection.ExecuteAsync("DELETE FROM items WHERE META().id = 'a'");
Console.WriteLine(removed); // 1; items and any index definitions remain
```

Start with an empty/new collection for the first run. CRUD retains bytes/versions; use PutAsync
for replacement rather than repeating INSERT. A decoded duplicate-key failure keeps the rental usable.

## Related reference

See [DocumentRow](../document-row.md#example) for VALUE MISSING normalization and JSON bytes,
and the [SELECT fixture](../../../../../database/document/language/statements/select.md#distinct-and-bare-values)
for DISTINCT, RAW and constructor results checked over InMemory and TCP.

[DocumentConnection](../document-connection.md) · [DocumentRow](../document-row.md) · [Design](../design.md) · [Assembly](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentExplainWireTests.cs`.
