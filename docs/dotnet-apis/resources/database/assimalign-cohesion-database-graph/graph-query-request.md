# GraphQueryRequest

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphQueryRequest : GraphRequest
{
    public GraphQueryRequest(GqlQueryStatement statement,
        IReadOnlyDictionary<string, object?>? parameters = null);
    public static GraphQueryRequest FromGql(string gql,
        IReadOnlyDictionary<string, object?>? parameters = null);
}
```

## Behavior and exceptions

Statement and Parameters are inherited from GraphRequest. Parameters bind case-sensitive bare
names in WHERE, RETURN and property maps, retaining runtime scalar, list and map types. Missing/colliding/unsupported
values fail planning with CG05001/CG01014/CG03003, including under EXPLAIN. Unused values are ignored.
Embedded row requests support scalar/entity/path projections, standalone RETURN, SHOW, EXPLAIN
and mutations. RETURN supports expressions, aliases, ALL/DISTINCT and star. Duplicate final
column names fail CG01015 before reads/writes, including empty matches and EXPLAIN; no SQLSTATE
is assigned. Only the wire Execute exchange accepts scalar and negotiated composite columns: it rejects entity
results before reads or mutation and directs callers to ExecutePaths. EXPLAIN remains scalar.

Null statements throw ArgumentNullException. FromGql rejects blank text with ArgumentException,
parser errors with DatabaseParseException carrying complete diagnostics, and exhausted parsing
stack with DatabaseException CG05008 (SQLSTATE 54001). A graph session accepts both leaves via
DatabaseLanguageSession<GraphRequest>; other models' requests do not compile.

Referenced list/map parameter trees are snapshotted once per execution, with depth 64, 65,536
values including root and 16 MiB encoded bytes. Invalid dimensions/cycles/leaves or bounds use
CG03003 before reads/writes. Stored property-map values remain scalar-only. Wire parameters
use [GraphValueCodec](../assimalign-cohesion-database-graph-protocol/graph-value-codec.md) only when graph.composite-values is negotiated;
path payloads are unchanged. Unused values remain ignored by engine planning.

## Example

```csharp
var request = GraphQueryRequest.FromGql(
    "MATCH (n:P {name:$name}) RETURN n.name",
    new Dictionary<string, object?> { ["name"] = "Ada" });
var result = await session.ExecuteAsync(request, CancellationToken.None);
// Matches the stored Ada node, with scalar row results.
```

## Related reference

[GraphRequest](graph-request.md) · [Assembly](index.md) · [GQL parameters](../../../../database/graph/language/expressions/parameters.md)

## Lower execution limits

The existing constructors/factories remain. Added overloads accept GraphQueryLimits after parameters:

```csharp
public GraphQueryRequest(GqlQueryStatement statement,
    IReadOnlyDictionary<string, object?>? parameters, GraphQueryLimits limits);
public static GraphQueryRequest FromGql(string gql,
    IReadOnlyDictionary<string, object?>? parameters, GraphQueryLimits limits);
```

Null limits throw ArgumentNullException. Limits is inherited from GraphRequest. Execution requires positive values no greater than the engine ceilings, otherwise CG08021. See [limits DTO](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md).

## Application function binding

FromGql admits [GqlNamedFunctionCallExpression](../assimalign-cohesion-database-graph-language/gql-named-function-call-expression.md)
automatically. Resolution belongs to the executing engine's frozen GraphFunctionCatalog; the
request carries no catalog or callback. Unknown names retain CG00010 during binding/execution,
with ExecutionFailure over the wire and no StatementParseFailed event. Standalone default
GqlQueryParser retains its unknown-name parse diagnostic.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphQueryRequest.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
