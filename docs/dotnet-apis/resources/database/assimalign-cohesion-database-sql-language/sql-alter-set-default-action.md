# SqlAlterSetDefaultAction

Retains the column and parsed default expression. The engine accepts signed/typed literals or
column-free Immutable scalar calls, checking literal conversion/bounds and function result
assignment compatibility before publication. Later omitted-column/DEFAULT writes use it; frozen
old-row missing values are unchanged. An explicit NULL can hide a domain default.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterSetDefaultAction : SqlAlterAction
{
    public string ColumnName { get; }
    public SqlExpression DefaultValue { get; }
}
```

## Behavior and exceptions

This parser-created action has no public constructor. Its read-only properties retain the parsed
payload; property access performs no execution or semantic validation. It is the Action of a
[SqlAlterTableExpression](sql-alter-table-expression.md).

Invalid default forms or incompatible result types use COHSQLE013; Stable/Volatile calls use
COHDBL001 until #1121. Identity columns refuse this facet with COHSQLE025. Conversion failures
retain their normal codes.
Engine failures are DatabaseException diagnostics. No SQLSTATE is assigned above unless stated.
All actions retain ownership checks and refuse explicit transactions with COHSQLT003, leaving
that transaction usable. Malformed text returns parser diagnostics; execution rejects it with
DatabaseParseException.

## Example

From SqlAlterTableParserTests; parsing requires no stored table:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE \"my schema\".\"my table\" ALTER COLUMN \"old name\" SET DEFAULT -7;");
var alter = (SqlAlterTableExpression)statement.SqlExpression;
var action = (SqlAlterSetDefaultAction)alter.Action;
Console.WriteLine(action.ColumnName); // old name
```

Execution behavior and results appear in [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

## Related reference

[Assembly](index.md) · [SqlAlterTableExpression](sql-alter-table-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterSetDefaultAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAlterTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs`.
