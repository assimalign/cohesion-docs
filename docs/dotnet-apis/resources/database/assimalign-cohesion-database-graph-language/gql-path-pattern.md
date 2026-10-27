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
    public GqlPathSelector? Selector { get; init; }
    public GqlExpression? Predicate { get; init; }
    public GqlPathMode? Mode { get; init; }
    public override string ToString();
}
```

## Behavior and exceptions

Elements contains [GqlPathElement](gql-path-element.md) leaves in source order. Without parenthesized
elements, Nodes/Relationships retain their chain representation and must agree with Elements when
present. With a parenthesized element, both chain lists are empty. A hand-built chain may leave
Elements null. Variable optionally names the complete MATCH path, independently of group bindings.

Construction retains references without executing or validating. Planning refuses contradictory
representations, invalid bounds, nested repetitions, quantified/parenthesized INSERT paths,
cycles/reused path instances or direct subpath depth above 128 with CG05001 (no SQLSTATE).
Mode is null for implicit Trail; an explicit [GqlPathMode](gql-path-mode.md) applies to the
complete outer path and every segment. Nested modes and unselected unbounded Walk report CK00001.
Undefined modes or insertion modes report CG05001. Group declarations bind ordered lists.

Selector is null for ordinary exhaustive matching; an explicit [GqlPathSelector](gql-path-selector.md)
retains kind/count and prefix location. Predicate is a parenthesized path's search WHERE prefilter.
ToString renders the complete semantic path, including binding, selector, mode, labels/maps,
predicates and repetition bounds, with canonical PATH/GROUPS tokens. Selector or inline WHERE
execution reports CK00001 after binding; EXPLAIN succeeds. Selected prefilters cannot depend on
peer-path-only variables in the same MATCH (CG01016).

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
