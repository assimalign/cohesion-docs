# GraphPropertyType

An immutable scalar property declaration for a graph element type.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphPropertyType
{
    public string Name { get; }
    public DatabaseType Type { get; }
    public bool Required { get; }
    public static GraphPropertyType Create(string name, DatabaseType type, bool required = false);
}
```

## Behavior and exceptions

Name compares ordinally and case-sensitively. Supported types are Boolean, Int8, Int16, Int32,
Int64, Float32, Float64, Decimal and String. A blank name or unsupported type throws ArgumentException.
Required defaults to false, allowing absent/null values; true expresses NOT NULL. Present non-NULL
values must match the graph engine's scalar type, including its unsigned numeric promotions.

Node declarations enforce these rules; violations use CG03003. Proposed types that fail existing
member validation use CG03010 without replacing the prior type. Edge property declarations are
persisted but not yet enforced. Independent node rules may agree; conflicting declared property
types or named type constraints use CG03009 in either publication order.

## Example

```csharp
var property = GraphPropertyType.Create("name", DatabaseType.String, required: true);
Console.WriteLine(property.Required); // True
```

## Related reference

[Assembly](index.md) · [GraphNodeType](graph-node-type.md) · [GraphEdgeType](graph-edge-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphPropertyType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphTypeTests.cs`.
