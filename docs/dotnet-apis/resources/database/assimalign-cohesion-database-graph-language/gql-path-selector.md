# GqlPathSelector

A MATCH path selection descriptor applied independently per ordered endpoint pair.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlPathSelector(
    GqlPathSelectorKind Kind, long Count = 1, Location? Location = null)
{
    public override string ToString();
}

public enum GqlPathSelectorKind
{
    Any, All, AnyShortest, AllShortest, Shortest, ShortestGroups
}
```

Location is `Assimalign.Cohesion.Database.Language.Location`.

## Behavior and exceptions

Attach through [GqlPathPattern.Selector](gql-path-pattern.md). Count is a positive path count for
Shortest, or a distinct-length-group count for ShortestGroups; other kinds require Count = 1.
AllShortest is equivalent to ShortestGroups with Count = 1. Omitted mode resolves to Trail.
ToString returns the canonical selector spelling without mode or optional PATHS tokens.
The parser retains a prefix span including optional mode/path/group tokens.

Construction stores values without semantic validation. Planning rejects invalid enum/count
values and insertion selectors with CG05001; nested selectors use CK00001. Text invalid counts
use CG00011. Search prefilters naming peer-path-only variables use CG01016. These codes have
no assigned SQLSTATE. Execution selects independently per ordered endpoint pair and incoming row,
including quantified bodies in all four modes. Ties compare relationship sequences, then node
sequences; shortest selectors rank hop count first. Work/64-hop exhaustion uses CG05004 and
retained history/state/output exhaustion uses CG05020, with no partial selection (no SQLSTATE).
Plain EXPLAIN succeeds without executing. See [selector semantics](../../../../database/graph/language/path-selectors.md).

## Example

From the GqlPathSelectorParserTests corpus:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH p = SHORTEST 2 WALK PATHS GROUPS (a)->{0,3}(b) RETURN p");
var path = statement.GqlExpression.Matches[0];
Console.WriteLine(path.Selector!.Kind); // ShortestGroups
Console.WriteLine(path.Selector.Count); // 2
Console.WriteLine(path.Mode);           // Walk
Console.WriteLine(path.Selector);       // SHORTEST 2 GROUPS
```

With an active GraphDatabaseSession, the execution tests use the path request API:

```csharp
await session.ExecuteAsync(
    "INSERT (a:Start)-[:T]->(b)-[:T]->(z:End), (a)-[:T]->(z)",
    cancellationToken: CancellationToken.None);
var result = (GraphPathsQueryResult)await session.ExecuteAsync(
    GraphPathsQueryRequest.FromGql(
        "MATCH p = ANY SHORTEST (:Start)-[:T]->{0,4}(:End) RETURN p"),
    CancellationToken.None);
Console.WriteLine(result.Paths.Count); // 1
Console.WriteLine(result.Paths[0].Relationships.Count); // 1
```

GraphPathsQueryRequest and GraphPathsQueryResult are in
`Assimalign.Cohesion.Database.Graph`. ANY SHORTEST and SHORTEST k use breadth-first
distances for element-local searches and select witnesses lazily, without enumerating every
shortest tie. Path predicates and correlations outside the current hop/endpoints use the
accounted general search. ALL SHORTEST and length groups retain all selected ties.

## Related reference

[Assembly](index.md) · [GqlPathPattern](gql-path-pattern.md) · [GqlPathMode](gql-path-mode.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathSelector.cs` and `GqlPathSelectorKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlPathSelectorParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlPathSelectorPlanningTests.cs` and `GqlPathSelectorExecutionTests.cs`.
