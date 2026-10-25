# GqlLogicalExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlLogicalExpression : GqlExpression
{
    public GqlLogicalExpression(GqlLogicalOperator @operator, IEnumerable<GqlExpression> operands, Location? location = null);
    public GqlLogicalOperator Operator { get; }
    public IReadOnlyList<GqlExpression> Operands { get; }
}
```

## Behavior and exceptions

Operator uses GqlLogicalOperator.And, Or or Xor. Operands are copied into a read-only chain; valid
chains have at least two. AND stops at false, OR at true, XOR propagates unknown. OR/XOR share a
precedence level in text. Undefined operators, null lists/items and short chains fail planning with
CG05001. Deep recursion fails as CG05008 (SQLSTATE 54001), not a fixed count limit.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlLogicalExpression(GqlLogicalOperator.Xor,
    [new GqlLiteralExpression(true), new GqlLiteralExpression(true)]);
Console.WriteLine(node.Operands.Count); // 2
```

## Related reference

[GqlLogicalOperator](gql-logical-operator.md)


[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLogicalExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
