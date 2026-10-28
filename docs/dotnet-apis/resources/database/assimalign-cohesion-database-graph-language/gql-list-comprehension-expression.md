# GqlListComprehensionExpression

A list source with a lexical iterator, optional filter and optional projection.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlListComprehensionExpression : GqlExpression
{
    public GqlListComprehensionExpression(string variable, GqlExpression list,
        GqlExpression? predicate = null, GqlExpression? projection = null,
        Location? location = null);
    public string Variable { get; }
    public GqlExpression List { get; }
    public GqlExpression? Predicate { get; }
    public GqlExpression? Projection { get; }
}
```

## Behavior and exceptions

Construction retains the operands and source location without validation or execution. List
evaluates in the enclosing scope; Variable shadows an outer name only inside Predicate/Projection.
Null Predicate accepts every element; null Projection retains the element. Only TRUE passes a
predicate. NULL input yields NULL; empty input yields an empty object?[] without evaluating the body.
See [list comprehensions](../../../../database/graph/language/expressions/list-comprehensions.md).

Engine validation raises DatabaseException with CG01017 for a missing/empty iterator declaration,
CG05001 for unbound names or missing operands, and CG05024 for body aggregates. Non-list input
uses CG02014; non-Boolean filters use CG02013, including statically known types on empty/NULL input.
These codes have no SQLSTATE. Parser diagnostics must be checked before planning.

## Example

Adapted from GqlListComprehensionParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN [x IN $xs WHERE x > 0 | x + 1]");
var expression = (GqlListComprehensionExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(expression.Variable);               // x
Console.WriteLine(expression.Predicate is not null);  // True
Console.WriteLine(expression.Projection is not null); // True
```

With xs = [1,2], execution returns one list cell `[2,3]`.

## Related reference

[Assembly](index.md) · [GqlListExpression](gql-list-expression.md) · [GqlListSliceExpression](gql-list-slice-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlListComprehensionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlListComprehensionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphListComprehensionTests.cs`.
