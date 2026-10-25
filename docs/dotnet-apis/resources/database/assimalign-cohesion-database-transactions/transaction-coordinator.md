# TransactionCoordinator

Composes transaction state, snapshots, locks, version undo and journal durability for one database.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Lifecycle signatures

```csharp
public sealed class TransactionCoordinator : IAsyncDisposable
{
    public TransactionCoordinator(Storage storage, StorageJournal journal,
        TransactionRecordSpace records);
    public TransactionCoordinator(Storage storage, StorageJournal journal,
        TransactionRecordSpace records,
        Func<TransactionSequence, CancellationToken, ValueTask<long>> indexPruner);
    public TransactionSequence PruneBound { get; }
    public long RunVersionPurgePass(CancellationToken cancellationToken);
    public ValueTask<TransactionContext> BeginAsync(IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default);
    public ValueTask<TransactionContext> BeginSnapshotPinAsync(
        CancellationToken cancellationToken = default);
    public ValueTask CommitAsync(TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask RollbackAsync(TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask<T> ApplyStatementAsync<T>(TransactionContext context,
        Func<StorageTransaction, T> apply, CancellationToken cancellationToken = default);
    public ValueTask DisposeAsync();
}
```

Supply the journal and record adapter for the same storage. The caller owns them and disposes
the coordinator first. Model engines perform recovery before admitting sessions; see [design](design.md).

## Begin and first write

BeginAsync reserves a storage-counter sequence, captures the normal isolation snapshot and enters
the manager's active table. It opens no physical bracket and appends no journal record.
The first admitted ApplyStatementAsync enrolls the transaction as a writer: one logical BEGIN
precedes its physical bracket and any version stamps. Later applies append no second logical BEGIN.
An admitted apply remains a writer even if its callback changes nothing or its bracket rolls back.
Cancellation before admission leaves a reader unenrolled.

BeginSnapshotPinAsync registers a fixed Snapshot context to hold one statement's pruning horizon.
Begin it before capturing the statement snapshot, keep it alive through enumeration, then release
it with RollbackAsync. It retains ordinary registration, sequence and cleanup behavior but moves
only the current-transactions gauge. It contributes no transaction/commit/rollback rate or Verbose
begin/commit/rollback event, including on disposal. See [counters](design.md#transaction-eventsource).
This read-only pin path is used by all five models' ReadCommitted statements and SQL's reclamation
fence. Schema-provisioner lock transactions use the ordinary counted path.

A reader spanning a checkpoint is neither journaled nor anchored. Its first later apply enrolls
under the checkpoint interlock before any stamp, so crash recovery can still classify its writes.
Snapshots and visibility retain their existing semantics. Sequence reservation and reader ends
bypass the checkpoint's transaction/log gates, so readers can begin, pin cached pages and
commit or roll back during checkpoint flush I/O. Writers still wait through the checkpoint's
statement-apply gate. See [checkpoint behavior and measurements](../assimalign-cohesion-database-storage/design.md#checkpoints).

## Commit and rollback

Both methods observe caller cancellation only before the manager claims the transaction's end.
A token canceled by then throws `OperationCanceledException` and leaves the transaction active
and usable, with its writes, locks and visibility untouched and nothing committed. Once claimed,
the end ignores caller cancellation while draining admitted applies and appending/flushing the log.
Commit returns its real outcome, including normal append or durability failures. This applies to
readers as well as writers; a reader still appends and flushes nothing.

CommitAsync ends a reader and releases its locks without a lifecycle append or durability wait.
For an enrolled writer it appends COMMIT and waits for durability under the configured policy while
the transaction remains active. Journal ordering covers earlier statement records. A successful
commit then removes the context and releases its locks; committed tombstones remain for pruning.

RollbackAsync undoes writer stamps and appends an abort record only for an enrolled writer,
without waiting for that record's durability. A reader simply ends and releases its locks.
Cancellation is observed before rollback starts and leaves the transaction active; once started,
rollback runs to completion. A failed undo defers cleanup and retains the writer's locks and
in-flight visibility until undo succeeds. Disposal aborts outstanding readers without journal work.

## Exceptions

| Exception | Trigger |
| --- | --- |
| ArgumentNullException | Constructor dependency or lifecycle context is null |
| ObjectDisposedException | Lifecycle call after the underlying manager is disposed |
| OperationCanceledException | Begin/apply canceled, or commit/rollback canceled before its end starts |
| TransactionAbortedException | Context is foreign, a statement view, ended or already ending; commit refused or its record could not be appended |
| TransactionCommitUnconfirmedException | Writer COMMIT was appended but its drain/durable flush failed; committed in memory, with persistence decided by recovery |

A reader appends no COMMIT and cannot have an unconfirmed journal commit. Storage allocation and
engine offline checks still apply. A failed journal write or fsync takes storage offline.

## Example

With a new coordinator over an open model storage, journal and record adapter:

```csharp
await using var coordinator = new TransactionCoordinator(storage, journal, records);
long before = journal.LastLsn;
var reader = await coordinator.BeginAsync(IsolationLevel.Snapshot);
await coordinator.CommitAsync(reader);
Console.WriteLine(journal.LastLsn == before); // True

var anotherReader = await coordinator.BeginAsync(IsolationLevel.ReadCommitted);
await coordinator.RollbackAsync(anotherReader);
Console.WriteLine(journal.LastLsn == before); // True

var writer = await coordinator.BeginAsync(IsolationLevel.Snapshot);
await coordinator.ApplyStatementAsync(writer, _ => 0);
await coordinator.CommitAsync(writer);
Console.WriteLine(journal.LastLsn > before); // True, even though the callback changed no rows
```

This follows the admission/no-op cases in TransactionCoordinatorRecoveryTests. Each model's
read-only transaction tests also verify unchanged journal length, LSN and durable-flush attempts
for autocommit reads, explicit commit, rollback, disposal and ReadCommitted snapshot pins.

The pin lifecycle follows TransactionEventSourceTests, with the same open coordinator:

```csharp
var pin = await coordinator.BeginSnapshotPinAsync(cancellationToken);
try
{
    TransactionSnapshot snapshot = pin.Snapshot;
    // Complete reads using snapshot while the pin is registered.
}
finally
{
    await coordinator.RollbackAsync(pin, CancellationToken.None);
}
```

The gauge rises by one while the pin is tracked and returns when it ends; all three transaction
rates stay unchanged. A canceled begin throws OperationCanceledException without tracking a pin;
a disposed manager throws ObjectDisposedException. A canceled rollback leaves the pin active,
so cleanup uses a non-canceled token.

## Related reference

The cancellation test's pre-canceled end can be reproduced with the open coordinator:

```csharp
var context = await coordinator.BeginAsync(IsolationLevel.Snapshot);
using var canceled = new CancellationTokenSource();
canceled.Cancel();
try { await coordinator.CommitAsync(context, canceled.Token); }
catch (OperationCanceledException)
{
    Console.WriteLine(context.State); // Active
}
await coordinator.CommitAsync(context, CancellationToken.None);
Console.WriteLine(context.State); // Committed
```

SQL/Graph pooled client disposal resets an open or failed transaction by rollback before reuse.
Only a certified Idle connector returns to the pool; failed or unsupported reset discards it. See [client lease lifetime](../assimalign-cohesion-database-client/database-connection.md).

[Assembly](index.md) · [Design](design.md) · [LockManager](lock-manager.md)

## Index maintenance

`RunVersionPurgePass` called inside this coordinator's `ApplyStatementAsync` callback immediately
throws `StorageTransactionException`, before undo or pruning. `Checkpoint` and `TryCheckpoint`
have the same refusal because the shared apply semaphore is not reentrant.

The optional indexPruner callback runs under the coordinator apply gate after record pruning in RunVersionPurgePass. It receives the same conservative exclusive PruneBound and returns the removed-entry count, which contributes to the pass total. An independent record-prune failure does not suppress index maintenance; the retained failure is rethrown afterward. Cancellation, disposal and out-of-memory failures are not treated as ordinary deferred maintenance failures.

PruneBound includes active transaction snapshot minima and statement pins. Do not substitute the oldest active sequence. See [safe index-history pruning](../assimalign-cohesion-database-indexing/design.md#safe-index-history-pruning).

## Sources

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCommitCancellationTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionCoordinator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionManager.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCoordinatorRecoveryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/JournalEnrollmentTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionEventSourceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReadOnlyTransactionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/docs/DESIGN.md`.
