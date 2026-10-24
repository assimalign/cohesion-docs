# GqlPathElement

The closed record base for source-ordered path elements.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public abstract record GqlPathElement
{
    private protected GqlPathElement();
}
```

## Behavior and exceptions

Only [GqlPathNode](gql-path-node.md), [GqlPathRelationship](gql-path-relationship.md) and
[GqlParenthesizedPathPattern](gql-parenthesized-path-pattern.md) are public leaves. External code
cannot construct the abstract base or derive another leaf. The base adds no validation exceptions;
path shape validation belongs to the planner and uses CG05001 (no SQLSTATE).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) ((x)-[r:T]->(y)){2} (b) RETURN b")).GqlExpression;
GqlPathElement element = query.Matches[0].Elements![1];
Console.WriteLine(element is GqlParenthesizedPathPattern); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathElement.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
