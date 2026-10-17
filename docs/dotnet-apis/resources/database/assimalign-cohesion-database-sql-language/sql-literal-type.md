# SqlLiteralType

## Signature

```csharp
public enum SqlLiteralType
{
    String, Integer, Float, Null, Boolean, Date, Time, Timestamp,
    TimestampWithTimeZone, Interval, Binary
}
```

## Behavior and exceptions

Typed literal kinds select strict readers for canonical SqlLiteralExpression.Value text. Float retains exact Decimal semantics; national strings normalize to String. The enum adds no exceptions; malformed literal content is SQL0008 at parse time and unsupported year-month intervals are COHDBL001.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE TABLE t (d DATE DEFAULT DATE '2024-01-01')");
var table = (SqlCreateTableExpression)statement.SqlExpression;
Console.WriteLine(((SqlLiteralExpression)table.Columns[0].DefaultValue!).LiteralType); // Date
```

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLiteralType.cs`.
