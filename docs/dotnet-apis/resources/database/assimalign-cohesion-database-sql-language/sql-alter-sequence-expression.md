# SqlAlterSequenceExpression

Parsed ALTER SEQUENCE name, existence guard and written options, including RESTART.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlAlterSequenceExpression : SqlQueryExpression
{
    public SqlTableReference Sequence { get; }
    public bool IfExists { get; }
    public IReadOnlyList<SqlSequenceOption> Options { get; }
}
```

## Behavior and exceptions

The parser owns construction. Options retains written order; a RESTART option with null Value
means the configured START. START alone does not restart. Accessors add no validation exceptions.
Execution self-commits and waits for transactions using the sequence. DatabaseException codes
include CS03069 (22023) for options, CS03073 (42P01) for an unguarded absent name, CS04067 (40001)
for replacement during a wait, and CS04003 (25001) inside explicit transactions.
See [ALTER SEQUENCE](../../../../database/sql/language/statements/alter-sequence.md).

## Example

Adapted from SqlSequenceParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "ALTER SEQUENCE IF EXISTS s RESTART WITH 8;");
var alter = (SqlAlterSequenceExpression)statement.SqlExpression;
Console.WriteLine(alter.IfExists); // True
Console.WriteLine(alter.Options[0].Value); // 8
```

After execution against an existing s, its next allocation returns `8`.

## Related reference

[Assembly](index.md) · [SqlSequenceOption](sql-sequence-option.md) · [SqlCreateSequenceExpression](sql-create-sequence-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlAlterSequenceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSequenceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
