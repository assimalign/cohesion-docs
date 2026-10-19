# SqlAlterSetNotNullAction

Requires non-null live and future values. The executor validates current live rows, including frozen
missing fields, under the table lock before publishing. Failure preserves the old definition; no row
rewrite is performed.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterSetNotNullAction : SqlAlterAction
{
    public string ColumnName { get; }
}
```

## Behavior and exceptions

This parser-created action has no public constructor. Its read-only properties retain the parsed
payload; property access performs no execution or semantic validation. It is the Action of a
[SqlAlterTableExpression](sql-alter-table-expression.md).

A NULL fails NOT NULL validation with COHSQLE024 (23502), naming the column.
Engine failures are DatabaseException diagnostics. No SQLSTATE is assigned above unless stated.
All actions retain ownership checks and refuse explicit transactions with COHSQLT003, leaving
that transaction usable. Malformed text returns parser diagnostics; execution rejects it with
DatabaseParseException.

## Example

From SqlAlterTableParserTests; parsing requires no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" ALTER COLUMN \"old name\" SET NOT NULL;");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterSetNotNullAction)alter.Action;
Console.WriteLine(action.ColumnName); // old name
```

Execution behavior and results appear in [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterSetNotNullAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs`.
