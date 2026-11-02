# GqlCompositeStage

A composite binding-table operation inside a NEXT pipeline.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlCompositeStage : GqlStage
{
    public GqlCompositeStage(GqlCompositeQuery query, Location? location = null);
    public GqlCompositeQuery Query { get; }
}
```

## Behavior and exceptions

Construction retains Query and the optional source Location; a null query throws
ArgumentNullException. Each operand receives the same incoming projected binding table in an
isolated scope. The combined result supplies later stages. NEXT has lower precedence than UNION;
parenthesized NEXT stays inside its operand. Proven entity/path kinds and decoded binding names
survive the boundary. Validation and execution use the [UNION rules](../../../../database/graph/language/clauses/union.md).

## Example

Using the composite/NEXT forms covered by GqlUnionFixParserTests:

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN 1 AS x UNION ALL RETURN 2 AS x NEXT RETURN x*10 AS x")).GqlExpression;
var stage = query.Stages.OfType<GqlCompositeStage>().Single();
Console.WriteLine(stage.Query.Operator); // UnionAll
```

Use System.Linq for OfType/Single and inspect parser diagnostics before execution. Execution
returns rows 10 and 20.

## Related reference

[Assembly](index.md) · [GqlStage](gql-stage.md) · [GqlCompositeQuery](gql-composite-query.md) · [GqlNextStage](gql-next-stage.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Composite.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUnionFixParserTests.cs`.
