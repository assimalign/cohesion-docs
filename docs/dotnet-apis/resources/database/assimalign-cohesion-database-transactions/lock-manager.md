# LockManager

Arbitrates writer locks and releases new grants when an unwritten statement retries arbitration.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public sealed class LockManager
{
    public static LockManager Create();
    public ValueTask AcquireAsync(TransactionSequence owner, LockResource resource,
        LockMode mode, CancellationToken cancellationToken = default);
    public IDisposable EnterLockTimeout(TimeSpan timeout);
    public bool TryAcquire(TransactionSequence owner, LockResource resource, LockMode mode);
    public LockStatementMark MarkStatement(TransactionSequence owner);
    public void ReleaseSince(LockStatementMark mark);
    public void ReleaseAll(TransactionSequence owner);
}
```

## Behavior and exceptions

Create returns a standalone lock manager; the coordinator supplies the engine's manager. Locks
belong to a TransactionSequence. AcquireAsync waits for a compatible grant in the resource's FIFO
queue. TryAcquire uses the same grant rules without waiting, returning false when the request
would queue. Existing owner grants can be re-granted or upgraded; every granted mode remains held.

AcquireAsync can throw TransactionDeadlockException when a wait closes a cycle,
OperationCanceledException on cancellation, or TransactionAbortedException when the owner ends
while waiting or an engine-mode wait is abandoned because storage goes offline. An engine-mode
request that would wait inside the statement apply gate throws InvalidOperationException.
ReleaseAll ends a standalone owner's holdings and fails its pending requests. In engine mode it
leaves active transactions' release to the transaction manager at commit/rollback.

### Statement marks

MarkStatement(owner) returns an allocation-free [LockStatementMark](lock-statement-mark.md)
recording the manager, owner and current grant boundary. It adds no validation exception. The
caller must serialize that owner's statements and lock requests so earlier grants are complete
and no request races the mark.

ReleaseSince(mark) releases only resources first granted to that owner after the mark and wakes
their queues. It does not end the owner. Resources held at the mark retain every mode, including
upgrades acquired later; it never downgrades an earlier resource. Re-granting a new resource does
not make it survive release. An abandoned writer keeps all grants until deferred undo completes.
The method throws ArgumentException for a default mark or one created by another manager.

Use a mark within the same transaction lifetime, with no surviving writes since the mark and no
pending lock requests when releasing. A failed physical statement bracket must be fully restored
before release. ReleaseSince performs no undo and does not replace transaction rollback.

SQL [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md#isolation-locking-and-retries)
uses a retry mark to release new keys/rows before waiting for a contended row. After AcquireAsync
grants the row, it takes a new retry mark and retains that row across later restarts. Rows kept
by successful waits stay locked until transaction end. A separate original statement mark lets
preparation failures release all new grants, including retained waited rows, while earlier grants
and upgrades survive. Once apply begins, ordinary transaction-duration locking applies.
Opposite multi-row key/row orders can still deadlock; the victim retries its entire transaction.

### Per-wait timeouts

EnterLockTimeout sets a flow-local budget until its IDisposable scope is disposed on the entering
flow, in reverse nesting order. Zero disables it; nested scopes restore the prior value and
concurrent flows remain independent. Negative values or values above int.MaxValue milliseconds
throw ArgumentOutOfRangeException. Direct and nested unique-index requests observe the scope in
standalone and engine modes. Uncontended grants allocate no timer; each queued wait gets a fresh
budget. TryAcquire remains nonwaiting.

Expiration removes the pending request and throws
[TransactionLockTimeoutException](transaction-lock-timeout-exception.md), leaving existing grants
and the owner active. The caller owns rollback/release. Queue removal fixes the exception and
LockWaitStop outcome (Timeout); caller cancellation uses Cancelled, while grant, owner end, deadlock
and storage abandonment retain their own outcomes. A late cancellation cannot replace a dequeued
grant awaiting notification. No lock wait is allowed inside the coordinator apply gate.

Adapted from LockTimeoutTests:

```csharp
var timedLocks = LockManager.Create();
var holder = new TransactionSequence(1);
var waiter = new TransactionSequence(2);
var row = LockResource.Entry(7, 7);
timedLocks.TryAcquire(holder, row, LockMode.Exclusive);
using (timedLocks.EnterLockTimeout(TimeSpan.FromMilliseconds(30))) {
    try { await timedLocks.AcquireAsync(waiter, row, LockMode.Exclusive); }
    catch (TransactionLockTimeoutException) { Console.WriteLine("Wait timed out"); }
}
timedLocks.ReleaseAll(holder);
Console.WriteLine(timedLocks.TryAcquire(waiter, row, LockMode.Exclusive)); // True
timedLocks.ReleaseAll(waiter);
```


## Example

Adapted from LockStatementMarkTests.ReleaseSince_PreviousGrants_ShouldKeepEveryMode; no data has
been written in this standalone example:

```csharp
using Assimalign.Cohesion.Database.Transactions;

var locks = LockManager.Create();
var owner = new TransactionSequence(1);
var other = new TransactionSequence(2);
var earlier = LockResource.Entry(1, 1);
var newer = LockResource.Entry(1, 2);
locks.TryAcquire(owner, earlier, LockMode.Shared);
var mark = locks.MarkStatement(owner);
locks.TryAcquire(owner, earlier, LockMode.Exclusive);
locks.TryAcquire(owner, newer, LockMode.Exclusive);
locks.TryAcquire(owner, newer, LockMode.Exclusive);
locks.ReleaseSince(mark);

Console.WriteLine(locks.TryAcquire(other, earlier, LockMode.Shared));    // False: upgrade retained
Console.WriteLine(locks.TryAcquire(other, newer, LockMode.Exclusive)); // True: new grant released
locks.ReleaseAll(other);
locks.ReleaseAll(owner);
```

## Related reference

[Assembly](index.md) · [Design](design.md#unwritten-statement-marks) · [LockStatementMark](lock-statement-mark.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/LockTimeoutTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/LockManager.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/LockStatementMarkTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertArbitrationConformanceTests.cs`.
