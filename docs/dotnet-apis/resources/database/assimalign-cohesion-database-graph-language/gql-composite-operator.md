# GqlCompositeOperator

Selects a set/multiset operation or whole-result fallback for two read-only results.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlCompositeOperator
{
    Union, UnionAll, Except, ExceptAll, Intersect, IntersectAll, Otherwise
}
```

## Behavior and exceptions

Union removes structurally equal tuples and keeps the first representative; UnionAll preserves
every occurrence. Bare UNION and explicit UNION DISTINCT both parse as Union. Mixing these
modes in one tree, including grouped operands, reports CG00016. Enum access adds no exceptions;
engine validation failures carry DatabaseException.Diagnostics with no assigned SQLSTATE.
Except and Intersect implement DISTINCT by default; their All variants return max(m-n,0) and
min(m,n) occurrences using structural equality. Eligible rows retain their left representation and
order. Otherwise executes the right only when the whole left result is empty; NULL rows and
duplicates count as nonempty. Left errors propagate. All operators have equal precedence and
associate left; every operator binds above NEXT. Quantifiers may mix except UNION/UNION ALL.
EXCEPT/INTERSECT retained-state exhaustion uses CG05032, with no assigned SQLSTATE.

## Example

Using the parser forms covered by GqlCompositeQueryParserTests:

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN 1 AS x UNION ALL RETURN 1 AS x")).GqlExpression;
Console.WriteLine(query.Composite!.Operator); // UnionAll
```

Execution returns two rows containing 1. Inspect parser diagnostics before execution.

Checked against GqlSetQueryParserTests and GraphSetQueryTests:

```csharp
var set = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "FOR x IN [1,1,2] RETURN x EXCEPT ALL RETURN 1 AS x")).GqlExpression;
Console.WriteLine(set.Composite!.Operator); // ExceptAll
```

Execution returns `1, 2`.

## Related reference

[Assembly](index.md) · [GqlCompositeQuery](gql-composite-query.md) · [UNION](../../../../database/graph/language/clauses/union.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCompositeOperator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCompositeQueryParserTests.cs`.
