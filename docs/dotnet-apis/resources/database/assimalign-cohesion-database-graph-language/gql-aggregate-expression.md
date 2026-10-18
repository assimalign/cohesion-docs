# GqlAggregateExpression

An aggregate call with one operand or the special COUNT(*) shape.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlAggregateExpression : GqlExpression
{
    public GqlAggregateExpression(GqlAggregateFunction function,
        GqlExpression? operand = null, bool distinct = false, Location? location = null);
    public GqlAggregateFunction Function { get; }
    public GqlExpression? Operand { get; }
    public bool Distinct { get; }
}
```

## Behavior and exceptions

The constructor stores arguments without executing or validating them. Function selects a
[GqlAggregateFunction](gql-aggregate-function.md). Null Operand denotes COUNT(*); Distinct removes
equal non-NULL operands and false is the default ALL behavior. collect and collect_list both parse
to CollectList. Function names remain usable graph variable/property/alias names outside calls.

The planner rejects unknown enum values, null operands outside non-DISTINCT Count, nested or
misplaced aggregates, ungrouped expressions and invalid path requests with COHDBG001. Malformed
text arguments, SUM(*) and COUNT(DISTINCT/ALL *) use GQL0002. Referenced parameters validate even
on empty input. SUM/AVG numeric errors and bounded collection failures use COHDBG013; these codes
have no assigned SQLSTATE. Missing composite negotiation uses COHDBG016 before wire output.
The language [aggregate reference](../../../../database/graph/language/functions/aggregate-functions.md)
defines NULL/empty identities, exact numeric promotion, grouping and result types.

## Example

From GqlAggregateParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN count(DISTINCT a.name)");
var aggregate = (GqlAggregateExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(aggregate.Function); // Count
Console.WriteLine(aggregate.Distinct); // True
Console.WriteLine(aggregate.Operand is GqlPropertyExpression); // True
```

After inserting the three [Person rows](../../../../database/graph/language/functions/aggregate-functions.md#examples),
this execution example follows GraphAggregateWireTests and preserves scale 1:

```csharp
await using var result = (DatabaseResultSet)await session.ExecuteAsync(
    "MATCH (a:Person) RETURN sum($d) AS total",
    new Dictionary<string, object?> { ["d"] = 0.1m }, CancellationToken.None);
await foreach (var row in result.GetRowsAsync()) {
    decimal total = (decimal)row.GetValue(0)!;
    Console.WriteLine(total); // 0.3
    Console.WriteLine((decimal.GetBits(total)[3] >> 16) & 0xff); // 1
}
```

## Related reference

[Assembly](index.md) · [GqlQueryExpression.GroupBy](gql-query-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlAggregateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphAggregateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphAggregateWireTests.cs`.
