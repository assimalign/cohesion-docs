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

Copies Items into a read-only list in source order. Lists execute in every value position and
materialize as object?[], including nested lists/maps and NULL elements. The constructor defers
semantic validation; null Items or a missing item expression fails planning with CG05001.
An empty IN list yields FALSE even for NULL. Evaluated bounds use CG02013 inside the statement
transaction, rolling back writes. See [list values](../../../../database/graph/language/expressions/list-values.md)
for comparison, depth/count/byte limits and the negotiated graph.composite-values capability.
A possible composite wire result without negotiation uses CG07016 before reads/writes.

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
