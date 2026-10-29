# SqlWindowFrame

The explicit ROWS or offset-free RANGE frame of a window specification.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlWindowFrameMode { Rows, Range }
public sealed class SqlWindowFrame
{
    public SqlWindowFrameMode Mode { get; }
    public SqlWindowFrameBound Start { get; }
    public SqlWindowFrameBound End { get; }
}
```

## Behavior and exceptions

Construction is internal; parse SQL to obtain a frame through
[SqlWindowSpecification.Frame](sql-window-specification.md). Mode chooses physical rows or
ordering peers. Start and End are inclusive [bounds](sql-window-frame-bound.md). A short
frame has CurrentRow as End. A null specification Frame retains the SQL default; it does not
create an explicit frame node. Accessors add no exceptions.

Binding raises DatabaseException with CS05080 (42P20) for invalid boundary categories and
CS01066 (42P20) when copying a named specification that has an explicit frame. RANGE offsets,
GROUPS and EXCLUDE retain CK00001. Inspect parser diagnostics first. See
[OVER and WINDOW](../../../../database/sql/language/clauses/window.md) for defaults, execution and limits.

## Example

Adapted from SqlWindowParserTests:

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT SUM(v) FILTER(WHERE v>0) OVER w FROM t " +
    "WINDOW w AS(ORDER BY k ROWS BETWEEN 3 PRECEDING AND 2 FOLLOWING);")).SqlExpression;
var frame = select.Windows[0].Specification.Frame!;
Console.WriteLine(frame.Mode);       // Rows
Console.WriteLine(frame.Start.Kind); // Preceding
Console.WriteLine(frame.End.Kind);   // Following
```

## Related reference

[Assembly](index.md) · [SqlWindowSpecification](sql-window-specification.md) · [SqlWindowFrameBound](sql-window-frame-bound.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowFrame.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
