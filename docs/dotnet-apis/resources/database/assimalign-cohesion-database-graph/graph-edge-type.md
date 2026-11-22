# GraphEdgeType

An immutable directed edge declaration referencing source and target node types.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphEdgeType
{
    public string RelationshipType { get; }
    public string SourceNodeType { get; }
    public string TargetNodeType { get; }
    public IReadOnlyList<GraphPropertyType> Properties { get; }
    public static GraphEdgeType Create(string relationshipType, string sourceNodeType,
        string targetNodeType, IEnumerable<GraphPropertyType>? properties = null);
}
```

## Behavior and exceptions

SourceNodeType and TargetNodeType are identifying-label strings. Schema mutation resolves them
against the complete resulting graph type, refusing absent references with CG03011. This allows
ADD/ALTER fragments to reference retained node declarations. Selective DROP must remove referencing
edges together with their node types.

Create copies properties into read-only ordinal order; null means none. Blank names or null/repeated
properties throw ArgumentException. Names compare ordinally and case-sensitively. Edge declarations
are stored and survive reopen, but edge property types, NOT NULL and endpoint membership are not
yet enforced.

## Example

```csharp
var edge = GraphEdgeType.Create("VISITS", "Person", "Place",
    [GraphPropertyType.Create("count", DatabaseType.Int64, required: true)]);
Console.WriteLine(edge.TargetNodeType); // Place
```

## Related reference

[Assembly](index.md) · [GraphType](graph-type.md) · [GraphSchema](graph-schema.md#graph-type-operations)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphEdgeType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphTypeTests.cs`.
