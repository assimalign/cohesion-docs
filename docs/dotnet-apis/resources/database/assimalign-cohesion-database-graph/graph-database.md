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
    public ValueTask<int?> FindShortestDistanceAsync(GraphDatabaseSession session,
        GraphShortestPathSearch search, CancellationToken cancellationToken = default);
    public ValueTask<GraphShortestPathResult?> FindShortestPathAsync(GraphDatabaseSession session,
        GraphShortestPathSearch search, CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

The engine creates or opens this concrete type, replacing IGraphDatabase. Node, relationship and
traversal operations retain an explicit GraphDatabaseSession argument and follow its transaction.
GetNodeAsync returns null for an absent node. GraphNode and GraphRelationship carry typed identities
and scalar properties. A foreign session raises DatabaseException with CG07005; an aborted
transaction uses CG04007. Disposed databases throw ObjectDisposedException; offline databases
throw DatabaseOfflineException (CG06012). Cancellation throws OperationCanceledException.

Direct lifetime guards retain ObjectDisposedException. Disposal inside admitted execution becomes
DatabaseException with structured CG09019. Aborted-transaction refusals carry CG04007 in
Diagnostics with an unprefixed message and the original cause; roll back before continuing.
CreateNodeAsync rejects a null label list and null/empty/whitespace label names before admission;
CreateRelationshipAsync similarly validates its type. These ArgumentException/ArgumentNullException
caller failures leave an explicit transaction active. GQL text uses located planner diagnostics.

## Shortest distance and path

Both shortest-search methods count unweighted relationship hops. `FindShortestDistanceAsync`
returns only the distance; `FindShortestPathAsync` returns a
[GraphShortestPathResult](graph-shortest-path-result.md) with the selected path and distance.
[GraphShortestPathSearch](graph-shortest-path-search.md) defaults to Outgoing. Incoming follows
relationships backwards; Both searches as an undirected graph. The path keeps stored relationship
endpoints even when traversed backwards. A null type filter accepts every type. Type names compare
ordinally and case-sensitively; unknown, empty and whitespace names admit no edges, without warnings.

Disconnected or invisible endpoints return null. A visible start equal to the target returns zero
and a singleton path, even with an unknown type. Equal-length paths select the lexicographically
smallest complete relationship-ID sequence, starting at Start. Cycles, loops and parallel edges
do not change the minimum-hop rule. Search has no hop cap, including no GQL 64-hop cap.

Each call uses one statement snapshot: Snapshot retains the transaction horizon; ReadCommitted
pins a horizon for the statement. Earlier own writes are visible. Only the selected path's entities
are materialized; distance search keeps identity-based state rather than explored entity payloads.

Engine memory/work ceilings apply, and search Limits can only lower them. Endpoint probes,
selected adjacency candidates and path reconstruction consume work; retained search state and
path payloads consume memory. Exhaustion fails with CG05004 (work) or CG05020 (memory), never
a false no-path or partial result. Invalid limits use CG08021. Cancellation raises
OperationCanceledException with the caller token. Invalid direction (CG05001) and pre-cancellation
precede admission and keep an explicit transaction active; admitted failures fault it until rollback.

Session closure during expansion or reconstruction retains CK07004; transaction termination retains
CK04002. Lifecycle refusals take precedence over missing records. If the operation is still active
but a selected node or relationship is absent during reconstruction, DatabaseException carries
structured CG05022. These conditions have no assigned SQLSTATE.

GQL path selectors, including ANY SHORTEST, remain refused with CK00001; these methods are typed APIs.

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

### Search in reverse

Adapted from `GraphShortestPathTests.FindShortestPathAsync_DirectionFilter_ShouldPreserveTraversalOrder`:

```csharp
using Assimalign.Cohesion.Database.Graph;
using Assimalign.Cohesion.Database.Graph.Protocol;

await using var engine = GraphDatabaseEngine.Create("directions", new());
var graph = await engine.CreateDatabaseAsync("graph");
await using var session = await graph.CreateSessionAsync();
var a = await graph.CreateNodeAsync(session, ["Person"]);
var b = await graph.CreateNodeAsync(session, ["Person"]);
var c = await graph.CreateNodeAsync(session, ["Person"]);
await graph.CreateRelationshipAsync(session, a.Id, b.Id, "KNOWS");
await graph.CreateRelationshipAsync(session, b.Id, c.Id, "KNOWS");
var search = new GraphShortestPathSearch(c.Id, a.Id, GraphDirection.Incoming, "KNOWS");
int? distance = await graph.FindShortestDistanceAsync(session, search);
var result = await graph.FindShortestPathAsync(session, search);
Console.WriteLine(distance); // 2
Console.WriteLine(result!.Path.Nodes[0].Id == c.Id); // True
Console.WriteLine(result.Path.Nodes[2].Id == a.Id); // True
```

## Related reference

[Engine](graph-database-engine.md) · [Schema](graph-schema.md) · [Examples](examples/index.md)

## Sources

- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.cs`.
- **Session binding cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphDatabaseScopeTests.cs`.
- **Shortest search and lifecycle cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphShortestPathTests.cs`.
- **Shortest implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.ShortestPaths.cs`.
