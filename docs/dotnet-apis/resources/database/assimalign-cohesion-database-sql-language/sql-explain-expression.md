# SqlExplainExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlExplainExpression : SqlQueryExpression
{
    public SqlQueryExpression Statement { get; }
}
```

## Behavior and exceptions

Statement is the SELECT/INSERT/UPDATE/DELETE tree to plan. The wrapper has no public constructor.
Parsing EXPLAIN DDL, transaction commands, nested EXPLAIN or ANALYZE reports CK00001. Engine
execution returns DatabasePlanDescription and never invokes the wrapped data executor; ordinary
planning errors retain their diagnostics.

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("EXPLAIN DELETE FROM t");
var node = (SqlExplainExpression)statement.SqlExpression;
Console.WriteLine(node.Statement.GetType().Name); // SqlDeleteExpression
```

## Related reference

[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlExplainExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExplainTests.cs`.
