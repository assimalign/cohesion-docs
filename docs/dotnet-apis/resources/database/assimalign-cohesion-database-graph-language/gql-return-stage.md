# GqlReturnStage

A RETURN or Cypher WITH projection with its own grouping, ordering, paging and scope.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlReturnStage(IReadOnlyList<GqlProjection> Projections,
    bool IsWith = false, Location? Location = null) : GqlStage(Location)
{
    public GqlSetQuantifier SetQuantifier { get; init; }
    public bool ReturnStar { get; init; }
    public IReadOnlyList<GqlExpression>? GroupBy { get; init; }
    public IReadOnlyList<GqlSortItem> OrderBy { get; init; } = [];
    public GqlExpression? Offset { get; init; }
    public GqlExpression? Limit { get; init; }
    public GqlExpression? Predicate { get; init; }
}
```

## Behavior and exceptions

IsWith distinguishes intermediate WITH from RETURN. SetQuantifier defaults to All; GroupBy null
means implicit keys and an empty list means GROUP BY (). Predicate belongs only to WITH and
executes after sorting/paging; a hand-built RETURN predicate uses COHDBG001. A statement cannot
end with WITH or place NEXT immediately after it. Constructors retain supplied operands without
semantic validation; null/missing mandatory operands fail planning with COHDBG001.

WITH and RETURN … NEXT carry aliases/bare variables only, including decoded names under star.
Duplicate carried names use COHDBG015. Unaliased computed items are accepted without a later
binding. Entity kinds, optionality, composite possibilities and Int64 COUNT metadata survive.
Without DISTINCT/aggregation, WITH ORDER BY/WHERE also sees incoming bindings; projected aliases
shadow them. DISTINCT/aggregation restricts WHERE to projected names. Aggregate calls in WHERE
use COHDBG001; aggregate aliases remain usable. Counts retain COHDBG013 validation. These codes
have no assigned SQLSTATE. See [WITH](../../../../database/graph/language/clauses/with.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "FOR x IN [3,1,2] WITH x ORDER BY x OFFSET 1 LIMIT 1 WHERE x > 1 RETURN x")).GqlExpression;
var stage = (GqlReturnStage)query.Stages[1];
Console.WriteLine(stage.IsWith); // True
Console.WriteLine(stage.OrderBy.Count); // 1
Console.WriteLine(stage.Predicate is not null); // True
```

Execution returns 2. Each projection stage owns its tail independently of later RETURN stages.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [GqlProjection](gql-projection.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Return.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
