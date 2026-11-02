# SqlExpressionRenderer

Renders parsed SQL trees as canonical SQL.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## COPY signature

```csharp
public static string Render(SqlCopyToExpression query);
```

## Behavior and exceptions

This overload emits COPY table TO STDOUT WITH a parenthesized option list, without a terminator.
It writes FORMAT, DELIMITER, NULL and HEADER explicitly; CSV also writes effective QUOTE/ESCAPE
and any FORCE_QUOTE selection. UTF-8 encoding is implicit. Identifiers and string values are
escaped for canonical SQL. A null query throws ArgumentNullException. Rendering does not bind
columns or execute the export.

## Example

Language SqlCopyToTests checks canonical round trips:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("COPY t TO STDOUT (FORMAT csv);");
string sql = SqlExpressionRenderer.Render((SqlCopyToExpression)statement.SqlExpression);
var again = (SqlQueryStatement)new SqlQueryParser().Parse(sql);
Console.WriteLine(SqlExpressionRenderer.Render((SqlCopyToExpression)again.SqlExpression) == sql); // True
```

## Related reference

[Assembly](index.md) · [SqlCopyToExpression](sql-copy-to-expression.md) · [SqlCopyOptions](sql-copy-options.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlExpressionRenderer.Copy.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCopyToTests.cs`.
