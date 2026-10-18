# GqlParenthesizedPathPattern

A path body that may repeat as one segment.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlParenthesizedPathPattern(GqlPathPattern Pattern,
    GqlQuantifier? Quantifier = null) : GqlPathElement;
```

## Behavior and exceptions

A null Quantifier means one occurrence. Iterations join the last node to the next first node,
applying both boundary constraints. Variables declared in a repeated body are ordered group lists;
zero repetitions bind empty lists and conjoin outer endpoints on one node.

Construction retains references without validation exceptions. Nested quantifiers and subpath
assignment/WHERE use COHDBL001 in text. Invalid direct shapes, nesting beyond 128, cycles/reused
path instances and parenthesized INSERT paths use COHDBG001 (no SQLSTATE).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) ((x)-[r:T]->(y)){2} (b) RETURN b")).GqlExpression;
var part = (GqlParenthesizedPathPattern)query.Matches[0].Elements![1];
Console.WriteLine(part.Quantifier!.Max); // 2
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Patterns](../../../../database/graph/language/patterns.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlParenthesizedPathPattern.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQuantifiedPathParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlQuantifiedPathTests.cs`.
