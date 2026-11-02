# SqlNextValueExpression

Parsed standard NEXT VALUE FOR expression.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlNextValueExpression : SqlExpression
{
    public SqlTableReference Sequence { get; }
}
```

## Behavior and exceptions

The parser owns construction; Sequence retains the qualified name. The accessor does not allocate.
Execution returns BIGINT, sharing one allocation for repeated standard occurrences of the same
sequence in each row. DatabaseException codes include CS03076 (0A000) for restricted contexts
or detached evaluation, CS03074 (0A000) for persisted expressions, CS03073 (42P01) for an absent
sequence and CS02002 (22003) for exhaustion. See the complete
[NEXT VALUE FOR context list](../../../../database/sql/language/expressions/next-value-for.md#remarks).

## Example

Adapted from SqlSequenceParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT NEXT VALUE FOR dbo.s, NEXT VALUE FOR dbo.s;");
var select = (SqlSelectExpression)statement.SqlExpression;
Console.WriteLine(select.Columns[0].Expression is SqlNextValueExpression); // True
```

With a fresh default sequence s, execution returns `(1,1)`.

## Related reference

[Assembly](index.md) · [SqlCreateSequenceExpression](sql-create-sequence-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlNextValueExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSequenceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
