# GqlTruthTestExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlTruthTestExpression : GqlExpression
{
    public GqlTruthTestExpression(GqlExpression operand, bool? truthValue, bool isNegated, Location? location = null);
    public GqlExpression Operand { get; }
    public bool? TruthValue { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

TruthValue true/false/null means TRUE/FALSE/UNKNOWN. IsNegated implements IS NOT. A Boolean/NULL
operand yields Boolean; a non-Boolean evaluated operand raises COHDBG013. Invalid AST shapes fail
COHDBG001.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlTruthTestExpression(new GqlLiteralExpression(null), null, false);
Console.WriteLine(node.TruthValue is null); // True
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlTruthTestExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
