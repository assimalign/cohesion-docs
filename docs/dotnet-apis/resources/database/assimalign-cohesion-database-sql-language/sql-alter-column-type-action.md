# SqlAlterColumnTypeAction

A parsed target type, optional collation and conversion against the original table row.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterColumnTypeAction : SqlAlterAction
{
    public SqlColumnDefinition Column { get; }
    public SqlExpression? UsingExpression { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain the action from SqlQueryParser through
[SqlAlterTableExpression.Action](sql-alter-table-expression.md). Column carries the existing
column name, target type/modifiers and optional CollationName. UsingExpression is null without
USING. These getters neither execute nor validate. This action preserves existing nullability,
identity and constraints rather than replacing the full column definition.

USING binds against every column of the original row, including qualified references. Without
it, the old column is the operand. Old values, USING results and dependent defaults require an
assignment/implicit cast (CS02064 / 42804); an explicit-only conversion needs a written CAST.
USING does not convert the default. A column with no default inherits the target domain default.
Subqueries, aggregates and windows in USING are CK00001. Ordinary name, parameter, conversion,
CHECK/domain, FK and index failures retain their diagnostics.

Execution runs inline under the table's exclusive lock and publishes the replacement atomically.
Binary-compatible widenings without USING keep the heap and index trees. Dependent views refuse
with CS03302 (2BP01), even for same-type USING. Identity targets must remain integer and fit
their retained start/counter (CS03025, no SQLSTATE for definition/range failures). Ownership and
explicit-transaction checks retain CS03066 (42501) and CS04003 (25001). Failures use
DatabaseException.Diagnostics; malformed text returns parser diagnostics and execution rejects it
with DatabaseParseException. See [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md#alter-column-type)
for conversion, defaults, full-definition differences and remaining batch-7 worker handoff.

## Example

Adapted from SqlAlterTableParserTests; parsing needs no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE t ALTER COLUMN v SET DATA TYPE BIGINT USING CAST(t.v AS BIGINT)");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterColumnTypeAction)alter.Action;
Console.WriteLine(action.Column.ColumnName); // v
Console.WriteLine(action.UsingExpression is SqlCastExpression); // True
Console.WriteLine(SqlExpressionRenderer.Render(alter));
// ALTER TABLE t ALTER COLUMN v TYPE BIGINT USING CAST(t.v AS BIGINT)
```

The renderer overload is:

```csharp
public static class SqlExpressionRenderer
{
    public static string Render(SqlAlterTableExpression query);
}
```

It emits canonical TYPE syntax without a terminator, with optional COLLATE/USING and escaped
domain identifiers. Null query throws ArgumentNullException; an action other than
SqlAlterColumnTypeAction throws NotSupportedException. Deep expressions can throw
InsufficientExecutionStackException. Execution examples and results appear in
[ALTER TABLE](../../../../database/sql/language/statements/alter-table.md#convert-using-the-old-row).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [SqlColumnDefinition](sql-column-definition.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterColumnTypeAction.cs` and `SqlExpressionRenderer.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterColumnTypeTests.cs` and `SqlAlterColumnTypeReviewTests.SecondPass.cs`.
