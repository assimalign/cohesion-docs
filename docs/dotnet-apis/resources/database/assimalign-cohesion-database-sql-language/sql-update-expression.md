# SqlUpdateExpression

Parsed UPDATE target, predicate and statement-level WITH definitions.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlUpdateExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public SqlTableReference Table { get; }
    public IReadOnlyList<SqlFromItem> From { get; }
    public IReadOnlyList<SqlSelectColumn>? Returning { get; }
    public IReadOnlyList<SqlAssignment> Assignments { get; }
    public SqlExpression? Where { get; }
}
```

## Behavior and exceptions

Construction and CommonTables mutation are internal to the parser. Table is always a stored-table
target, even when a CTE has the same name. Where accepts correlated and uncorrelated subqueries;
all candidate predicates evaluate under the statement snapshot before writes. Assignments retain SET order; SET subqueries remain CK00001.
[CommonTables](sql-common-table-expression.md) binds in declaration order and materializes reachable
bodies once before mutation. Unused definitions bind but never execute.

Accessors add no validation exceptions. Malformed text yields parser diagnostics; typed request
creation rejects parser errors with DatabaseParseException. Duplicate/future CTE names use
CS01026 during planning, unknown/hidden columns CS01015 and invalid grouped references
CS05017. Execution retains ordinary destination/constraint failures and statement rollback.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "WITH s AS (SELECT id FROM t) UPDATE t SET id = id + 100 WHERE id IN (SELECT id FROM s);");
var node = (SqlUpdateExpression)statement.SqlExpression;
Console.WriteLine(node.CommonTables[0].Name); // s
Console.WriteLine(node.Table.TableName); // t
```

With stored t containing IDs 1, 2 and 3, execution affects three rows and leaves 101, 102 and 103.

Returning is null when absent, otherwise the parser-owned SELECT projection list. Execution
binds only the target row and expands star in table order. INSERT/UPDATE project stored new values;
DELETE projects directly targeted old values. Projection errors roll the statement back. Aggregates
use CS05017; subqueries/windows and OUTPUT/OLD/NEW/INTO forms use CK00001.
See [RETURNING](../../../../database/sql/language/clauses/returning.md).

```csharp
var returningStatement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "UPDATE t SET id = 1 RETURNING id;");
Console.WriteLine(((SqlUpdateExpression)returningStatement.SqlExpression).Returning!.Count); // 1
```

## Related reference

[Assembly](index.md) · [SqlCommonTableExpression](sql-common-table-expression.md) · [UPDATE](../../../../database/sql/language/statements/update.md)

## Source binding

From holds comma-separated sources and their join trees, or an empty list when absent.
Source forms parse and bind but execution reports CK00001 in this batch before callbacks or
writes. EXPLAIN describes the source plan. With sources present, bare RETURNING * binds
target columns followed by visible source columns; the target-only execution description
above applies to statements without sources. See [UPDATE](../../../../database/sql/language/statements/update.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlUpdateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteExecutionTests.cs`.
