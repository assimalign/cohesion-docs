# GqlListExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlListExpression : GqlExpression
{
    public GqlListExpression(IEnumerable<GqlExpression> items, Location? location = null);
    public IReadOnlyList<GqlExpression> Items { get; }
}
```

## Behavior and exceptions

Copies Items into a read-only list in source order. The list executes only as IN's right operand. A null list, null item or use outside IN is rejected by the planner with COHDBG001. An empty IN list is valid and yields FALSE even for NULL.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlListExpression([new GqlLiteralExpression(1L)]);
Console.WriteLine(node.Items.Count); // 1
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlListExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
