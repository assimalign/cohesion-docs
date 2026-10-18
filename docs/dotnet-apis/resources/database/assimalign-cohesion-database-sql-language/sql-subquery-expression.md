# SqlSubqueryExpression

A query body used as a scalar expression.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlSubqueryExpression : SqlExpression
{
    public SqlQueryBodyExpression Select { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Select accepts SELECT or a set-operation body. Planning
requires one output column; zero rows produces NULL, one row its value, and multiple rows throw
DatabaseException with the scalar-cardinality message (no dedicated diagnostic code).
Correlated bodies execute in their outer statement context.
The accessor adds no exceptions; invalid text becomes DatabaseParseException at the request/session
boundary. Set width/type mismatches use COHSQLE013/COHSQLE003 (42804 for the latter). Nesting
beyond 32 subquery levels uses COHDBL001. See [subquery diagnostics](../../../../database/sql/language/expressions/subqueries.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT (SELECT id FROM a INTERSECT SELECT id FROM b) FROM target;");
var scalar = (SqlSubqueryExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(scalar.Select is SqlSetOperationExpression); // True
```

With one target row and both a/b containing only id 1, execution returns 1.

## Related reference

[SqlQueryBodyExpression](sql-query-body-expression.md) · [SqlInExpression](sql-in-expression.md) · [SqlExistsExpression](sql-exists-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSubqueryExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSetOperationExecutionTests.cs`.
