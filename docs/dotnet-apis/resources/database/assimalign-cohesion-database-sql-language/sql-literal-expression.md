# SqlLiteralExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlLiteralExpression : SqlExpression
{
    public string Value { get; }
    public SqlLiteralType LiteralType { get; }
}
```

## Behavior and exceptions

Value contains unquoted text. Temporal, day-time interval and binary values use parser-normalized canonical text; LiteralType selects the reader. National strings become ordinary String literals. SqlLiteralType adds Date, Time, Timestamp, TimestampWithTimeZone, Interval and Binary alongside String/Integer/Float/Null/Boolean; Float still evaluates as exact Decimal. No public constructor is exposed. Malformed typed contents report SQL0008; unsupported year-month intervals report COHDBL001.

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("CREATE TABLE t (d DATE DEFAULT DATE '2024-01-01')");
var table = (SqlCreateTableExpression)statement.SqlExpression;
var node = (SqlLiteralExpression)table.Columns[0].DefaultValue!;
Console.WriteLine(node.LiteralType); // Date
Console.WriteLine(node.Value); // 2024-01-01
```

## Related reference

[SqlLiteralType](sql-literal-type.md)


[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlLiteralExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTypedLiteralTests.cs`.
