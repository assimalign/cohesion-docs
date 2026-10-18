# GraphRequest

The common typed request root for graph row and path execution.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public abstract class GraphRequest : DatabaseRequest
{
    public GqlQueryStatement Statement { get; }
    public IReadOnlyDictionary<string, object?>? Parameters { get; }
}
```

## Behavior and exceptions

Construction is restricted to the two graph request leaves. Their constructors fix statement,
parameter dictionary and result shape. Statement cannot be null (ArgumentNullException in the leaf
constructor). Parameters are optional and retain the caller's dictionary; planning normalizes by
ordinal bare name and validates referenced values. Collisions are COHDBG014, absent references
COHDBG001 and unsupported/non-finite values COHDBG003. Null is a binding and unused values are
ignored. Parameters can occur in maps, WHERE and RETURN. [Row requests](graph-query-request.md)
support standalone/computed projections and DISTINCT; [path requests](graph-paths-query-request.md)
require one bare entity/path binding, including DISTINCT or star over one named binding.

Referenced list/map parameter trees are snapshotted once per execution, with depth 64, 65,536
values including root and 16 MiB encoded bytes. Invalid dimensions/cycles/leaves or bounds use
COHDBG003 before reads/writes. Stored property-map values remain scalar-only. Wire parameters
use [GraphValueCodec](graph-value-codec.md) only when graph.composite-values is negotiated;
path payloads are unchanged. Unused values remain ignored by engine planning.

## Example

```csharp
GraphRequest request = GraphQueryRequest.FromGql("MATCH (n) WHERE n.age >= $age RETURN n.age",
    new Dictionary<string, object?> { ["age"] = 18 });
Console.WriteLine(request.Parameters!["age"]); // 18 (Int32)
```

## Related reference

[GraphQueryRequest](graph-query-request.md) · [GraphPathsQueryRequest](graph-paths-query-request.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphRequest.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
