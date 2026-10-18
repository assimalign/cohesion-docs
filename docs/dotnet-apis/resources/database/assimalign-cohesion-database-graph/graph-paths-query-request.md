# GraphPathsQueryRequest

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphPathsQueryRequest : GraphRequest
{
    public GraphPathsQueryRequest(GqlQueryStatement statement,
        IReadOnlyDictionary<string, object?>? parameters = null);
    public static GraphPathsQueryRequest FromGql(string gql,
        IReadOnlyDictionary<string, object?>? parameters = null);
}
```

## Behavior and exceptions

Statement and Parameters are inherited from GraphRequest. Parameters bind case-sensitive bare names
in WHERE, RETURN and property maps, retaining runtime scalar, list and map types. Missing/colliding/unsupported values
fail planning with COHDBG001/014/003, including under EXPLAIN. Unused values are ignored. Path
requests require one bare bound entity/path from read-only composition, optionally DISTINCT, or star
over one named binding. Scalar/computed/multiple and multi-binding star projections, catalog
statements and mutations fail with COHDBG001 before output. Shape checks precede duplicate-name
checks. DISTINCT uses node/relationship identity or ordered path identities, including traversal
direction; independent materializations compare equal. A returned variable supplied only by optional
clauses fails with COHDBG001 during planning; it must be bound or constrained by a mandatory clause.
For example, `MATCH (a:Person) OPTIONAL MATCH (a)-[:KNOWS]->(b) RETURN a` permits a path
request, while the same request returning `b` is refused even if every person has a match.

Null statements throw ArgumentNullException. FromGql rejects blank text with ArgumentException,
parser errors with DatabaseParseException carrying complete diagnostics, and exhausted parsing stack
with DatabaseException COHDBG008 (SQLSTATE 54001). A graph session accepts both leaves via
DatabaseLanguageSession<GraphRequest>; other models' requests do not compile.

Referenced list/map parameter trees are snapshotted once per execution, with depth 64, 65,536
values including root and 16 MiB encoded bytes. Invalid dimensions/cycles/leaves or bounds use
COHDBG003 before reads/writes. Stored property-map values remain scalar-only. Wire parameters
use [GraphValueCodec](graph-value-codec.md) only when graph.composite-values is negotiated;
path payloads are unchanged. Unused values remain ignored by engine planning.

## Example

```csharp
var request = GraphPathsQueryRequest.FromGql(
    "MATCH (n:P {name:$name}) RETURN n",
    new Dictionary<string, object?> { ["name"] = "Ada" });
var result = await session.ExecuteAsync(request, CancellationToken.None);
// Matches the stored Ada node, with path results.
```

## Related reference

[GraphRequest](graph-request.md) · [Assembly](index.md) · [GQL parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphPathsQueryRequest.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
