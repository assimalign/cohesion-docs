# SqlDatabaseEngineOptions

Configures SQL engine execution budgets and storage/worker settings.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

Execution-budget members include:

```csharp
public sealed class SqlDatabaseEngineOptions
{
    public TimeSpan StatementTimeout { get; set; }
    public TimeSpan LockTimeout { get; set; }
    public long StatementMemoryBudget { get; set; }
}
```

## Behavior and exceptions

Both default to TimeSpan.Zero, disabling the timer. StatementTimeout bounds cooperative execution
after parsing; LockTimeout bounds each contended lock wait separately, including unique-index waits.
SqlDatabaseEngine.Create snapshots options and throws ArgumentOutOfRangeException for either value
below zero or above TimeSpan.FromMilliseconds(int.MaxValue). Setters store the values; later changes
to the caller's options do not change the engine snapshot.

[Session overrides](sql-database-session.md) inherit when null and disable when zero. Started
commits and mandatory post-self-commit cleanup complete without these timers. SQL transaction
control arms neither timer; schema-provisioning sessions explicitly disable both. Other options
are described in [design](design.md#engine-owned-background-workers).

## Example

Adapted from SqlTimeoutConformanceTests; engine defaults are copied before the caller changes them:

```csharp
var options = new SqlDatabaseEngineOptions { LockTimeout = TimeSpan.FromMilliseconds(80) };
await using var engine = SqlDatabaseEngine.Create("lock-timeout", options);
options.LockTimeout = TimeSpan.Zero;
var database = await engine.CreateDatabaseAsync("db");
await using var session = await database.CreateSessionAsync();
Console.WriteLine(session.LockTimeout is null); // True: inherits the engine's 80 ms
session.LockTimeout = TimeSpan.Zero; // disable for this session
session.LockTimeout = null;          // inherit the engine's 80 ms again
```

## Related reference

[Assembly](index.md) · [Session](sql-database-session.md) · [Timeout exception](sql-timeout-exception.md)

## Hash-build memory

StatementMemoryBudget defaults to 67,108,864 bytes (64 MiB) and must be positive; engine creation rejects a nonpositive value with ArgumentOutOfRangeException. The engine snapshots the value. It limits accounted concurrently live hash-build memory across one statement's nested queries; it does not cap output materialization or planning. Exhaustion reports CS05067 / SQLSTATE 53200. Hash builds do not spill. See [query performance](../../../../database/sql/performance.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
