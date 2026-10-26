# SqlQuantifiedComparisonExpression

A scalar comparison against ANY, SOME or ALL rows of a subquery.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlQuantifiedComparisonExpression : SqlExpression
{
    public SqlExpression Operand { get; }
    public SqlBinaryOperator Operator { get; }
    public SqlComparisonQuantifier Quantifier { get; }
    public SqlQueryBodyExpression Subquery { get; }
}
```

## Behavior and exceptions

The parser owns construction; there is no public constructor. Operand retains the scalar left
side, Operator one of the six comparisons, Quantifier the written ANY/SOME/ALL spelling, and
Subquery the complete SELECT or set body. Rendering preserves the quantifier spelling. Property
access adds no validation exceptions. Parser diagnostics retain CS00003 for row values and
CK00001 for arrays or excluded positions. Planning requires one child column (CS05013) and
uses ordinary name binding (CS01015); execution rejects compared incompatible non-NULL values
with CS02055 (42804). See [quantified comparisons](../../../../database/sql/language/expressions/quantified-comparisons.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT 2 = SOME (SELECT 1 UNION ALL SELECT 2);");
var select = (SqlSelectExpression)statement.SqlExpression;
var comparison = (SqlQuantifiedComparisonExpression)select.Columns[0].Expression;
Console.WriteLine(comparison.Operator); // Equal
Console.WriteLine(comparison.Quantifier); // Some
Console.WriteLine(comparison.Subquery is SqlSetOperationExpression); // True
```

Execution returns one TRUE Boolean cell. The parser tests round-trip every comparison operator
and quantifier with a complete set-operation body.

## Related reference

[SqlComparisonQuantifier](sql-comparison-quantifier.md) · [SqlQueryBodyExpression](sql-query-body-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlQuantifiedComparisonExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlQuantifiedComparisonParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Quantified.cs`.
