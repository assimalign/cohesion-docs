# GqlCoalesceExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlCoalesceExpression : GqlExpression
{
    public GqlCoalesceExpression(IEnumerable<GqlExpression> operands, Location? location = null);
    public IReadOnlyList<GqlExpression> Operands { get; }
}
```

## Behavior and exceptions

Copies operands in source order. Evaluation stops at the first non-NULL value, retaining its
runtime type; all NULL values yield NULL. This is a case abbreviation, outside the function
registry. The constructor defers semantic validation. Fewer than two operands, a null list or a
missing expression fails planning with CG05001. Planning checks skipped bindings and shapes;
evaluated value failures use CG02013.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (CG07016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/coalesce.md).

## Example

```csharp
var node = new GqlCoalesceExpression([
    new GqlLiteralExpression(null), new GqlLiteralExpression(2L)]);
Console.WriteLine(node.Operands.Count); // 2
// RETURN COALESCE(NULL, 2) returns Int64 2.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCoalesceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphValueExpressions.cs`.
