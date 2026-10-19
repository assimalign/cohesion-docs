# GqlQuantifier

Inclusive repetition bounds on an edge or parenthesized path.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlQuantifier(int Min, int? Max);
```

## Behavior and exceptions

Null Max means unbounded. Construction has no validation exceptions; the parser/planner validate
bounds. Text reversed bounds and ISO {0} use GQL0002; overflow uses GQL0004; repetition-bound sums
above 64 use GQL0005. Direct AST invalid bounds use COHDBG001 (no SQLSTATE). An unbounded trail
needing a qualifying 65th edge uses COHDBG004. Zero repetition emits empty group lists.
See [GqlRelationshipPattern](gql-relationship-pattern.md) and
[GqlParenthesizedPathPattern](gql-parenthesized-path-pattern.md).

## Example

```csharp
var oneOrMore = new GqlQuantifier(1, null);
var exactlyTwo = new GqlQuantifier(2, 2);
Console.WriteLine(oneOrMore.Max is null); // True
Console.WriteLine(exactlyTwo.Min); // 2
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQuantifier.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
