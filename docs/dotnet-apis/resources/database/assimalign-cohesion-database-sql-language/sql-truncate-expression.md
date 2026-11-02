# SqlTruncateExpression

The parsed TRUNCATE target list and effective identity/foreign-key policies.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlTruncateExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlTruncateTable> Tables { get; }
    public bool RestartIdentity { get; }
    public bool Cascade { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain the node through SqlQueryParser. Tables is an immutable ordered
snapshot with each target's ONLY modifier. RestartIdentity false means CONTINUE IDENTITY; Cascade
false means RESTRICT. CommandType is SqlQueryCommandType.Truncate (12). Canonical rendering adds
TABLE and both effective policies. Getters perform no validation or execution.

Execution empties the complete table set with affected count 0. RESTRICT's missing referencing
table uses CS03067 (0A000); explicit-transaction admission uses CS04066 (25001), keeping prior
work and active state; concurrent closure change uses retryable CS04064 (40001). Malformed syntax
uses CS00003. [CTAS](sql-create-table-as-expression.md) shares prepared-owner reclamation for atomic
publication in SQL format 31. See [TRUNCATE](../../../../database/sql/language/statements/truncate.md)
for identity policies, table-level FK closure, snapshot retention and failure semantics.

## Example

From SqlTruncateParserTests' policy round trips; parsing needs no catalog:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "TRUNCATE TABLE ONLY dbo.t, ONLY u RESTART IDENTITY CASCADE");
var truncate = (SqlTruncateExpression)statement.SqlExpression;
Console.WriteLine(truncate.Tables.Count);    // 2
Console.WriteLine(truncate.RestartIdentity); // True
Console.WriteLine(truncate.Cascade);         // True
```

## Related reference

[Assembly](index.md) · [SqlTruncateTable](sql-truncate-table.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlTruncateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTruncateParserTests.cs`.
