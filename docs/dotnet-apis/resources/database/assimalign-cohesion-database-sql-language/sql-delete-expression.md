# SqlDeleteExpression

Parsed DELETE target, predicate and statement-level WITH definitions.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDeleteExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public SqlTableReference Table { get; }
    public IReadOnlyList<SqlFromItem> Using { get; }
    public IReadOnlyList<SqlSelectColumn>? Returning { get; }
    public SqlExpression? Where { get; }
}
```

## Behavior and exceptions

Construction and CommonTables mutation are internal to the parser. Table is always a stored-table
target, even when a CTE has the same name. Where accepts correlated and uncorrelated subqueries;
all candidate predicates evaluate under the statement snapshot before writes.
[CommonTables](sql-common-table-expression.md) binds in declaration order and materializes reachable
bodies once before mutation. Unused definitions bind but never execute.

Accessors add no validation exceptions. Malformed text yields parser diagnostics; typed request
creation rejects parser errors with DatabaseParseException. Duplicate/future CTE names use
CS01026 during planning, unknown/hidden columns CS01015 and invalid grouped references
CS05017. Execution retains ordinary destination/constraint failures and statement rollback.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "WITH s AS (SELECT id FROM t) DELETE FROM t WHERE id IN (SELECT id FROM s);");
var node = (SqlDeleteExpression)statement.SqlExpression;
Console.WriteLine(node.CommonTables[0].Name); // s
Console.WriteLine(node.Table.TableName); // t
```

With stored t containing IDs 1, 2 and 3, execution affects three rows and leaves t empty.

Returning is null when absent, otherwise the parser-owned SELECT projection list. Execution
without source clauses binds only the target row and expands star in table order. INSERT/UPDATE project stored new values;
DELETE projects directly targeted old values. Projection errors roll the statement back. Aggregates
use CS05017; subqueries/windows and OUTPUT/OLD/NEW/INTO forms use CK00001.
See [RETURNING](../../../../database/sql/language/clauses/returning.md).

```csharp
var returningStatement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "DELETE FROM t RETURNING id;");
Console.WriteLine(((SqlDeleteExpression)returningStatement.SqlExpression).Returning!.Count); // 1
```

## Related reference

[Assembly](index.md) · [SqlCommonTableExpression](sql-common-table-expression.md) · [DELETE](../../../../database/sql/language/statements/delete.md)

## Source binding

Using holds comma-separated sources and their join trees, or an empty list when absent.
Sources execute as a target-outer association join. WHERE and RETURNING can read source columns.
Each physical target is deleted once even with several matching source associations. RETURNING
uses old target values and one unspecified collected source association; bare star expands target
columns then visible sources in written USING order, retaining JOIN USING merges. EXPLAIN
describes access paths and join algorithms. See [DELETE](../../../../database/sql/language/statements/delete.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDeleteExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.cs`.
