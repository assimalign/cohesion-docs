# SqlSelectExpression

Parsed SELECT clauses, including the canonical comma-separated FROM join trees.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlSelectExpression : SqlQueryBodyExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public IReadOnlyList<SqlSelectColumn> Columns { get; }
    public IReadOnlyList<SqlFromItem> From { get; }
    public SqlExpression? Where { get; }
    public IReadOnlyList<SqlExpression> GroupBy { get; }
    public bool IsGroupByDistinct { get; }
    public SqlExpression? Having { get; }
    public IReadOnlyList<SqlNamedWindow> Windows { get; }
    public IReadOnlyList<SqlOrderByColumn> OrderBy { get; }
    public SqlExpression? Limit { get; }
    public SqlLimitSyntaxKind? LimitSyntaxKind { get; }
    public SqlExpression? Offset { get; }
    public bool IsDistinct { get; }
}
```

## Behavior and exceptions

WITH and result-tail members are inherited from [SqlQueryBodyExpression](sql-query-body-expression.md).
The root may instead be [SqlSetOperationExpression](sql-set-operation-expression.md); inspect it
before casting to SELECT. A CTE/derived/subquery/INSERT source accepts either body type.

The parser owns construction. From replaces the former single table and flat join shape; consume
each [SqlFromItem](sql-from-item.md), then its [Joins](sql-join-clause.md). JOIN binds more tightly
than comma. Every Table member is a [SqlRelationReference](sql-relation-reference.md), either
SqlTableReference, [SqlDerivedTableReference](sql-derived-table-reference.md), or
[SqlJoinedTableReference](sql-joined-table-reference.md) for an unaliased parenthesized join. Renderer and
persisted expressions use this same shape. Current SQL storage format 31 includes canonical
quoting of newly executable function names and BOTH/LEADING/TRAILING, following format 17 for the
default/missing-value split and format 16 for SEARCH/MATERIALIZED quoting. Older formats are
refused without an upgrade reader. Compiled schema artifacts remain version 10.

The node's accessors add no validation exceptions. Parser.Parse returns diagnostics for invalid
text; SqlQueryRequest.FromSql and session execution reject parser errors with DatabaseParseException.
Unknown/out-of-scope names and invalid USING pairs fail planning with CS01015; duplicate
bindings use CS01019. Qualified `t.*` remains CK00001.

GroupBy retains plain keys and [SqlGroupingSetExpression](sql-grouping-set-expression.md) nodes.
IsGroupByDistinct removes duplicate expanded sets; false preserves them (omitted quantifier or ALL).
Expansion is limited to 4096 sets before deduplication (CS05075, 54001). GROUPING SETS inside
ROLLUP/CUBE is CS00010 (42601). These are engine planning failures, not accessor exceptions.

CommonTables contains [SqlCommonTableExpression](sql-common-table-expression.md) definitions in
declaration order, or an empty list. The setter is internal. Each reachable definition materializes
once before writes; unused definitions bind but do not execute. INSERT's query-source WITH lives
on SelectSource.CommonTables; a leading WITH lives on the INSERT node. See [WITH](../../../../database/sql/language/clauses/with.md).

Limit and Offset hold normalized count expressions for every paging spelling.
[SqlLimitSyntaxKind](sql-limit-syntax-kind.md) distinguishes Limit, Fetch and Top, or null without
a limit. The renderer retains that family and normalizes FETCH to OFFSET before FETCH FIRST and
TOP to parentheses. Mixed families/repeated clauses report CS00003; PERCENT, WITH TIES and DML TOP
report CK00001. Invalid count values report CS02018 naming the written clause.

```csharp
var paging = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM t FETCH NEXT 2 ROWS ONLY OFFSET 1 ROW")).SqlExpression;
Console.WriteLine(paging.LimitSyntaxKind); // Fetch
Console.WriteLine(SqlExpressionRenderer.Render(paging));
// SELECT id FROM t OFFSET 1 FETCH FIRST 2 ROWS ONLY
```

Windows retains [SqlNamedWindow](sql-named-window.md) definitions in declaration order.
Columns and OrderBy may contain [SqlWindowExpression](sql-window-expression.md). An empty
list means no WINDOW clause. See [OVER rules](../../../../database/sql/language/clauses/window.md).

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

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Format 30 added ON UPDATE action metadata in table extension 8.
Current format 31 adds partial-index predicate metadata in index extension 2 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 10.

## Related reference

[Assembly](index.md) · [SELECT](../../../../database/sql/language/statements/select.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSelectExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDataStorageFormatTests.cs`.
