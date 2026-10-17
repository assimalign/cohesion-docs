# SqlDefaultExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDefaultExpression : SqlExpression
{

}
```

## Behavior and exceptions

A contextual request for the target column default as a whole INSERT VALUES item or UPDATE SET
value, not a scalar operand. There is no public constructor. Invalid scalar placement reports
SQL0003 before planning. Omitted/default values use one path; missing nullable defaults yield NULL
and NOT NULL failures use COHSQLE024 (23502).

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("INSERT INTO t VALUES (DEFAULT)");
var insert = (SqlInsertExpression)statement.SqlExpression;
Console.WriteLine(insert.Values![0][0] is SqlDefaultExpression); // True
```

## Related reference

[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDefaultExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDefaultParserTests.cs`.
