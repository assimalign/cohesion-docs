# SqlInExpression

IN/NOT IN membership over a value list or query body.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlInExpression : SqlExpression
{
    public SqlExpression Operand { get; }
    public IReadOnlyList<SqlExpression>? Values { get; }
    public SqlQueryBodyExpression? Subquery { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

The parser owns construction. Values is used for a list; Subquery accepts SELECT or set bodies.
Redundant query parentheses, as in IN ((SELECT …)), remain membership. A scalar query followed
by arithmetic or a list comma remains a value expression. Membership uses unified output collation;
a match wins, otherwise a NULL operand/candidate yields UNKNOWN, and an empty query gives FALSE
(TRUE for NOT IN), including a NULL operand. The properties add no validation exceptions.

A completed list or subquery membership result may feed another predicate, as in
`1 IN (1) IN (TRUE)` or `1 IN (SELECT id FROM t) = TRUE`. Other non-associative
predicate chains retain SQL0003 at their second operator. See [precedence](../../../../database/sql/language/expressions/operators.md).

Request/session factories reject parser errors with DatabaseParseException. Query width and set
width failures use COHSQLE013; incompatible set types use COHSQLE003 (42804). Existing correlation,
parameter and depth rules apply. See [Subqueries](../../../../database/sql/language/expressions/subqueries.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM a WHERE id IN ((SELECT id FROM b));");
var membership = (SqlInExpression)((SqlSelectExpression)statement.SqlExpression).Where!;
Console.WriteLine(membership.Subquery is SqlSelectExpression); // True
Console.WriteLine(membership.Values is null); // True
```

With a containing 1, 2 and b containing 2, execution returns 2.

## Related reference

[SqlQueryBodyExpression](sql-query-body-expression.md) · [SqlSubqueryExpression](sql-subquery-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlInExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSetOperationParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSetOperationExecutionTests.cs`.
