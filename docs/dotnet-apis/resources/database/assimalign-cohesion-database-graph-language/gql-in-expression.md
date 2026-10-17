# GqlInExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlInExpression : GqlExpression
{
    public GqlInExpression(GqlExpression operand, GqlListExpression list, Location? location = null);
    public GqlExpression Operand { get; }
    public GqlListExpression List { get; }
}
```

## Behavior and exceptions

Operand tests membership in List. All list items evaluate before membership, so later errors still
fail with COHDBG013. A match wins, otherwise NULL operand/item yields unknown; empty-list IN is
FALSE. Malformed AST operands/list shapes fail COHDBG001.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlInExpression(new GqlLiteralExpression(1L),
    new GqlListExpression([new GqlLiteralExpression(1L)]));
Console.WriteLine(node.List.Items.Count); // 1
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlInExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
