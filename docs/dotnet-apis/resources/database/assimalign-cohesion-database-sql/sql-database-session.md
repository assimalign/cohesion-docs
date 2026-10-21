# SqlDatabaseSession

Executes typed or text SQL in one database, with nullable execution-budget overrides.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

The timeout members are:

```csharp
public sealed class SqlDatabaseSession : DatabaseLanguageSession<SqlQueryRequest>
{
    public TimeSpan? StatementTimeout { get; set; }
    public TimeSpan? LockTimeout { get; set; }
}
```

## Behavior and exceptions

SqlDatabase.CreateSessionAsync creates the session. Null overrides inherit the
[engine snapshot](sql-database-engine-options.md); zero disables and a positive duration applies
to subsequent statements. Each setter throws ArgumentOutOfRangeException below zero or above
int.MaxValue milliseconds. Configuration and execution require single-threaded session use.

Both text ExecuteAsync and typed SqlQueryRequest execution observe the same timers after parsing.
COHSQLE043 / 57014 and COHSQLE044 / 55P03 throw [SqlTimeoutException](sql-timeout-exception.md).
The failed statement rolls back, releasing its new resources after undo. Earlier transaction work,
grants and upgrades remain; offline/abandoned writers retain locks for recovery. Caller cancellation
remains OperationCanceledException with its prior lock behavior. Transaction-control requests arm
neither timer, and started commits and committed DDL cleanup finish without cancellation.

If an object-lock wait invalidates the target definition, DatabaseTransactionAbortedException carries
COHSQLT006 (no SQLSTATE); retry in a fresh transaction. Adjacent FK changes refresh before row reads.
See [execution and transactions](design.md#sql-transaction-control-and-the-b7-seam).

## Example

This is the unique-key wait case from SqlTimeoutConformanceTests, without test helpers:

```csharp
await using var engine = SqlDatabaseEngine.Create("timeouts", new SqlDatabaseEngineOptions());
var database = await engine.CreateDatabaseAsync("db");
await using var holder = await database.CreateSessionAsync();
await using var waiter = await database.CreateSessionAsync();
await holder.ExecuteAsync("CREATE TABLE items (id INT PRIMARY KEY)");
await using var held = await holder.BeginTransactionAsync();
await holder.ExecuteAsync("INSERT INTO items VALUES (1)");
await using var transaction = await waiter.BeginTransactionAsync();
waiter.LockTimeout = TimeSpan.FromMilliseconds(80);
try { await waiter.ExecuteAsync("INSERT INTO items VALUES (1)"); }
catch (SqlTimeoutException failure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE044
    Console.WriteLine(failure.SqlState);            // 55P03
}
await held.RollbackAsync();
waiter.LockTimeout = TimeSpan.Zero;
var inserted = await waiter.ExecuteAsync("INSERT INTO items VALUES (2)");
Console.WriteLine(inserted.AffectedCount); // 1: explicit transaction still usable
await transaction.CommitAsync();
```

## Related reference

[Assembly](index.md) · [Engine options](sql-database-engine-options.md) · [SQL limits](../../../../database/sql/language/diagnostics.md#statement-and-lock-timeouts)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferenceLockTests.cs`.
