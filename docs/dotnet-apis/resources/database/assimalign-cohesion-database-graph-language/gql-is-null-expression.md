# GqlIsNullExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlIsNullExpression : GqlExpression
{
    public GqlIsNullExpression(GqlExpression operand, bool isNegated, Location? location = null);
    public GqlExpression Operand { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

Tests explicit NULL or an absent graph property; IsNegated implements IS NOT NULL. The result is
Boolean. Invalid hand-built operand shapes fail CG05001 at planning. GQL does not use Documents
MISSING.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlIsNullExpression(new GqlLiteralExpression(null), false);
Console.WriteLine(node.IsNegated); // False
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlIsNullExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
