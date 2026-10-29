# SqlAlterIdentityAction

One ALTER COLUMN identity operation, including combined mode and sequence-option changes.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterIdentityAction : SqlAlterAction
{
    public string ColumnName { get; }
    public string Operation { get; }
    public SqlIdentityDefinition? Identity { get; }
    public bool? IsAlways { get; }
    public IReadOnlyList<SqlSequenceOption> Options { get; }
    public bool IfExists { get; }
}
```

## Behavior and exceptions

Construction is internal; read SqlAlterTableExpression.Action after parsing. Operation is ADD,
DROP, SET GENERATED or OPTIONS. Identity carries the ADD declaration. IsAlways carries a new
generation mode, or null when unchanged. Combined mode/option changes use OPTIONS and retain
IsAlways separately. Options contains explicit sequence changes; RESTART without a value has
a null Value. IfExists guards DROP IDENTITY only.

Accessors add no exceptions. Inspect parser diagnostics before execution. Engine failures use
DatabaseException.Diagnostics: CS03078 (55000) for ADD on a nullable column, CS03079 (55000)
for a required identity that is absent, CS03080 (2BP01) for dependencies blocking sequence
removal, and CS03081 (55000) for invalid ownership. Invalid declarations retain CS03025;
explicit-transaction DDL uses CS04003 (25001).

## Example

Adapted from SqlIdentityParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER TABLE t ALTER id SET GENERATED ALWAYS SET INCREMENT BY 2 RESTART WITH 10;");
var action = (SqlAlterIdentityAction)((SqlAlterTableExpression)statement.SqlExpression).Action;
Console.WriteLine(action.Operation); // OPTIONS
Console.WriteLine(action.IsAlways);  // True
Console.WriteLine(action.Options.Count); // 2
```

Execution changes the generation mode and increment and makes the next generated value 10,
preserving existing rows. See [ALTER behavior](../../../../database/sql/language/clauses/identity.md#alter-and-backfill).

## Related reference

[Assembly](index.md) · [SqlIdentityDefinition](sql-identity-definition.md) · [SqlSequenceOption](sql-sequence-option.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlAlterIdentityAction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIdentityParserTests.cs`.
