# GraphShortestPathSearch

An unweighted shortest-search specification with database-local node identities.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public readonly record struct GraphShortestPathSearch(
    GraphNodeId Start,
    GraphNodeId Target,
    GraphDirection Direction = GraphDirection.Outgoing,
    string? RelationshipType = null)
{
    public GraphQueryLimits? Limits { get; init; }
}
```

GraphNodeId and GraphQueryLimits belong to `Assimalign.Cohesion.Database.Graph.Protocol`.
Start and Target identify the endpoints in the session's database. Outgoing is the default;
Incoming follows stored edges backwards and Both permits either direction. RelationshipType is
an ordinal, case-sensitive filter: null admits all types; an unknown name admits none. A visible
node to itself still has distance zero with any filter. Default/zero IDs are absent nodes.

Limits can lower the engine ceilings (256 MiB retained memory and 1,000,000 work units
by default). Omitted members inherit their ceiling. Nonpositive values or values above the engine
ceiling fail with CG08021 when executed. There is no maximum-hop member. Work/memory exhaustion
fails with CG05004/CG05020; it never returns an incomplete no-path answer. The value itself does
not validate its members; an undefined Direction fails with CG05001 before statement admission.

## Example

For the `a -> b -> c` KNOWS fixture in the [GraphDatabase example](graph-database.md#search-in-reverse):

```csharp
var search = new GraphShortestPathSearch(c.Id, a.Id, GraphDirection.Both, "KNOWS")
{
    Limits = new GraphQueryLimits { WorkUnits = 1_000 }
};
Console.WriteLine(await graph.FindShortestDistanceAsync(session, search)); // 2
```

See [GraphDatabase](graph-database.md#shortest-distance-and-path) for snapshot, cancellation,
transaction and lifecycle exceptions, and [GraphQueryLimits](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/ValueObjects/GraphShortestPathSearch.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphShortestPathTests.cs`.
