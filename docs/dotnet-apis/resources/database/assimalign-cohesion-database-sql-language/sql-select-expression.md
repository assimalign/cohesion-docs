# SqlSelectExpression

Parsed SELECT clauses, including the canonical comma-separated FROM join trees.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlSelectExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public IReadOnlyList<SqlSelectColumn> Columns { get; }
    public IReadOnlyList<SqlFromItem> From { get; }
    public SqlExpression? Where { get; }
    public IReadOnlyList<SqlExpression> GroupBy { get; }
    public SqlExpression? Having { get; }
    public IReadOnlyList<SqlOrderByColumn> OrderBy { get; }
    public SqlExpression? Limit { get; }
    public SqlExpression? Offset { get; }
    public bool IsDistinct { get; }
}
```

## Behavior and exceptions

The parser owns construction. From replaces the former single table and flat join shape; consume
each [SqlFromItem](sql-from-item.md), then its [Joins](sql-join-clause.md). JOIN binds more tightly
than comma. Every Table member is a [SqlRelationReference](sql-relation-reference.md), either
SqlTableReference or [SqlDerivedTableReference](sql-derived-table-reference.md). Renderer and
persisted expressions use this same shape. SQL row-storage format 16 accounts for the new
canonical quoting of recognized identifiers such as search and materialized; older formats are
refused without an upgrade path. Compiled schema artifacts remain version 7.

The node's accessors add no validation exceptions. Parser.Parse returns diagnostics for invalid
text; SqlQueryRequest.FromSql and session execution reject parser errors with DatabaseParseException.
Unknown/out-of-scope names and invalid USING pairs fail planning with COHSQLE015; duplicate
bindings use COHSQLE019. Qualified `t.*` remains COHDBL001.

CommonTables contains [SqlCommonTableExpression](sql-common-table-expression.md) definitions in
declaration order, or an empty list. The setter is internal. Each reachable definition materializes
once before writes; unused definitions bind but do not execute. INSERT's query-source WITH lives
on SelectSource.CommonTables; a leading WITH lives on the INSERT node. See [WITH](../../../../database/sql/language/clauses/with.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT * FROM a, b LEFT OUTER JOIN c USING (id, name) CROSS JOIN d;");
var select = (SqlSelectExpression)statement.SqlExpression;
Console.WriteLine(select.From.Count); // 2
Console.WriteLine(select.From[1].Joins.Count); // 2
Console.WriteLine(SqlExpressionRenderer.Render(select));
// SELECT * FROM a, b LEFT OUTER JOIN c USING (id, name) CROSS JOIN d
```

## Related reference

[Assembly](index.md) · [SELECT](../../../../database/sql/language/statements/select.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSelectExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDataStorageFormatTests.cs`.
