# SqlCommonTableExpression

One named non-recursive query definition in a statement's WITH list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCommonTableExpression
{
    public string Name { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlQueryBodyExpression Body { get; }
    public Location Location { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Name is schema-less; Columns is empty when omitted, or
contains complete positional output renames. Body sees earlier definitions. Location covers the
definition. Accessors add no validation exceptions. CommonTables on SELECT/INSERT/UPDATE/DELETE
is an empty list when WITH is absent; its setter is internal.

All definitions bind, even unused ones. Duplicate names and unresolved future references use
COHSQLE026 without an assigned SQLSTATE; a stored table with a future/self name resolves normally.
Output width or blank names use COHSQLE013. Parser diagnostics reject recursive/nested/modifying
bodies and materialization hints with COHDBL001. SqlQueryRequest.FromSql rejects parser errors with
DatabaseParseException; semantic errors arise during planning. Reachable definitions materialize
once before writes. See [WITH](../../../../database/sql/language/clauses/with.md) for snapshot and scope rules.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "WITH a (n) AS (SELECT 1 AS id) SELECT n FROM a;");
var select = (SqlSelectExpression)statement.SqlExpression;
var cte = select.CommonTables[0];
Console.WriteLine(cte.Name);       // a
Console.WriteLine(cte.Columns[0]); // n
Console.WriteLine(((SqlSelectExpression)cte.Body).From.Count); // 0
```

Execution returns one row containing Int64 1.

Body may also be [SqlSetOperationExpression](sql-set-operation-expression.md). For example,
`WITH a AS (SELECT 1 AS n UNION ALL SELECT 2 AS n) SELECT n FROM a;` has a set body and returns
1 and 2. Each definition's root operands may omit FROM; nested children retain ordinary rules.

## Related reference

[SqlSelectExpression](sql-select-expression.md) · [SqlInsertExpression](sql-insert-expression.md) · [SqlUpdateExpression](sql-update-expression.md) · [SqlDeleteExpression](sql-delete-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCommonTableExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCteParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.Binding.cs`.
