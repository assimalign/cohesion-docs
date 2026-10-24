# SqlExistsExpression

EXISTS/NOT EXISTS over a query body.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlExistsExpression : SqlExpression
{
    public SqlQueryBodyExpression Subquery { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

Construction is internal. Subquery accepts SELECT or UNION/INTERSECT/EXCEPT bodies in the same
statement context. EXISTS tests whether any row exists, including a row of NULL values; it does
not impose scalar-subquery cardinality. NOT EXISTS negates that Boolean result. Accessors add no
exceptions. Parser errors become DatabaseParseException at request/session boundaries; set width
and type mismatches use CS05013/CS02003 (42804 for the latter), even over empty inputs.
Existing parameter, correlation and depth validation applies.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM b WHERE EXISTS (SELECT id FROM a EXCEPT SELECT id FROM b);");
var exists = (SqlExistsExpression)((SqlSelectExpression)statement.SqlExpression).Where!;
Console.WriteLine(exists.Subquery is SqlSetOperationExpression); // True
```

If a contains 1 and b contains 2, execution returns 2 because the EXCEPT body contains a row.

## Related reference

[SqlQueryBodyExpression](sql-query-body-expression.md) · [Subqueries](../../../../database/sql/language/expressions/subqueries.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlExistsExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSetOperationExecutionTests.cs`.
