# SqlAlterRenameColumnAction

Renames one column while preserving its physical ordinal. Primary-key/index column lists,
local/incoming/self-referencing foreign keys and bound CHECK identifiers change atomically. Literal
text, function names, type names and collations in CHECKs retain their meaning.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterRenameColumnAction : SqlAlterAction
{
    public string ColumnName { get; }
    public string NewName { get; }
}
```

## Behavior and exceptions

This parser-created action has no public constructor. Its read-only properties retain the parsed
payload; property access performs no execution or semantic validation. It is the Action of a
[SqlAlterTableExpression](sql-alter-table-expression.md).

A missing column uses CS01015; a conflicting new name (including the same name) uses CS05013.
Engine failures are DatabaseException diagnostics. No SQLSTATE is assigned above unless stated.
All actions retain ownership checks and refuse explicit transactions with CS04003, leaving
that transaction usable. Malformed text returns parser diagnostics; execution rejects it with
DatabaseParseException.

## Example

From SqlAlterTableParserTests; parsing requires no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" RENAME COLUMN \"old name\" TO \"new name\";");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterRenameColumnAction)alter.Action;
Console.WriteLine(action.NewName); // new name
```

Execution behavior and results appear in [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterRenameColumnAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs`.
