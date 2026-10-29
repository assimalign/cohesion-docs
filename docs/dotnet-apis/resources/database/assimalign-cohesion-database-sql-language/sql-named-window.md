# SqlNamedWindow

A WINDOW name AS (specification) definition.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlNamedWindow
{
    public string Name { get; }
    public SqlWindowSpecification Specification { get; }
}
```

## Behavior and exceptions

Construction is internal. SqlSelectExpression.Windows retains definitions in declaration order.
Names resolve case-insensitively; a definition can copy only an earlier definition. Accessors
add no exceptions. Engine binding reports duplicate names as CS01064 (42P20), unknown/forward
names as CS01065 (42704), and invalid inheritance as CS01066 (42P20), through
DatabaseException.Diagnostics. Unused definitions still validate.

## Example

Adapted from SqlWindowParserTests:

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT LAG(v,n,0) OVER b FROM t WINDOW a AS(PARTITION BY p),b AS(a ORDER BY k NULLS FIRST);")).SqlExpression;
Console.WriteLine(select.Windows.Count); // 2
Console.WriteLine(select.Windows[1].Name); // b
Console.WriteLine(select.Windows[1].Specification.ExistingWindow); // a
```

## Related reference

[Assembly](index.md) · [SqlWindowExpression](sql-window-expression.md) · [SqlWindowSpecification](sql-window-specification.md) · [WINDOW](../../../../database/sql/language/clauses/window.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlNamedWindow.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
