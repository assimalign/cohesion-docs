# SqlComparisonQuantifier

Selects how a scalar comparison combines subquery rows.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlComparisonQuantifier { Any, Some, All }
```

## Behavior and exceptions

Any and Some combine comparison results with OR; All combines them with AND. Empty Any/Some
returns FALSE and empty All returns TRUE, including for a NULL operand. UNKNOWN survives unless
a deciding TRUE (Any/Some) or FALSE (All) exists. Enum access adds no exceptions. The parser
retains Some as a distinct written spelling although its execution matches Any. Structural
grouping-key matching also retains that distinction.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("SELECT 2 = ALL (SELECT 2);");
var select = (SqlSelectExpression)statement.SqlExpression;
var comparison = (SqlQuantifiedComparisonExpression)select.Columns[0].Expression;
Console.WriteLine(comparison.Quantifier == SqlComparisonQuantifier.All); // True
```

Execution returns TRUE. See the [truth table](../../../../database/sql/language/expressions/quantified-comparisons.md).

## Related reference

[SqlQuantifiedComparisonExpression](sql-quantified-comparison-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlComparisonQuantifier.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlQuantifiedComparisonParserTests.cs`.
