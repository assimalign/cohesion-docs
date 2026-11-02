# SqlWindowExclusion

Selects members removed after computing a window frame's boundaries.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlWindowExclusion { NoOthers, CurrentRow, Group, Ties }
```

## Behavior and exceptions

[SqlWindowFrame.Exclusion](sql-window-frame.md) retains this policy. NoOthers keeps every member;
CurrentRow removes only the current row; Group removes all its ordering peers including itself;
Ties removes other peers and keeps the current row if it belongs to the frame. Without ORDER BY,
the partition is one peer group. The policy affects aggregate and frame-value functions, while
ranking and LAG/LEAD ignore frames. Reading the enum adds no exceptions.

## Example

Adapted from SqlWindowParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT SUM(v) OVER(ORDER BY k RANGE BETWEEN 1 PRECEDING AND 2 FOLLOWING EXCLUDE TIES) FROM t;");
var select = (SqlSelectExpression)statement.SqlExpression;
var window = (SqlWindowExpression)select.Columns[0].Expression;
Console.WriteLine(window.Specification!.Frame!.Exclusion); // Ties
```

Inspect statement.Diagnostics before execution. See [frame exclusions](../../../../database/sql/language/clauses/window.md#exclusion).

## Related reference

[Assembly](index.md) · [SqlWindowFrame](sql-window-frame.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowExclusion.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
