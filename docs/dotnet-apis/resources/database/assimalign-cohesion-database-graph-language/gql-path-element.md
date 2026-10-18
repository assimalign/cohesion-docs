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
path shape validation belongs to the planner and uses COHDBG001 (no SQLSTATE).

## Example

```csharp
GqlPathElement element = new GqlParenthesizedPathPattern(
    new GqlPathPattern([], []), new GqlQuantifier(2, 2));
Console.WriteLine(element is GqlParenthesizedPathPattern); // True
// This incomplete path is only an AST value; executing it fails COHDBG001.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathElement.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
