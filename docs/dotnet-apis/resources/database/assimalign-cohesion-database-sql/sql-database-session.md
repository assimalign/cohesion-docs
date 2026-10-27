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
CS05043 / 57014 and CS04044 / 55P03 throw [SqlTimeoutException](sql-timeout-exception.md).
The failed statement rolls back, releasing its new resources after undo. Earlier transaction work,
grants and upgrades remain; offline/abandoned writers retain locks for recovery. Caller cancellation
remains OperationCanceledException with its prior lock behavior. Transaction-control requests arm
neither timer, and started commits and committed DDL cleanup finish without cancellation.

If an object-lock wait invalidates the target definition, DatabaseTransactionAbortedException carries
CS04006 (no SQLSTATE); retry in a fresh transaction. Adjacent FK changes refresh before row reads.

[Table rewrite](sql-database.md) also retires the physical layout. Older read-only snapshots
retain it until they end, while DML needing it aborts with CS04006. A stale foreign-key parent
probe receives that retryable abort rather than a false FK violation. RewriteTableAsync is a
database maintenance call; session timeout overrides do not configure its lock acquisition.
See [execution and transactions](design.md#sql-transaction-control-and-the-b7-seam).

## Example

Execution, constraint and DDL failures expose stable codes through DatabaseException.Diagnostics.
The session retains precise current-statement locations or attaches its statement span; persisted
expression offsets are discarded first. Concrete SqlCatalogException, SqlConstraintViolationException
and DatabaseObjectLockedException metadata remain available. Ordinary statement failures preserve
earlier transaction work; storage/abort failures retain their existing transaction policy.
SQLSTATEs are documented mappings, with no new SQLSTATE field on Diagnostic.
See the [execution code reference](../../../../database/sql/language/diagnostics.md).

This execution-diagnostic case adapts SqlExecutionDiagnosticConformanceTests:

```csharp
using System;
using Assimalign.Cohesion.Database;
using Assimalign.Cohesion.Database.Sql;

await using var engine = SqlDatabaseEngine.Create("diagnostics", new());
var database = await engine.CreateDatabaseAsync("app");
await using var session = await database.CreateSessionAsync();
await session.ExecuteAsync("CREATE TABLE required_value (v INT NOT NULL)");
try { await session.ExecuteAsync("INSERT INTO required_value VALUES (NULL)"); }
catch (DatabaseException failure)
{
    var detail = failure.Diagnostics[0];
    Console.WriteLine(detail.Code); // CS03024; mapped SQLSTATE 23502
    Console.WriteLine(detail.Start); // 0
}
await session.ExecuteAsync("INSERT INTO required_value VALUES (1)"); // succeeds
```

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
    Console.WriteLine(failure.Diagnostics[0].Code); // CS04044
    Console.WriteLine(failure.SqlState);            // 55P03
}
await held.RollbackAsync();
waiter.LockTimeout = TimeSpan.Zero;
var inserted = await waiter.ExecuteAsync("INSERT INTO items VALUES (2)");
Console.WriteLine(inserted.AffectedCount); // 1: explicit transaction still usable
await transaction.CommitAsync();
```

## TRUNCATE and old snapshots

[TRUNCATE](../../../../database/sql/language/statements/truncate.md) returns Success with
AffectedCount 0 in automatic execution. Inside an explicit transaction, embedded ExecuteAsync
returns an Error result carrying CS04066 (SQLSTATE 25001); prior writes and the active transaction
remain usable. Remote execution raises SqlClientException with the same diagnostic and a reusable
connection. RESTRICT missing a referencing table uses CS03067 (0A000); a concurrent closure change
uses retryable CS04064 (40001). DML targeting retired tables or joining them as sources keeps
CS04006 before writes.

Adapted from `SqlTruncateTests.ExecuteAsync_PinnedSnapshot_ShouldRetainRowsViewsAndStorageUntilItEnds`:

```csharp
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Database.Execution;

await using var engine = SqlDatabaseEngine.Create("truncate", new());
var database = await engine.CreateDatabaseAsync("db");
await using var writer = await database.CreateSessionAsync();
await using var reader = await database.CreateSessionAsync();
await writer.ExecuteAsync("CREATE TABLE t (id INT PRIMARY KEY, n INT)");
await writer.ExecuteAsync("INSERT INTO t VALUES (1,10),(2,20)");
await writer.ExecuteAsync("CREATE VIEW v AS SELECT id, n FROM t");
await using var snapshot = await reader.BeginTransactionAsync();
await writer.ExecuteAsync("TRUNCATE t");
await writer.ExecuteAsync("INSERT INTO t VALUES (3,30)");
await using var old = (DatabaseResultSet)await reader.ExecuteAsync("SELECT n FROM t WHERE id=1");
await foreach (var row in old.GetRowsAsync(CancellationToken.None))
    Console.WriteLine(row.GetInt32(0)); // 10, through the old primary-key index
await snapshot.RollbackAsync();
await using var current = (DatabaseResultSet)await reader.ExecuteAsync("SELECT * FROM v");
await foreach (var row in current.GetRowsAsync(CancellationToken.None))
    Console.WriteLine($"{row.GetInt32(0)},{row.GetInt32(1)}"); // 3,30
```

Before rollback the reader also sees the original rows through views and joins. Reclamation waits
for all older transaction/statement pins; it frees retired row/index storage afterwards. ReadCommitted
pins each statement snapshot and refreshes on a later statement. CREATE TABLE AS SELECT remains refused.

## Related reference

[Assembly](index.md) · [Engine options](sql-database-engine-options.md) · [SQL limits](../../../../database/sql/language/diagnostics.md#statement-and-lock-timeouts)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferenceLockTests.cs`.
