# SqlSetQuantifier

The set quantifier written inside a function call.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlSetQuantifier
{
    Default,
    All,
    Distinct
}
```

## Behavior and exceptions

Default and All pass every input row; All retains explicit syntax when rendered. Distinct passes
each coerced argument tuple once under its collations and exact numeric equality. The aggregate's
NULL rule applies. This enum describes call inputs, independently of SELECT/set-operation ALL
and DISTINCT. It has no methods or constructor exceptions. Applying an explicit quantifier to
a scalar function uses COHSQLE032 (no SQLSTATE); a quantified star uses SQL0003.

## Example

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT COUNT(DISTINCT x), SUM(ALL x) FROM t;")).SqlExpression;
Console.WriteLine(((SqlFunctionCallExpression)select.Columns[0].Expression).Quantifier); // Distinct
Console.WriteLine(((SqlFunctionCallExpression)select.Columns[1].Expression).Quantifier); // All
```

For x values 1, 1, 2 and NULL, the result is `(2, 4)`; SUM returns Decimal.

## Related reference

[Assembly](index.md) · [SqlFunctionCallExpression](sql-function-call-expression.md) · [Aggregate functions](../../../../database/sql/language/functions/aggregate-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSetQuantifier.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`.
