# TransactionLockTimeoutException

Reports expiration of one contended lock wait.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public sealed class TransactionLockTimeoutException : TimeoutException
{
}
```

## Behavior and exceptions

There is no public constructor. LockManager.AcquireAsync throws it when the current
EnterLockTimeout budget removes a queued request. Existing grants and the transaction remain
active; the caller owns rollback and release. The exception retains its cancellation cause.
SQL maps it to SqlTimeoutException, CS04044 / SQLSTATE 55P03. This kernel exception has no
Diagnostics or SqlState member and is separate from TransactionAbortedException.

## Example

With a holder blocking row, as in the [LockManager example](lock-manager.md#per-wait-timeouts):

```csharp
using (locks.EnterLockTimeout(TimeSpan.FromMilliseconds(30))) {
    try { await locks.AcquireAsync(waiter, row, LockMode.Exclusive); }
    catch (TransactionLockTimeoutException failure) {
        Console.WriteLine(failure.Message); // A lock wait exceeded the configured lock timeout.
    }
}
```

## Related reference

[Assembly](index.md) · [LockManager](lock-manager.md) · [SQL timeout](../assimalign-cohesion-database-sql/sql-timeout-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/Exceptions/TransactionLockTimeoutException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/LockTimeoutTests.cs`.
