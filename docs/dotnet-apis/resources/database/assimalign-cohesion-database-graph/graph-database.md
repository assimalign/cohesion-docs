# GraphDatabase

A sealed property-graph database with database-bound sessions.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed partial class GraphDatabase : DatabaseInstance
{
    public new GraphDatabaseEngine Engine { get; }
    public new ValueTask<GraphDatabaseSession> CreateSessionAsync(CancellationToken cancellationToken = default);
    public ValueTask<GraphNode> CreateNodeAsync(GraphDatabaseSession session, IReadOnlyList<string> labels,
        IReadOnlyDictionary<string, object?>? properties = null, CancellationToken cancellationToken = default);
    public ValueTask<GraphNode?> GetNodeAsync(GraphDatabaseSession session, GraphNodeId id,
        CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

The engine creates or opens this concrete type, replacing IGraphDatabase. Node, relationship and
traversal operations retain an explicit GraphDatabaseSession argument and follow its transaction.
GetNodeAsync returns null for an absent node. GraphNode and GraphRelationship carry typed identities
and scalar properties. A foreign session raises DatabaseException with COHDBG005; an aborted
transaction uses COHDBG007. Disposed databases throw ObjectDisposedException; offline databases
throw DatabaseOfflineException (COHDBG012). Cancellation throws OperationCanceledException.

## Example

```csharp
await using var engine = GraphDatabaseEngine.Create("people-engine", new());
var graph = await engine.CreateDatabaseAsync("people");
await using var session = await graph.CreateSessionAsync();
var node = await graph.CreateNodeAsync(session, new[] { "Person" },
    new Dictionary<string, object?> { ["name"] = "Ada" });
var read = await graph.GetNodeAsync(session, node.Id);
Console.WriteLine(read!.Value.Properties["name"]); // Ada
```

## Related reference

[Engine](graph-database-engine.md) · [Schema](graph-schema.md) · [Examples](examples/index.md)

## Sources

- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.cs`.
- **Session binding cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphDatabaseScopeTests.cs`.
