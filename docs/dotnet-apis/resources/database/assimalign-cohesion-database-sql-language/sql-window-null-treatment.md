# SqlWindowNullTreatment

Retains the NULL treatment written on a navigation or frame-value window call.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlWindowNullTreatment { Default, Ignore, Respect }
```

## Behavior and exceptions

[SqlWindowExpression.NullTreatment](sql-window-expression.md) is Default when omitted, with
RESPECT NULLS behavior. Ignore skips NULL argument values when selecting a member; Respect
counts them. LAG, LEAD, FIRST_VALUE, LAST_VALUE and NTH_VALUE accept the modifier. LAG/LEAD offset
zero still selects the current row. Other functions fail binding with DatabaseException and
CS05088 (SQLSTATE 42601). Enum access itself adds no exceptions. FROM FIRST/LAST remains CK00001.

## Example

From the forms in SqlWindowParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT FIRST_VALUE(v) IGNORE NULLS OVER(ORDER BY k GROUPS CURRENT ROW EXCLUDE GROUP) FROM t;");
var select = (SqlSelectExpression)statement.SqlExpression;
var window = (SqlWindowExpression)select.Columns[0].Expression;
Console.WriteLine(window.NullTreatment); // Ignore
```

Inspect statement.Diagnostics before execution. See [window functions](../../../../database/sql/language/functions/window-functions.md).

## Related reference

[Assembly](index.md) · [SqlWindowExpression](sql-window-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlWindowNullTreatment.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
