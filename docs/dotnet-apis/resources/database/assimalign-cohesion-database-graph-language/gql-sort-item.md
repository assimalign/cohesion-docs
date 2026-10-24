# GqlSortItem

One ORDER BY expression, direction and NULL placement.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSortItem(GqlExpression Expression,
    GqlSortDirection Direction = GqlSortDirection.Ascending,
    GqlNullOrdering NullOrdering = GqlNullOrdering.Default);
```

## Behavior and exceptions

The record stores its arguments without validation and provides generated init properties,
deconstruction and value equality. [GqlQueryExpression.OrderBy](gql-query-expression.md) retains
keys in priority order. [Direction](gql-sort-direction.md) defaults to ascending and
[NULL placement](gql-null-ordering.md) to last ascending/first descending. Keys can use RETURN
aliases; DISTINCT limits references to projected values. Equal keys preserve input order.

The planner rejects null keys/expressions or undefined enum values with DatabaseException carrying
CG05001. Hidden DISTINCT bindings and unbound names/parameters use the same code, with no
assigned SQLSTATE. Evaluated key errors use CG02013. Sort preparation and comparisons observe
cancellation. Keys are evaluated once per retained row, with nested map keys cached in ordinal order.

## Example

From GqlOrderPagingParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN a ORDER BY a.age DESC NULLS LAST, a.name OFFSET @start LIMIT $rows");
GqlSortItem key = statement.GqlExpression.OrderBy[0];
Console.WriteLine(key.Direction);    // Descending
Console.WriteLine(key.NullOrdering); // Last
Console.WriteLine(statement.GqlExpression.OrderBy[1].NullOrdering); // Default
```

## Related reference

[Assembly](index.md) · [ORDER BY](../../../../database/graph/language/clauses/order-by.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSortItem.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOrderPagingParserTests.cs`.
