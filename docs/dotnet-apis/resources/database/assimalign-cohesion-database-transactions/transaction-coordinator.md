# TransactionCoordinator

Composes transaction state, snapshots, locks, version undo and journal durability for one database.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Lifecycle signatures

```csharp
public sealed class TransactionCoordinator : IAsyncDisposable
{
    public TransactionCoordinator(Storage storage, StorageJournal journal,
        TransactionRecordSpace records);
    public ValueTask<TransactionContext> BeginAsync(IsolationLevel isolationLevel,
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

A reader spanning a checkpoint is neither journaled nor anchored. Its first later apply enrolls
under the checkpoint interlock before any stamp, so crash recovery can still classify its writes.
Snapshots and visibility retain their existing semantics.

## Commit and rollback

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
| OperationCanceledException | Begin canceled, apply canceled, or rollback canceled before it starts |
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

## Related reference

Pooled client disposal does not dispose the server session or reset an explicit transaction.
End or roll back that transaction before returning the connection; otherwise the next renter can
inherit it (known gap #1540). See [client lease lifetime](../assimalign-cohesion-database-client/database-connection.md).

[Assembly](index.md) · [Design](design.md) · [LockManager](lock-manager.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionCoordinator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionManager.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCoordinatorRecoveryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/JournalEnrollmentTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReadOnlyTransactionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/docs/DESIGN.md`.
