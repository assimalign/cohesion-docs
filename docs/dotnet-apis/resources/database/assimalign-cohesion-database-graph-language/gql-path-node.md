# GqlPathNode

A node constraint in a source-ordered path.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlPathNode(GqlNodePattern Pattern) : GqlPathElement;
```

## Behavior and exceptions

Pattern retains the node binding and constraints. Construction retains its reference with no
additional validation exceptions. The planner validates the containing path (CG05001 for invalid
shape, no SQLSTATE). A node declared inside a quantified segment is a list-valued group variable.
Repeated node constraints apply at every occurrence.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a)->(b) RETURN a")).GqlExpression;
Console.WriteLine(query.Matches[0].Elements![0] is GqlPathNode); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathNode.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
