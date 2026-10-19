# SqlAlterTableExpression

A parser-created ALTER TABLE target and one typed action.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterTableExpression : SqlQueryExpression
{
    public SqlTableReference Table { get; }
    public SqlAlterAction Action { get; }
}
public abstract class SqlAlterAction { } // private protected constructor
```

## Behavior and exceptions

There is no public constructor. Table retains the schema-qualified target; Action retains the
single parsed operation. Existing ADD/DROP column/constraint and full-definition ALTER actions
are joined by:

- [SqlAlterRenameTableAction](sql-alter-rename-table-action.md) — NewName.
- [SqlAlterRenameColumnAction](sql-alter-rename-column-action.md) — ColumnName and NewName.
- [SqlAlterSetDefaultAction](sql-alter-set-default-action.md) — ColumnName and DefaultValue.
- [SqlAlterDropDefaultAction](sql-alter-drop-default-action.md) — ColumnName.
- [SqlAlterSetNotNullAction](sql-alter-set-not-null-action.md) — ColumnName.
- [SqlAlterDropNotNullAction](sql-alter-drop-not-null-action.md) — ColumnName.

Parsing returns diagnostics for malformed syntax (SQL0003) or unsupported actions/lists (COHDBL001).
Requests reject parser errors with DatabaseParseException. Planning/execution retains table and
column diagnostics (COHSQLE014/015), system protection (COHSQLE021, 25006), schema ownership and
explicit-transaction refusal (COHSQLT003). Action pages document additional failures.
DDL returns a successful command with AffectedCount zero. Properties introduce no exceptions.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" RENAME TO next;");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
Console.WriteLine(alter.Table.SchemaName); // my schema
Console.WriteLine(((SqlAlterRenameTableAction)alter.Action).NewName); // next
```

## Related reference

[Assembly](index.md) · [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlAlterTableExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
