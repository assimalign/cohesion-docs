# GqlFunctionCallExpression

A resolved scalar call with an immutable argument snapshot.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlFunctionCallExpression : GqlExpression
{
    public GqlFunctionCallExpression(GqlScalarFunction function,
        IEnumerable<GqlExpression> arguments, Location? location = null);
    public GqlScalarFunction Function { get; }
    public IReadOnlyList<GqlExpression> Arguments { get; }
    public bool IsNiladic { get; }
}
```

## Behavior and exceptions

Function uses [GqlScalarFunction](gql-scalar-function.md); aliases resolve to the same identity.
The constructor copies a non-null argument sequence to a read-only list; null is retained for
planner validation rather than throwing. IsNiladic is true for CurrentDate and CurrentTimestamp.
Construction does not evaluate or check signatures. The planner refuses invalid enums/arity or
unbound arguments with CG05001, proven incompatible kinds with CG03003, and dynamic
kind/domain/conversion failures with CG02013. These codes have no assigned SQLSTATE.

Parsed arity uses CG00002; unknown names use CG00010. Every argument evaluates, then a NULL
argument yields NULL before signature kind/domain checks. Clocks share one transaction-start
UTC instant across rows, stages and explicit-transaction statements.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN substring('A😀BC',1,2) AS part, CURRENT_DATE AS d")).GqlExpression;
var call = (GqlFunctionCallExpression)query.Projections[0].Expression;
Console.WriteLine(call.Function); // Substring
Console.WriteLine(call.Arguments.Count); // 3
Console.WriteLine(((GqlFunctionCallExpression)query.Projections[1].Expression).IsNiladic); // True
// Execution returns "😀B" and the transaction's UTC DateOnly.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Scalar functions](../../../../database/graph/language/functions/scalar-functions.md) · [CAST](../../../../database/graph/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlFunctionCallExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
