# GqlUnaryExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlUnaryExpression : GqlExpression
{
    public GqlUnaryExpression(string @operator, GqlExpression operand, Location? location = null);
    public string Operator { get; }
    public GqlExpression Operand { get; }
}
```

## Behavior and exceptions

Operator supports NOT and unary +/-. NOT requires Boolean/NULL, preserves unknown, and reverses
true/false. Signs promote numeric operands and propagate NULL. Invalid evaluated types, overflow or
non-finite results raise CG02013; unsupported hand-built operator shapes fail CG05001.

Sign promotion is arithmetic only. Numeric comparisons, IN and RETURN DISTINCT compare exact
represented values across runtime types; see [Operators](../../../../database/graph/language/expressions/operators.md).

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlUnaryExpression("NOT", new GqlLiteralExpression(true));
Console.WriteLine(node.Operator); // NOT
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlUnaryExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
