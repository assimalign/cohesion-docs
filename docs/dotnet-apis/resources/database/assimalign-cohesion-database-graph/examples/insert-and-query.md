# Insert and query a graph

Create graph data, define a node-property index, and execute a graph query.

> **Status:** Partial.

This package-overview example adds the explicit namespace import. The engine uses the `graphs`
directory for its durable files.

```csharp
using Assimalign.Cohesion.Database.Graph;

await using var engine = GraphDatabaseEngine.Create(new() { RootPath = "graphs" });
var graph = (IGraphDatabase)await engine.CreateDatabaseAsync("people");
await using var session = await graph.CreateSessionAsync();
await session.ExecuteAsync("INSERT (a:Person {name:'Ada'})-[r:KNOWS]->(b:Person {name:'Grace'})");
await GraphSchema.Open(graph, session).CreateIndexAsync("Person", "by_name", "name");
await using var result = await session.ExecuteAsync(
    "MATCH (a:Person {name:'Ada'})-[r:KNOWS]->(b) RETURN a,r,b");
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
