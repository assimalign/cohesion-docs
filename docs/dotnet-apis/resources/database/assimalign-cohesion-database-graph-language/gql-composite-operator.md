# GqlCompositeOperator

Selects distinct or bag composition for two read-only query results.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlCompositeOperator { Union, UnionAll }
```

## Behavior and exceptions

Union removes structurally equal tuples and keeps the first representative; UnionAll preserves
every occurrence. Bare UNION and explicit UNION DISTINCT both parse as Union. Mixing these
modes in one tree, including grouped operands, reports CG00016. Enum access adds no exceptions;
engine validation failures carry DatabaseException.Diagnostics with no assigned SQLSTATE.
EXCEPT, INTERSECT and OTHERWISE remain CK00001 and have no member here.

## Example

Using the parser forms covered by GqlCompositeQueryParserTests:

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN 1 AS x UNION ALL RETURN 1 AS x")).GqlExpression;
Console.WriteLine(query.Composite!.Operator); // UnionAll
```

Execution returns two rows containing 1. Inspect parser diagnostics before execution.

## Related reference

[Assembly](index.md) · [GqlCompositeQuery](gql-composite-query.md) · [UNION](../../../../database/graph/language/clauses/union.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCompositeOperator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCompositeQueryParserTests.cs`.
