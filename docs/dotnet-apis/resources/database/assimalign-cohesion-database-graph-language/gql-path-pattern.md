# GqlPathPattern

A path of source-ordered nodes, relationships and parenthesized segments.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlPathPattern(IReadOnlyList<GqlNodePattern> Nodes,
    IReadOnlyList<GqlRelationshipPattern> Relationships)
{
    public IReadOnlyList<GqlPathElement>? Elements { get; init; }
    public string? Variable { get; init; }
}
```

## Behavior and exceptions

Elements contains [GqlPathElement](gql-path-element.md) leaves in source order. Without parenthesized
elements, Nodes/Relationships retain their chain representation and must agree with Elements when
present. With a parenthesized element, both chain lists are empty. A hand-built chain may leave
Elements null. Variable optionally names the complete MATCH path, independently of group bindings.

Construction retains references without executing or validating. Planning refuses contradictory
representations, invalid bounds, nested repetitions, quantified/parenthesized INSERT paths,
cycles/reused path instances or direct subpath depth above 128 with COHDBG001 (no SQLSTATE).
Runtime matching produces relationship-unique trails; group declarations bind ordered lists.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH p = (a) ((x)-[r:T]->(y)){2} (b) RETURN p")).GqlExpression;
var path = query.Matches[0];
Console.WriteLine(path.Variable); // p
Console.WriteLine(path.Nodes.Count); // 0
Console.WriteLine(path.Elements!.Count); // 3
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathPattern.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
