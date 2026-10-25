# GqlPathRelationship

A relationship constraint in a source-ordered path.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlPathRelationship(GqlRelationshipPattern Pattern) : GqlPathElement;
```

## Behavior and exceptions

Pattern retains direction, type/properties and optional repetition bounds. Construction adds no
validation exceptions; invalid containing paths fail planning with CG05001 (no SQLSTATE).
A named quantified relationship binds a source-ordered list. Each relationship identity may occur
only once per matched path; repeated nodes are permitted.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a)-[r:T]->{2}(b) RETURN a")).GqlExpression;
var element = (GqlPathRelationship)query.Matches[0].Elements![1];
Console.WriteLine(element.Pattern.Quantifier!.Min); // 2
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathRelationship.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
