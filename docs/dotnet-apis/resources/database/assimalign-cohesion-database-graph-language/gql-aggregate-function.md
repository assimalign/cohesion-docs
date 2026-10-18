# GqlAggregateFunction

Identifies the six executable graph aggregate operations.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlAggregateFunction { Count, Sum, Avg, Min, Max, CollectList }
```

## Behavior and exceptions

[GqlAggregateExpression](gql-aggregate-expression.md) uses these values. Count returns Int64,
Sum uses Int64/Double/Decimal promotion, Avg Double or Decimal, Min/Max retain the chosen input
type and CollectList returns an object array. collect is an exact parser alias for CollectList.
All operand aggregates skip NULL; Count with no operand counts rows. DISTINCT uses exact numeric
and structural value equality. See [aggregate results](../../../../database/graph/language/functions/aggregate-functions.md).

The enum itself adds no exceptions. A programmatic value outside the enum is COHDBG001 during
planning; numeric failures/overflow are COHDBG013 during evaluation. Wire lists require
GraphProtocol.CompositeValues. Final aggregate path projections retain COHDBG001; intermediate
WITH collect followed by FOR/UNWIND can produce one mandatory final entity/path. Final lists of
nodes/relationships remain unavailable over the wire (COHDBG001; #1662).

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse("RETURN collect(NULL)");
var aggregate = (GqlAggregateExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(aggregate.Function); // CollectList
```

Executing this statement yields one empty list. The constructor form is
`new GqlAggregateExpression(GqlAggregateFunction.Count)`, representing COUNT(*).
Standalone RETURN count(*) yields Int64 1.

## Related reference

[Assembly](index.md) · [GqlAggregateExpression](gql-aggregate-expression.md) · [GqlLanguageProfile](gql-language-profile.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlAggregateFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphAggregateTests.cs`.
