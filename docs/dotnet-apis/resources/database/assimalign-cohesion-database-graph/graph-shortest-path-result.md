# GraphShortestPathResult

One selected shortest path and its unweighted distance.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphShortestPathResult
{
    public int Distance { get; }
    public GraphPath Path { get; }
}
```

The engine constructs this type; there is no public constructor. Distance equals
Path.Relationships.Count. Path is a [GraphPath](../assimalign-cohesion-database-graph-protocol/graph-path.md)
in start-to-target order, with Distance + 1 nodes and Distance relationships, materialized from one
statement snapshot. Stored relationship directions are preserved. Equal-length ties choose the
lexicographically smallest complete relationship-ID sequence. A visible node to itself gives a
singleton path and Distance 0; invisible endpoints or no path give a null result instead.

## Example

For the fixture in the [GraphDatabase example](graph-database.md#search-in-reverse):

```csharp
var result = await graph.FindShortestPathAsync(session,
    new GraphShortestPathSearch(a.Id, c.Id, RelationshipType: "KNOWS"));
Console.WriteLine(result!.Distance);                 // 2
Console.WriteLine(result.Path.Nodes.Count);          // 3
Console.WriteLine(result.Path.Relationships.Count);  // 2
```

The result's getters do not perform database reads. The search method's budget, cancellation and
lifecycle failures are documented on [GraphDatabase](graph-database.md#shortest-distance-and-path).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphShortestPathResult.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphShortestPathTests.cs`.
