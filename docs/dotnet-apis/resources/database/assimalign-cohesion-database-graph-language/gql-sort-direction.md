# GqlSortDirection

The direction of a [GqlSortItem](gql-sort-item.md).

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlSortDirection { Ascending, Descending }
```

## Behavior and exceptions

Ascending (0) places smaller non-null values first; Descending (1) reverses their order. ASC and
ASCENDING parse to Ascending; DESC and DESCENDING parse to Descending. Equal keys keep input order
in either direction. NULL placement follows [GqlNullOrdering](gql-null-ordering.md).
Undefined enum values on a supplied AST cause DatabaseException with COHDBG001 during planning;
no SQLSTATE is assigned. Enum access introduces no exceptions.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN a ORDER BY a.age ASCENDING, a.name DESCENDING");
Console.WriteLine(statement.GqlExpression.OrderBy[0].Direction); // Ascending
Console.WriteLine(statement.GqlExpression.OrderBy[1].Direction); // Descending
```

## Related reference

[Assembly](index.md) · [GqlSortItem](gql-sort-item.md) · [ORDER BY](../../../../database/graph/language/clauses/order-by.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSortDirection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOrderPagingParserTests.cs`.
