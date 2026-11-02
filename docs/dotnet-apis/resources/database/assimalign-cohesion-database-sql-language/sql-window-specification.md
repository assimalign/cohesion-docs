# SqlWindowSpecification

Partition and ordering keys, optionally copied from a named window.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlWindowSpecification
{
    public string? ExistingWindow { get; }
    public IReadOnlyList<SqlExpression> PartitionBy { get; }
    public IReadOnlyList<SqlOrderByColumn> OrderBy { get; }
}
```

## Behavior and exceptions

Construction is internal. Lists retain the written keys; empty lists omit that clause.
ExistingWindow identifies the copied name, or null for a fresh specification. Binding resolves
inherited keys without changing these syntax lists. A copy cannot specify PARTITION BY and
can add ORDER BY only if its base has none (CS01066, 42P20). An unknown/forward reference is
CS01065 (42704). Accessors add no exceptions; engine failures use DatabaseException.Diagnostics.

## Example

Adapted from SqlWindowParserTests:

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT LEAD(v) OVER (a ORDER BY k) FROM t WINDOW a AS (PARTITION BY p);")).SqlExpression;
var spec = ((SqlWindowExpression)select.Columns[0].Expression).Specification!;
Console.WriteLine(spec.ExistingWindow);    // a
Console.WriteLine(spec.PartitionBy.Count); // 0 (inherited during binding)
Console.WriteLine(spec.OrderBy.Count);     // 1
```

## Related reference

[Assembly](index.md) · [SqlWindowExpression](sql-window-expression.md) · [SqlNamedWindow](sql-named-window.md) · [OVER rules](../../../../database/sql/language/clauses/window.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowSpecification.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
