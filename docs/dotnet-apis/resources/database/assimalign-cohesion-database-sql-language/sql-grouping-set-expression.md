# SqlGroupingSetExpression

A parsed GROUP BY grouping element, including an empty or composite set.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlGroupingSetExpression : SqlExpression
{
    public SqlGroupingSetKind Kind { get; }
    public IReadOnlyList<SqlExpression> Items { get; }
}
```

## Behavior and exceptions

The parser owns construction. Kind selects composite, concatenation, prefix or subset expansion;
Items retains ordered children. An empty Composite denotes `()`. SqlSelectExpression.GroupBy
contains these nodes alongside plain keys; IsGroupByDistinct records expansion deduplication.
Accessors do not evaluate expressions or add exceptions.

Engine planning refuses GROUPING SETS inside ROLLUP/CUBE with DatabaseException/CS00010 (42601),
and expansion above 4096 sets before DISTINCT with CS05075 (54001). Reverse nesting is allowed.
See [GROUP BY](../../../../database/sql/language/clauses/group-by.md).

## Example

Adapted from SqlGroupingSetsParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT COUNT(*) FROM t GROUP BY CUBE(a,b);");
var select = (SqlSelectExpression)statement.SqlExpression;
var element = (SqlGroupingSetExpression)select.GroupBy[0];
Console.WriteLine(element.Kind);        // Cube
Console.WriteLine(element.Items.Count); // 2
```

Planning expands four sets: `()`, `(a)`, `(b)`, `(a,b)`.

## Related reference

[Assembly](index.md) · [SqlGroupingSetKind](sql-grouping-set-kind.md) · [SqlSelectExpression](sql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlGroupingSetExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlGroupingSetsParserTests.cs`.
