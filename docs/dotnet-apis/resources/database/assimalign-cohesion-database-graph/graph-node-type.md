# GraphNodeType

An immutable node declaration selected by its identifying label.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphNodeType
{
    public string IdentifyingLabel { get; }
    public IReadOnlyList<string> ImpliedLabels { get; }
    public IReadOnlyList<GraphPropertyType> Properties { get; }
    public static GraphNodeType Create(string identifyingLabel,
        IEnumerable<string>? impliedLabels = null, IEnumerable<GraphPropertyType>? properties = null);
}
```

## Behavior and exceptions

Create copies declarations in ordinal order. Null lists mean none. Blank names, repeated implied
labels, implying this type's own identifying label, or null/repeated properties throw ArgumentException.
Names compare ordinally and case-sensitively. Schema publication also refuses an implied label
that identifies another node type (CG03011).

A member node must already carry every implied label and satisfy declared scalar types and
NOT NULL rules; labels are never synthesized. Multiple matching node types all apply. Additional
labels and properties remain permitted. Missing implied labels use CG03012; property violations
use CG03003. Removing an identifying label removes that membership; deleting a node is allowed.
See [publication and existing-data validation](graph-schema.md#graph-type-operations).

## Example

```csharp
var node = GraphNodeType.Create("Person", ["Entity"],
    [GraphPropertyType.Create("name", DatabaseType.String, required: true)]);
Console.WriteLine(node.ImpliedLabels[0]); // Entity
```

## Related reference

[Assembly](index.md) · [GraphType](graph-type.md) · [GraphPropertyType](graph-property-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphNodeType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphTypeTests.cs`.
