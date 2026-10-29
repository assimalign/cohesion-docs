# GqlCompositeQuery

A binary read-only UNION operation with isolated query operands.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlCompositeQuery(
    GqlQueryExpression Left, GqlCompositeOperator Operator, GqlQueryExpression Right)
{
    public override string ToString();
}
```

## Behavior and exceptions

The positional record retains Left, Operator and Right without constructor validation.
[GqlQueryExpression.Composite](gql-query-expression.md) retains a standalone binary tree;
[GqlCompositeStage](gql-composite-stage.md) embeds it within a NEXT pipeline. ToString renders
explicit grouping, preserving the operator tree and NEXT precedence; UNION DISTINCT normalizes
to UNION. Parsed chains associate left, while parentheses retain explicit grouping.

Planning rejects modifying operands (CG05029), mismatched column names/order/count (CG01020),
incompatible shapes (CG02017), and mixed UNION/UNION ALL modes (CG00016). Execution also checks
unknown runtime shapes before publishing results. UNION distinct-state exhaustion is CG05030.
These engine failures carry DatabaseException.Diagnostics and no assigned SQLSTATE. See
[UNION](../../../../database/graph/language/clauses/union.md) for snapshot, metadata and budget rules.

## Example

Adapted from GqlCompositeQueryParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN 1 AS x UNION RETURN 2 AS x UNION RETURN 3 AS x");
var root = statement.GqlExpression.Composite!;
Console.WriteLine(root.Operator);                   // Union
Console.WriteLine(root.Left.Composite is not null); // True
Console.WriteLine(root.Right.Composite is null);    // True
```

Inspect statement.Diagnostics before execution. This query returns rows 1, 2 and 3.

## Related reference

[Assembly](index.md) · [GqlCompositeOperator](gql-composite-operator.md) · [GqlQueryExpression](gql-query-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCompositeQuery.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCompositeQueryParserTests.cs`.
