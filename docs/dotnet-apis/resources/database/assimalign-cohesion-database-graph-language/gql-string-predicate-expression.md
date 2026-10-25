# GqlStringPredicateExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlStringPredicateExpression : GqlExpression
{
    public GqlStringPredicateExpression(GqlExpression left, GqlStringPredicateOperator @operator, GqlExpression right, Location? location = null);
    public GqlExpression Left { get; }
    public GqlStringPredicateOperator Operator { get; }
    public GqlExpression Right { get; }
}
```

## Behavior and exceptions

Operator uses GqlStringPredicateOperator.StartsWith, EndsWith or Contains. Comparison is ordinal and
case-sensitive; NULL or non-string inputs yield unknown. An undefined enum or malformed operand
fails planning with CG05001.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlStringPredicateExpression(new GqlLiteralExpression("Alice"),
    GqlStringPredicateOperator.StartsWith, new GqlLiteralExpression("Al"));
Console.WriteLine(node.Operator); // StartsWith
```

## Related reference

[GqlStringPredicateOperator](gql-string-predicate-operator.md)


[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStringPredicateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
