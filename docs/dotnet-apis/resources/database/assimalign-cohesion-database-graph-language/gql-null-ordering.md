# GqlNullOrdering

The top-level NULL placement of a [GqlSortItem](gql-sort-item.md).

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlNullOrdering { Default, First, Last }
```

## Behavior and exceptions

Default (0) uses Cypher placement: last ascending and first descending. First (1) and Last (2)
place NULL explicitly regardless of direction. A missing graph property is NULL; GQL has no
MISSING state. These settings apply per key; nested composite NULL values retain the total
value comparison rules. Undefined enum values in a supplied AST cause DatabaseException with
COHDBG001 during planning, with no SQLSTATE assigned. Enum access introduces no exceptions.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN a ORDER BY a.age DESC NULLS LAST, a.name");
Console.WriteLine(statement.GqlExpression.OrderBy[0].NullOrdering); // Last
Console.WriteLine(statement.GqlExpression.OrderBy[1].NullOrdering); // Default
```

## Related reference

[Assembly](index.md) · [GqlSortDirection](gql-sort-direction.md) · [ORDER BY](../../../../database/graph/language/clauses/order-by.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlNullOrdering.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOrderPagingParserTests.cs`.
