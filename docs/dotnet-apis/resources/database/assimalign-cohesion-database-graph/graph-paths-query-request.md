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

Statement and Parameters are inherited from GraphRequest. Parameters bind case-sensitive bare
names in WHERE and property maps, retaining runtime scalar types. Missing/colliding/unsupported
values fail planning with COHDBG001/014/003, including under EXPLAIN. Unused values are ignored.
Path requests require one bound entity/path from read-only MATCH; scalar/multiple projections, catalog statements and mutations fail before output.

Null statements throw ArgumentNullException. FromGql rejects blank text with ArgumentException,
parser errors with DatabaseParseException carrying complete diagnostics, and exhausted parsing
stack with DatabaseException COHDBG008 (SQLSTATE 54001). A graph session accepts both leaves via
DatabaseLanguageSession<GraphRequest>; other models' requests do not compile.

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
