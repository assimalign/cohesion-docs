# SqlGroupingSetKind

Identifies how a parsed GROUP BY element expands.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlGroupingSetKind
{
    Composite,
    Sets,
    Rollup,
    Cube
}
```

## Behavior and exceptions

Composite combines children into one set; no children means `()`. Sets concatenates child
expansions. Rollup includes successive prefixes and the empty prefix. Cube includes every subset.
The enum itself adds no exceptions; planning applies the
[grouping-set limits and nesting rules](../../../../database/sql/language/clauses/group-by.md).

## Example

Adapted from SqlGroupingSetsParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT COUNT(*) FROM t GROUP BY ROLLUP(a,b);");
var select = (SqlSelectExpression)statement.SqlExpression;
var grouping = (SqlGroupingSetExpression)select.GroupBy[0];
Console.WriteLine(grouping.Kind == SqlGroupingSetKind.Rollup); // True
```

Planning expands `(a,b)`, `(a)`, `()`.

## Related reference

[Assembly](index.md) · [SqlGroupingSetExpression](sql-grouping-set-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlGroupingSetKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlGroupingSetsParserTests.cs`.
