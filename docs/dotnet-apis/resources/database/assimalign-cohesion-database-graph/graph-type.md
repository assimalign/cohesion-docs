# GraphType

An immutable, open graph type scoped to one database.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphType
{
    public IReadOnlyList<GraphNodeType> NodeTypes { get; }
    public IReadOnlyList<GraphEdgeType> EdgeTypes { get; }
    public static GraphType Create(IEnumerable<GraphNodeType>? nodeTypes = null,
        IEnumerable<GraphEdgeType>? edgeTypes = null);
}
```

## Behavior and exceptions

Create copies collections into read-only, ordinally ordered declarations: nodes by identifying
label and edges by relationship type. Null collections mean none; an empty type is valid.
Null elements or repeated identifying names within a kind throw ArgumentException. All names
compare ordinally and case-sensitively. Construction does not publish a type or validate data.

[GraphSchema](graph-schema.md#graph-type-operations) resolves edge references against the complete
resulting type, allowing ADD/ALTER fragments. An implied label cannot identify any node type
(CG03011). Membership derives from identifying labels; additional labels, relationship types
and properties remain writable. Node rules execute; edge property/endpoint enforcement is deferred.

## Example

Following GraphTypeTests:

```csharp
var type = GraphType.Create([
    GraphNodeType.Create("Person", ["Entity"],
        [GraphPropertyType.Create("name", DatabaseType.String, required: true)]),
    GraphNodeType.Create("Place")],
    [GraphEdgeType.Create("VISITS", "Person", "Place")]);
Console.WriteLine(type.NodeTypes.Count); // 2
```

## Related reference

[Assembly](index.md) · [GraphSchema](graph-schema.md) · [GraphNodeType](graph-node-type.md) · [GraphEdgeType](graph-edge-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphTypeTests.cs`.
