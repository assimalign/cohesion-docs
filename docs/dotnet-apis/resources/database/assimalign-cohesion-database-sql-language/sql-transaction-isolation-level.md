# SqlTransactionIsolationLevel

SQL isolation spellings retained by the transaction AST.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlTransactionIsolationLevel
{
    ReadUncommitted,
    ReadCommitted,
    RepeatableRead,
    Serializable
}
```

## Behavior and exceptions

ReadUncommitted and ReadCommitted map to the kernel's IsolationLevel.ReadCommitted, with a fresh
committed snapshot per statement. RepeatableRead maps to IsolationLevel.Snapshot, fixed at BEGIN.
Serializable parses but execution refuses it with CS04073 (0A000), subject to transaction-state
checks. Outside a block, SET instead succeeds with Warning CS04071 (25000). The enum itself
performs no validation and throws no exceptions. It is distinct from the kernel IsolationLevel enum;
SQL text does not accept the word SNAPSHOT.

## Example

Checked against SqlIsolationParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
var control = (SqlTransactionExpression)statement.SqlExpression;
Console.WriteLine(control.IsolationLevels[0] == SqlTransactionIsolationLevel.ReadUncommitted); // True
```

## Related reference

[SqlTransactionExpression](sql-transaction-expression.md) · [BEGIN](../../../../database/sql/language/statements/begin-transaction.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlTransactionIsolationLevel.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIsolationParserTests.cs`.
