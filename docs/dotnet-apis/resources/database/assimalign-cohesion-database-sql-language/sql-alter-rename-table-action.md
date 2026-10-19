# SqlAlterRenameTableAction

Renames the table within its schema. Object IDs, row versions and index trees survive; incoming and
self-referencing foreign keys and CHECK text publish atomically. Generated constraint/index names
stay unchanged.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterRenameTableAction : SqlAlterAction
{
    public string NewName { get; }
}
```

## Behavior and exceptions

This parser-created action has no public constructor. Its read-only properties retain the parsed
payload; property access performs no execution or semantic validation. It is the Action of a
[SqlAlterTableExpression](sql-alter-table-expression.md).

Collisions use COHSQLE013, system relations COHSQLE021 (25006), and a retained declared-script
table/index claim COHSQLD002.
Engine failures are DatabaseException diagnostics. No SQLSTATE is assigned above unless stated.
All actions retain ownership checks and refuse explicit transactions with COHSQLT003, leaving
that transaction usable. Malformed text returns parser diagnostics; execution rejects it with
DatabaseParseException.

## Example

From SqlAlterTableParserTests; parsing requires no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" RENAME TO next;");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterRenameTableAction)alter.Action;
Console.WriteLine(action.NewName); // next
```

Execution behavior and results appear in [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterRenameTableAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs`.
