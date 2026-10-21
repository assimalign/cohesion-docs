# SqlDatabaseEngineOptions

Configures SQL engine execution budgets and storage/worker settings.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

The timeout members are:

```csharp
public sealed class SqlDatabaseEngineOptions
{
    public TimeSpan StatementTimeout { get; set; }
    public TimeSpan LockTimeout { get; set; }
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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
