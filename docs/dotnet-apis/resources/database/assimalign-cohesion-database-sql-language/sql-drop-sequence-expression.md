# SqlDropSequenceExpression

Parsed sequence name list, existence guard and dependency policy.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDropSequenceExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlTableReference> Sequences { get; }
    public bool IfExists { get; }
    public bool Cascade { get; }
}
```

## Behavior and exceptions

The parser owns construction and retains names in written order. Cascade is false for omitted or
explicit RESTRICT. RESTRICT refuses dependent column defaults with CS03080 (2BP01); CASCADE
atomically clears them. Identity-owned sequences refuse even with CASCADE. Execution validates the full list before one commit and waits for users of the sequences.
Accessors add no exceptions. Engine DatabaseException codes include CS03073 (42P01) for an
unguarded missing name and CS04067 (40001) for a replacement. Explicit transactions return
CS04003 (25001) diagnostics. See [DROP SEQUENCE](../../../../database/sql/language/statements/drop-sequence.md).

## Example

From SqlSequenceParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "DROP SEQUENCE IF EXISTS s, dbo.t CASCADE;");
var drop = (SqlDropSequenceExpression)statement.SqlExpression;
Console.WriteLine(drop.Sequences.Count); // 2
Console.WriteLine(drop.IfExists && drop.Cascade); // True
```

Execution removes any existing listed sequences atomically.

## Related reference

[Assembly](index.md) · [SqlCreateSequenceExpression](sql-create-sequence-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDropSequenceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSequenceParserTests.cs`.
