# GqlRelationshipPattern

A relationship binding, constraint and optional repetition.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlRelationshipPattern(string? Variable, string? Type,
    GqlPatternDirection Direction, IReadOnlyDictionary<string, GqlExpression> Properties)
{
    public GqlQuantifier? Quantifier { get; init; }
    public GqlLabelExpression? LabelExpression { get; init; }
}
```

## Behavior and exceptions

Quantifier retains inclusive repetitions; a named quantified relationship is a group list.
LabelExpression is the full type condition. When present, Type must be null or agree with its
single name; a contradictory representation fails planning with CG05001. The record retains
its supplied references without constructor validation. Invalid direct bounds/insertion quantifiers
also use CG05001 (no SQLSTATE); text insertion quantifiers use CG00002.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a)-[r:T*1..3]->(b) RETURN a")).GqlExpression;
var edge = query.Matches[0].Relationships[0];
Console.WriteLine(edge.Quantifier!.Min); // 1
Console.WriteLine(edge.Quantifier.Max);  // 3
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlRelationshipPattern.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
