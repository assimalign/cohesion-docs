# Create a table and insert a row

An embedded SQL engine creates a database, opens a session, and executes parameterized SQL.

> **Status:** Partial.

The package overview supplies the engine and session operations. This version adds the explicit
imports and a local `dataDirectory` value. The engine owns its background workers and is disposed
asynchronously.

```csharp
using System.Collections.Generic;

using Assimalign.Cohesion.Database.Sql;

string dataDirectory = "data";

// A data machine: operational from Create (background workers running), no
// start ceremony; dispose to durably flush and close.
await using var engine = SqlDatabaseEngine.Create(new SqlDatabaseEngineOptions { RootPath = dataDirectory });

var database = await engine.CreateDatabaseAsync("app");
await using var session = await database.CreateSessionAsync();

await session.ExecuteAsync(SqlQueryRequest.FromSql(
    "CREATE TABLE users (id BIGINT PRIMARY KEY, name VARCHAR(100));"));
await session.ExecuteAsync(SqlQueryRequest.FromSql(
    "INSERT INTO users (id, name) VALUES (@id, @name);",
    new Dictionary<string, object?> { ["id"] = 1L, ["name"] = "Ada" }));
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
