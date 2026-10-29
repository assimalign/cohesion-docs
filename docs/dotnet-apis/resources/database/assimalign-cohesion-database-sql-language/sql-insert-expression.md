# SqlInsertExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlInsertExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public SqlTableReference Table { get; }
    public IReadOnlyList<SqlSelectColumn>? Returning { get; }
    public SqlOnConflictClause? OnConflict { get; }
    public SqlIdentityOverride Overriding { get; }
    public IReadOnlyList<string>? Columns { get; }
    public IReadOnlyList<IReadOnlyList<SqlExpression>>? Values { get; }
    public SqlQueryBodyExpression? SelectSource { get; }
    public bool IsDefaultValues { get; }
}
```

## Behavior and exceptions

Overriding is None by default, System for OVERRIDING SYSTEM VALUE, or User for OVERRIDING
USER VALUE. It applies before conflict arbitration for all source forms. See
[SqlIdentityOverride](sql-identity-override.md) for assignment validation and exceptions.

SelectSource accepts a SELECT or [set-operation body](sql-query-body-expression.md), with the same
atomic INSERT/default/constraint rules. For example, parsing `INSERT INTO t SELECT id FROM a UNION ALL SELECT id FROM b;`
produces SqlSetOperationExpression in SelectSource. Width/type mismatches fail before writes.

IsDefaultValues identifies INSERT DEFAULT VALUES, inserting one row with every column omitted. Both
Values and SelectSource are null in that form; the optional column list is retained. Ordinary VALUES
may contain SqlDefaultExpression items. The parser owns construction. Invalid DEFAULT placement is
CS00003; NOT NULL failures use CS03024 (23502).

OnConflict is null for an ordinary INSERT, otherwise the parser-owned
[SqlOnConflictClause](sql-on-conflict-clause.md). It applies to VALUES, DEFAULT VALUES and query
sources. Execution binds an arbiter and the original target plus EXCLUDED proposal; RETURNING
projects only inserted/post-update target rows and counts them together. Missing arbiters use
CS03035 (42P10); repeated row effects use CS05036 (21000). See
[ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md) for constraint and retry rules.

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

CommonTables contains [SqlCommonTableExpression](sql-common-table-expression.md) definitions in
declaration order, or an empty list. The setter is internal. Each reachable definition materializes
once before writes; unused definitions bind but do not execute. INSERT's query-source WITH lives
on SelectSource.CommonTables; a leading WITH lives on the INSERT node. See [WITH](../../../../database/sql/language/clauses/with.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("INSERT INTO t DEFAULT VALUES");
var node = (SqlInsertExpression)statement.SqlExpression;
Console.WriteLine(node.IsDefaultValues); // True
```

Returning is null when absent, otherwise the parser-owned SELECT projection list. Execution
binds only the target row and expands star in table order. INSERT/UPDATE project stored new values;
DELETE projects directly targeted old values. Projection errors roll the statement back. Aggregates
use CS05017; windows use CS05076 (42P20); subqueries and OUTPUT/OLD/NEW/INTO forms use CK00001.
See [RETURNING](../../../../database/sql/language/clauses/returning.md).

```csharp
var returningStatement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1) RETURNING id;");
Console.WriteLine(((SqlInsertExpression)returningStatement.SqlExpression).Returning!.Count); // 1
```

```csharp
var upsert = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1, 20) ON CONFLICT (id) DO UPDATE SET age = EXCLUDED.age RETURNING id;");
var conflict = ((SqlInsertExpression)upsert.SqlExpression).OnConflict!;
Console.WriteLine(conflict.TargetKind); // Columns
Console.WriteLine(conflict.Action); // DoUpdate
```

## Related reference

[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlInsertExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDefaultParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlInsertParserTests.cs`.
