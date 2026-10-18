# GqlInExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlInExpression : GqlExpression
{
    public GqlInExpression(GqlExpression operand, GqlExpression list, Location? location = null);
    public GqlExpression Operand { get; }
    public GqlExpression List { get; }
}
```

## Behavior and exceptions

Operand tests membership in List. All list items evaluate before membership, so later errors still
fail with COHDBG013. A match wins, otherwise NULL operand/item yields unknown; empty-list IN is
FALSE. List is now any GqlExpression, a breaking API change: parameters, constructors and other
list-valued expressions execute. A NULL list yields NULL, a non-list runtime value COHDBG013.
Missing AST expressions fail planning with COHDBG001.
Numeric membership compares exact represented values across runtime types, agreeing with
comparison operators and RETURN DISTINCT: `1L` matches `1d`, while `0.1m` does not match `0.1d`.
These are .NET value spellings. NULL membership follows the rules above; DISTINCT groups NULLs.
Arithmetic in an operand keeps its separate promotion rules.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlInExpression(new GqlLiteralExpression(1L),
    new GqlListExpression([new GqlLiteralExpression(1L)]));
Console.WriteLine(((GqlListExpression)node.List).Items.Count); // 1
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlInExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlWhereExpressionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
