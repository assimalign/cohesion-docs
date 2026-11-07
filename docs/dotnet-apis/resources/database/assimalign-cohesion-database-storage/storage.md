# Storage

The physical storage base used by model facades. The members below provide engine plumbing for
physical write attribution and checkpoint retry admission.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

Selected members:

```csharp
public abstract class Storage : IAsyncDisposable, IDisposable
{
    public bool HasActiveTransactions { get; }
    public IReadOnlyList<PageId> GetOwnerIndexPages(ulong ownerId);
    public StorageTransaction BeginTransaction();
    public StorageTransaction BeginTransaction(string bracketClass, long owner = 0);
    public StorageTransaction BeginTransaction(long sequence);
}
```

## BeginTransaction

The string overload begins a physical bracket and records its operation class and optional logical
writer or physical object identity. Examples of classes are Statement, Undo and SqlCatalog.
The parameterless overload uses Raw. The sequence overload retains its caller-adopted sequence
contract; these are physical storage brackets, separate from application session transactions.

An empty, null or whitespace bracketClass throws ArgumentException. An uninitialized/unopened
storage throws InvalidOperationException; offline storage throws StorageOfflineException. A
recovered read-only probe rejects writes with NotSupportedException. The bracket's existing
commit, rollback and durability rules are unchanged.

Attribution does not acquire the engine checkpoint interlock. Engine callers acquire the matching
[TransactionCheckpointInterlock](../assimalign-cohesion-database-transactions/transaction-checkpoint-interlock.md)
before beginning a synchronous bracket and end all brackets and page latches before releasing it.
A refused checkpoint emits Storage event 19, CheckpointBusy, for each physical blocker with its
class, owner, sequence and refusal identifier. See [checkpoint events](design.md#storage-eventsource).

## HasActiveTransactions

True means a storage-level transaction is active and checkpoint truncation is refused. This is an
advisory, allocation-free read for engine retry admission: another transaction may begin immediately
after it returns. It neither excludes writers nor proves that a later checkpoint will succeed.
The interlock reads it while holding admission after its own brackets have ended to detect
remaining outside blockers.

## GetOwnerIndexPages

Returns a point-in-time snapshot of the owner's index pages in ascending page order, including
unregistered trees; empty when it owns none. Later allocations/releases do not change the result.
Released pages leave the directory on transaction commit. Open rebuilds the directory from
recovered page headers, including pages of trees no longer registered. Joint retirement admission
uses this to count separate tree pages without charging stranded pages of a kept heap.

## Example

Adapted from TransactionCoordinatorRecoveryTests, with an initialized storage:

```csharp
var interlock = TransactionCheckpointInterlock.ForStorage(storage);
using (var scope = interlock.Enter("Catalog", 42))
{
    using var bracket = storage.BeginTransaction("Catalog", 42);
    Console.WriteLine(storage.HasActiveTransactions); // True
    bracket.Commit();
}
```

Scope disposal can service a size-due or deferred checkpoint after the bracket has ended. Keep
the synchronous scope on its entering thread.

## Related reference

[Assembly](index.md) · [Design](design.md) · [StorageTransactionException](storage-transaction-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Storage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCoordinatorRecoveryTests.cs`.
