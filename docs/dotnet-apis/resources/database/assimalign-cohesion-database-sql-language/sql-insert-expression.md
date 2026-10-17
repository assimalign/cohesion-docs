# SqlInsertExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlInsertExpression : SqlQueryExpression
{
    public SqlTableReference Table { get; }
    public IReadOnlyList<string>? Columns { get; }
    public IReadOnlyList<IReadOnlyList<SqlExpression>>? Values { get; }
    public SqlSelectExpression? SelectSource { get; }
    public bool IsDefaultValues { get; }
}
```

## Behavior and exceptions

IsDefaultValues identifies INSERT DEFAULT VALUES, inserting one row with every column omitted. Both Values and SelectSource are null in that form; the optional column list is retained. Ordinary VALUES may contain SqlDefaultExpression items. The parser owns construction. Invalid DEFAULT placement is SQL0003; NOT NULL failures use COHSQLE024 (23502).

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("INSERT INTO t DEFAULT VALUES");
var node = (SqlInsertExpression)statement.SqlExpression;
Console.WriteLine(node.IsDefaultValues); // True
```

## Related reference

[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlInsertExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDefaultParserTests.cs`.
