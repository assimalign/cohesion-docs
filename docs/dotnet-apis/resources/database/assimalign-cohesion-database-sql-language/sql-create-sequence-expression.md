# SqlCreateSequenceExpression

Parsed CREATE SEQUENCE name, existence guard and written options.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCreateSequenceExpression : SqlQueryExpression
{
    public SqlTableReference Sequence { get; }
    public bool IfNotExists { get; }
    public IReadOnlyList<SqlSequenceOption> Options { get; }
}
```

## Behavior and exceptions

The parser owns construction; accessors retain syntax without allocating values. Options preserves
written order. Execution resolves defaults, atomically checks IF NOT EXISTS before option validation,
and self-commits. Invalid options use DatabaseException/CS03069 (22023); an unguarded name collision
uses CS03070 (42P07). Explicit transactions return CS04003 (25001) diagnostics, and declared provisioning scripts
use CS03074 (0A000). See [CREATE SEQUENCE](../../../../database/sql/language/statements/create-sequence.md).

## Example

From the SqlSequenceParserTests shape:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE SEQUENCE IF NOT EXISTS dbo.s AS SMALLINT START 3 INCREMENT 2;");
var create = (SqlCreateSequenceExpression)statement.SqlExpression;
Console.WriteLine(create.IfNotExists); // True
Console.WriteLine(create.Options.Count); // 3
```

After executing this statement in a fresh database, `SELECT nextval('s');` returns BIGINT `3`.

## Related reference

[Assembly](index.md) · [SqlSequenceOption](sql-sequence-option.md) · [SqlAlterSequenceExpression](sql-alter-sequence-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCreateSequenceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSequenceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
