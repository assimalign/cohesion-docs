# GraphSchema

The `GraphSchema` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Creates session-bound graph metadata access without changing the graph database contract.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

```csharp
public sealed class GraphSchema
{
    public static GraphSchema Open(GraphDatabase database, GraphDatabaseSession session);
    public ValueTask<IReadOnlyList<GraphLabelMetadata>> GetLabelsAsync(CancellationToken cancellationToken = default);
}
```

- **`GraphSchema.Open(database, session)`** — returns `GraphSchema` for label/type discovery, definition
  and property changes, and node-property index creation. It rejects foreign sessions.

Null arguments throw ArgumentNullException; a disposed database throws ObjectDisposedException.
A foreign session raises DatabaseException with COHDBG005. Operations follow the bound session's
transaction; an aborted transaction refuses work with COHDBG007 until rollback.

## Example

```csharp
await using var engine = GraphDatabaseEngine.Create("people-engine", new());
var graph = await engine.CreateDatabaseAsync("people");
await using var session = await graph.CreateSessionAsync();
await session.ExecuteAsync("INSERT (:Person {name:'Ada'})");
await GraphSchema.Open(graph, session).CreateIndexAsync("Person", "by_name", "name");
```

The index is created over Person.name in that database, as in the engine overview example.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.cs`.
