# GraphTraversal

The `GraphTraversal` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

A traversal specification: where to start, which relationships to follow, in which direction, and
how deep.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphTraversal`** — selects a start, direction, optional type and maximum depth. `TraverseAsync`
  yields distinct visited nodes in deterministic breadth-first order, excluding the start.

## Signature and selection

```csharp
public readonly record struct GraphTraversal(
    GraphNodeId Start,
    GraphDirection Direction = GraphDirection.Outgoing,
    string? RelationshipType = null,
    int MaxDepth = 1);

// GraphDatabase
public IAsyncEnumerable<GraphNode> TraverseAsync(GraphDatabaseSession session,
    GraphTraversal traversal, CancellationToken cancellationToken = default);
```

TraverseAsync resolves RelationshipType through the catalog and seeks only that type's selected
direction ranges. Null selects all types; an unknown type yields no nodes and no warning. Edges
are ordered by identity before breadth-first traversal. Self-loops and cycles never emit the start,
and each other node is emitted at most once. A zero depth emits nothing. Results materialize in one
statement snapshot before yielding. There is no candidate-expansion cap on this API; GQL's
[physical budget](../../../../database/graph/language/patterns.md#remarks) applies to pattern matching.

## Exceptions

ArgumentNullException reports a null session. DatabaseException reports negative depth or undefined
direction (CG05001), a foreign session (CG07005), closed/busy session or refused transaction
(CG04007). DatabaseOfflineException reports an offline database (CG06012), ObjectDisposedException
a disposed database, and OperationCanceledException cancellation during traversal or yielding.

## Example

With an open GraphDatabase and its GraphDatabaseSession, this follows GraphEngineTests:

```csharp
var a = await database.CreateNodeAsync(session, ["Person"]);
var b = await database.CreateNodeAsync(session, ["Person"]);
var c = await database.CreateNodeAsync(session, ["Person"]);
await database.CreateRelationshipAsync(session, a.Id, b.Id, "LINK");
await database.CreateRelationshipAsync(session, b.Id, c.Id, "LINK");
await database.CreateRelationshipAsync(session, c.Id, a.Id, "LINK");
await database.CreateRelationshipAsync(session, a.Id, a.Id, "LINK");
await foreach (var node in database.TraverseAsync(session,
    new GraphTraversal(a.Id, RelationshipType: "LINK", MaxDepth: int.MaxValue)))
{
    Console.WriteLine(node.Id == b.Id ? "b" : "c");
}
```

Result: b, then c. Incoming traversal at depth one from a returns c; an unknown type returns none.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Execution limits

The public GraphQueryLimits? Limits init property applies optional lower memory/work limits to typed traversal. Null inherits engine ceilings. See [GraphQueryLimits](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/ValueObjects/GraphTraversal.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.cs`.
- **Tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphEngineTests.cs`.
