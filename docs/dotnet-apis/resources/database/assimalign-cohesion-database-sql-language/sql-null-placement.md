# SqlNullPlacement

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlNullPlacement { Default, First, Last }
```

## Behavior and exceptions

Default records an omitted modifier: NULL first ascending and last descending.
First and Last explicitly place NULL independently of direction. NaN is a non-NULL value.
The enum adds no validation or exceptions. The parser stores it on
[SqlOrderByColumn](sql-order-by-column.md), including keys in STRING_AGG and LISTAGG.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM t ORDER BY id NULLS LAST;");
var key = ((SqlSelectExpression)statement.SqlExpression).OrderBy[0];
Console.WriteLine(key.NullPlacement == SqlNullPlacement.Last); // True
```

## Related reference

[Assembly](index.md) · [SqlOrderByColumn](sql-order-by-column.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlNullPlacement.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlOrderByParserTests.cs`.
