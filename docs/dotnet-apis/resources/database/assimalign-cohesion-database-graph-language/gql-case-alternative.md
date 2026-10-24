# GqlCaseAlternative

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlCaseAlternative(GqlExpression When, GqlExpression Then)
{
    public string Comparison { get; init; } = "=";
}
```

## Behavior and exceptions

Stores one WHEN/THEN pair. Simple CASE accepts Comparison values =, <>, !=, <, <=, >, >=,
IS NULL or IS NOT NULL. Searched CASE uses When as a Boolean predicate. A parsed comma operand
list becomes ordered alternatives sharing the same Then expression. The record constructor
stores inputs without semantic validation; invalid comparisons, null branches or missing
expressions fail engine planning with CG05001.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (CG07016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/case.md).

## Example

```csharp
var branch = new GqlCaseAlternative(new GqlLiteralExpression(30L), new GqlLiteralExpression(1L))
    { Comparison = ">" };
Console.WriteLine(branch.Comparison); // >
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCaseAlternative.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphValueExpressions.cs`.
