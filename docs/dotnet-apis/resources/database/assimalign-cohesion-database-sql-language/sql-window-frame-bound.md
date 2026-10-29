# SqlWindowFrameBound

One inclusive boundary of an explicit window frame.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlWindowFrameBoundKind
{ UnboundedPreceding, Preceding, CurrentRow, Following, UnboundedFollowing }
public sealed class SqlWindowFrameBound
{
    public SqlWindowFrameBoundKind Kind { get; }
    public SqlExpression? Offset { get; }
}
```

## Behavior and exceptions

Construction is internal. Kind retains the written boundary category; Offset retains the
expression for Preceding/Following and is null for other kinds. ROWS offsets count physical rows;
GROUPS counts peer groups and RANGE counts key-domain distances. CURRENT ROW in RANGE/GROUPS
selects the corresponding peer boundary. Bounds clamp to the partition.
Accessors do not execute or throw validation exceptions.

Offsets evaluate once per window execution, shared across partitions, and are not evaluated for
empty input. ROWS/GROUPS accepts signed integers; RANGE uses the ordering key's numeric or
INTERVAL offset domain (CS02074/CS02075 / 0A000; negative or NaN uses CS02076 / 22013). Engine validation/execution raises
DatabaseException with CS02070 (42804) for noninteger ROWS/GROUPS offsets, CS02071 (22004)
for NULL in any mode and CS02072 (22013) for negative ROWS/GROUPS offsets. CS05081 (42P20) prohibits current-query row variables, aggregates
or windows; nested-window validation can instead report CS05077 (42P20). Outer-query values
remain constant for each child-window execution. CS05080 (42P20) rejects invalid category order.
See [frame boundaries](../../../../database/sql/language/clauses/window.md#frame-boundaries-and-defaults).

## Example

Adapted from the window parser's ROWS cases:

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT SUM(v) OVER(ORDER BY k ROWS 2 PRECEDING) FROM t;")).SqlExpression;
var frame = ((SqlWindowExpression)select.Columns[0].Expression).Specification!.Frame!;
Console.WriteLine(frame.Start.Kind);               // Preceding
Console.WriteLine(frame.Start.Offset is not null); // True
Console.WriteLine(frame.End.Kind);                 // CurrentRow
```

## Related reference

[Assembly](index.md) · [SqlWindowFrame](sql-window-frame.md) · [SqlWindowSpecification](sql-window-specification.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowFrameBound.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowFrameTests.cs`.
