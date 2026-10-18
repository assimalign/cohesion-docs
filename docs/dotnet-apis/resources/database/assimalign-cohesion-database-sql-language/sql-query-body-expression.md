# SqlQueryBodyExpression

The common SELECT or set-operation body accepted wherever SQL expects a query.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public abstract class SqlQueryBodyExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public IReadOnlyList<SqlOrderByColumn> OrderBy { get; }
    public SqlExpression? Limit { get; }
    public SqlLimitSyntaxKind? LimitSyntaxKind { get; }
    public SqlExpression? Offset { get; }
}
```

## Behavior and exceptions

Construction is private protected; setters are internal. [SqlSelectExpression](sql-select-expression.md)
and [SqlSetOperationExpression](sql-set-operation-expression.md) inherit the WITH list and result tail.
TOP belongs to a SELECT operand; an unparenthesized trailing ORDER BY/paging belongs to the combined body.
Accessors add no validation exceptions. Parser errors become DatabaseParseException at execution;
set width/type/order failures use COHSQLE013/003/018/019 during planning.

The following properties now accept this base instead of SqlSelectExpression:

| Consumer | Member |
| --- | --- |
| SqlCommonTableExpression | Body |
| SqlDerivedTableReference | Query |
| SqlInsertExpression | SelectSource (nullable) |
| [SqlSubqueryExpression](sql-subquery-expression.md) | Select |
| [SqlExistsExpression](sql-exists-expression.md) | Subquery |
| [SqlInExpression](sql-in-expression.md) | Subquery (nullable; Values is used for a value list) |

Inspect the body type before accessing SELECT-only Columns/From. Unknown literals, common output
types, NULL equality and query positions follow the [set-operation reference](../../../../database/sql/language/clauses/set-operations.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM a UNION ALL SELECT id FROM b ORDER BY 1 LIMIT 2;");
var body = (SqlQueryBodyExpression)statement.SqlExpression;
Console.WriteLine(body.OrderBy.Count); // 1
Console.WriteLine(body is SqlSetOperationExpression); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCommonTableExpression](sql-common-table-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlQueryBodyExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSetOperationParserTests.cs`.
