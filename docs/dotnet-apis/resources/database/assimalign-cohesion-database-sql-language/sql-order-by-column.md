# SqlOrderByColumn

One query or aggregate-input ordering key with independently recorded NULL placement.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlOrderByColumn
{
    public SqlExpression Expression { get; }
    public bool IsDescending { get; }
    public SqlNullPlacement NullPlacement { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Expression retains aliases/ordinals as written;
binding resolves them later. Aggregate-input integers remain constants. NullPlacement is
[Default, First or Last](sql-null-placement.md). Rendering preserves an explicit modifier even
when it equals the default and emits none when omitted. Accessors declare no exceptions.
Malformed placement produces CS00003 in parser diagnostics.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM t ORDER BY id DESC NULLS FIRST;");
var key = ((SqlSelectExpression)statement.SqlExpression).OrderBy[0];
Console.WriteLine(key.IsDescending);  // True
Console.WriteLine(key.NullPlacement); // First
```

## Related reference

[Assembly](index.md) · [SqlSelectExpression](sql-select-expression.md) · [SqlFunctionCallExpression](sql-function-call-expression.md) · [ORDER BY](../../../../database/sql/language/clauses/order-by.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlOrderByColumn.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlOrderByParserTests.cs`.
