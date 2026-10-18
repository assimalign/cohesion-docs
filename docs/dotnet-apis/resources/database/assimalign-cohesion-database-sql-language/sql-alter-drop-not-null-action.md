# SqlAlterDropNotNullAction

Allows future NULL values on an ordinary column. The default and frozen missing value remain unchanged. A primary-key or identity column cannot become nullable.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterDropNotNullAction : SqlAlterAction
{
    public string ColumnName { get; }
}
```

## Behavior and exceptions

This parser-created action has no public constructor. Its read-only properties retain the parsed
payload; property access performs no execution or semantic validation. It is the Action of a
[SqlAlterTableExpression](sql-alter-table-expression.md).

Primary-key columns use SQL0003 and identity columns COHSQLE025.
Engine failures are DatabaseException diagnostics. No SQLSTATE is assigned above unless stated.
All actions retain ownership checks and refuse explicit transactions with COHSQLT003, leaving
that transaction usable. Malformed text returns parser diagnostics; execution rejects it with
DatabaseParseException.

## Example

From SqlAlterTableParserTests; parsing requires no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" ALTER COLUMN \"old name\" DROP NOT NULL;");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterDropNotNullAction)alter.Action;
Console.WriteLine(action.ColumnName); // old name
```

Execution behavior and results appear in [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterDropNotNullAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs`.
