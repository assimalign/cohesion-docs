# SqlWindowExpression

A function call with an OVER reference or inline specification.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlWindowExpression : SqlExpression
{
    public SqlFunctionCallExpression Function { get; }
    public string? WindowName { get; }
    public SqlWindowSpecification? Specification { get; }
}
```

## Behavior and exceptions

Construction is internal; parse SQL to obtain the node. Function retains the call and arguments.
WindowName is the direct `OVER name` reference; Specification holds a parenthesized OVER,
including any copied name. Depth includes call arguments and keys. Accessors add no exceptions.

Inspect parser diagnostics before execution. Invalid call signatures use CS00011 (42601),
placement/missing OVER uses CS05076 (42P20), and nested windows use CS05077 (42P20).
Frames, aggregate windows and FIRST_VALUE/LAST_VALUE/NTH_VALUE use CK00001. Engine failures
carry DatabaseException.Diagnostics. See [all window codes](../../../../database/sql/language/clauses/window.md#diagnostics).

## Example

Adapted from SqlWindowParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT LAG(v,n,0) OVER b FROM t WINDOW a AS(PARTITION BY p),b AS(a ORDER BY k NULLS FIRST);");
var select = (SqlSelectExpression)statement.SqlExpression;
var window = (SqlWindowExpression)select.Columns[0].Expression;
Console.WriteLine(window.Function.FunctionName); // LAG
Console.WriteLine(window.WindowName);            // b
Console.WriteLine(window.Specification is null); // True
```

## Related reference

[Assembly](index.md) · [SqlWindowSpecification](sql-window-specification.md) · [SqlNamedWindow](sql-named-window.md) · [SqlSelectExpression](sql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
