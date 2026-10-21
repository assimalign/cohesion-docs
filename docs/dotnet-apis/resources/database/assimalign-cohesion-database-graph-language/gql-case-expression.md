# GqlCaseExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlCaseExpression : GqlExpression
{
    public GqlCaseExpression(GqlExpression? operand, IEnumerable<GqlCaseAlternative> alternatives,
        GqlExpression? otherwise, Location? location = null);
    public GqlExpression? Operand { get; }
    public IReadOnlyList<GqlCaseAlternative> Alternatives { get; }
    public GqlExpression? Otherwise { get; }
}
```

## Behavior and exceptions

A null Operand selects searched CASE; otherwise the operand evaluates once. Alternatives are
copied in source order. The first TRUE match selects Then lazily; otherwise Otherwise evaluates,
or NULL when absent. No common-type coercion is applied; the selected runtime type survives.
Planning validates all bindings and shapes, including skipped arms. Constructors do not validate
semantic shape: null/empty alternatives, invalid Comparison or missing expressions fail planning
with COHDBG001. Evaluation errors use COHDBG013.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (COHDBG016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/case.md).

## Example

```csharp
var one = new GqlLiteralExpression(1L);
var node = new GqlCaseExpression(one, [new GqlCaseAlternative(one, one)], null);
Console.WriteLine(node.Alternatives.Count); // 1
// Equivalent RETURN CASE 1 WHEN 1 THEN 1 END returns Int64 1.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCaseExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphValueExpressions.cs`.
