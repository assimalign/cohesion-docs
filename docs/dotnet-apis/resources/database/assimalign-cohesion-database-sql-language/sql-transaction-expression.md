# SqlTransactionExpression

Parsed transaction, named savepoint, isolation or DISCARD ALL control.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlTransactionExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlTransactionIsolationLevel> IsolationLevels { get; }
    public string? SavepointName { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Inherited CommandType identifies Begin, SetTransaction,
Commit, Rollback, Discard, Savepoint, ReleaseSavepoint or RollbackToSavepoint. SavepointName is
null for other controls; it holds the normalized identifier for named controls. Unquoted names
fold to lowercase; quoted names preserve decoded case. Canonical rendering quotes the name.
IsolationLevels records modes in source order and is empty when no
mode is written. Repeated supported modes use the last level; any Serializable mode is refused
by execution in a state-valid block. Rendering uses canonical transaction spellings and mode lists.
Accessors add no validation exceptions.

Malformed modes carry CS00012 (42601). SqlQueryRequest.FromSql rejects parser errors with
DatabaseParseException. Embedded isolation-control refusals return an Error result with diagnostics;
SET outside a block returns Success with Warning CS04071 (25000). Late configuration uses CS04072
(25001); unsupported SERIALIZABLE uses CS04073 (0A000). See
[SET TRANSACTION](../../../../database/sql/language/statements/set-transaction.md) for timing and
state precedence. Active savepoints refuse SET with CS04074 (25001) before the other isolation
checks. Named controls throw DatabaseException with CK04012 outside a block, CK04013 for unknown names and CK04009 for
incomplete undo (no SQLSTATE). See [savepoint semantics](../../../../database/sql/language/statements/savepoint.md)
and [SqlDatabaseSession](../assimalign-cohesion-database-sql/sql-database-session.md)
for an execution example.

## Example

Adapted from SqlIsolationParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "BEGIN WORK ISOLATION LEVEL READ COMMITTED, ISOLATION LEVEL REPEATABLE READ;");
var control = (SqlTransactionExpression)statement.SqlExpression;
Console.WriteLine(control.CommandType);        // Begin
Console.WriteLine(control.IsolationLevels[0]); // ReadCommitted
Console.WriteLine(control.IsolationLevels[1]); // RepeatableRead
```

Execution starts Snapshot isolation. No data expression is evaluated by parsing.

```csharp
var named = (SqlTransactionExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SAVEPOINT Keep;")).SqlExpression;
Console.WriteLine(named.CommandType);   // Savepoint
Console.WriteLine(named.SavepointName); // keep
```

Checked against SavepointConformanceTests. Parsing creates no transaction or undo scope.

## Related reference

[SqlTransactionIsolationLevel](sql-transaction-isolation-level.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlTransactionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIsolationParserTests.cs`.
