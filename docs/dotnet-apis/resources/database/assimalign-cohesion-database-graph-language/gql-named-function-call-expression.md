# GqlNamedFunctionCallExpression

A call whose identity and scalar/aggregate kind are resolved by the engine catalog.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlNamedFunctionCallExpression : GqlExpression
{
    public GqlNamedFunctionCallExpression(GraphFunctionName name,
        IEnumerable<GqlExpression> arguments, bool distinct = false, Location? location = null);
    public GraphFunctionName Name { get; }
    public IReadOnlyList<GqlExpression> Arguments { get; }
    public bool Distinct { get; }
}
```

## Behavior and exceptions

Construction validates the identifier (ArgumentException for default/invalid), rejects a null
argument sequence with ArgumentNullException and copies arguments into a read-only list.
It does not invoke a callback. Distinct requests duplicate-input removal for an aggregate.

GqlQueryParser(allowCustomFunctions: true) admits named calls. GraphQueryRequest.FromGql and
ordinary session/wire requests enable this automatically. The engine resolves the catalog
descriptor by case-insensitive name and arity once per statement. Unknown names use CG00010;
arity/placement errors use CG05001, proven incompatible kinds CG03003, dynamic input failures
CG02013 and callback/result-contract failures CG05021. These codes have no SQLSTATE.
NULL handling and result shape come from the registered function metadata.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser(allowCustomFunctions: true)
    .Parse("RETURN developer_echo(42) AS echoed");
var call = (GqlNamedFunctionCallExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(call.Name); // developer_echo
Console.WriteLine(call.Arguments.Count); // 1
```

## Related reference

[Assembly](index.md) · [GraphFunctionName](graph-function-name.md) · [GraphFunction](../assimalign-cohesion-database-graph/graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlNamedFunctionCallExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlNamedFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
