# GqlQuerySegment

A sequence of stages between explicit NEXT boundaries.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlQuerySegment(IReadOnlyList<GqlStage> Stages);
```

## Behavior and exceptions

The constructor retains the supplied list and declares no validation exceptions.
GqlQueryExpression.Segments derives read-only segment lists from Stages, excluding NEXT itself.
A WITH stage projects within the same segment. Execution uses the source-ordered Stages list;
Segments provides its grouping for callers. Invalid boundaries/operands are planner errors
(COHDBG001) or text syntax errors (GQL0002); no SQLSTATE is assigned.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1 RETURN x NEXT RETURN x")).GqlExpression;
Console.WriteLine(query.Stages.Count); // 4
Console.WriteLine(query.Segments.Count); // 2
Console.WriteLine(query.Segments[0].Stages.Count); // 2
Console.WriteLine(query.Segments[1].Stages.Count); // 1
```

Execution returns one row containing 1.

## Related reference

[GqlStage](gql-stage.md) · [GqlNextStage](gql-next-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQuerySegment.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
