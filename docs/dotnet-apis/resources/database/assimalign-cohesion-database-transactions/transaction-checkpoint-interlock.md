# TransactionCheckpointInterlock

Engine plumbing that serializes physical write scopes and sharp checkpoints for one storage.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public sealed class TransactionCheckpointInterlock
{
    public static TransactionCheckpointInterlock ForStorage(
        Assimalign.Cohesion.Database.Storage.Storage storage);
    public Lease Enter(string bracketClass, long owner = 0,
        CancellationToken cancellationToken = default);
    public bool TryCheckpoint(TimeSpan gateTimeout,
        CancellationToken cancellationToken = default);

    public readonly struct Lease : IDisposable
    {
        public void Dispose();
    }
}
```

## Behavior and exceptions

ForStorage returns the single interlock for that storage instance, shared with its transaction
coordinator. A null storage throws ArgumentNullException. Catalog file sets use independent gates
with plain storage checkpoints; data storage uses the coordinator's logical-writer-anchor checkpoint.

Enter acquires a synchronous write scope. bracketClass names the operation in checkpoint-deferral
events; owner is an optional logical writer or physical object identity. Null, empty or whitespace
classes throw ArgumentException. Cancellation before admission throws OperationCanceledException
and owns no scope. Nested scopes on the entering thread share its gate.

Acquire model grants before the data gate; no operation inside it may wait for model locks.
Catalog publication takes its metadata monitor, independent catalog interlock, then storage/page
latches. Release physical brackets and page latches before disposing the outermost lease. That
release checks size-due work and services due or deferred checkpoints before the next admission.
Catalog readers can wait through this flush and fsync while the metadata monitor remains held.

TryCheckpoint waits at most gateTimeout for admission; zero attempts immediately. True means this
call completed the checkpoint. False transfers the request to the holder for later service. Invalid
timeouts throw ArgumentOutOfRangeException and canceled waits throw OperationCanceledException.
Calling from the current thread's own synchronous/coordinated scope, or encountering an active raw
bracket at checkpoint, throws StorageTransactionException. Offline storage or a durable checkpoint
failure can throw StorageOfflineException. Other checkpoint failures retain their original exception;
a deferred operational failure is rethrown by the next call, preserving its identity.

## Lease

Lease is a thread-affine readonly value type. Copies share one disposal identity. Dispose on the
entering thread, after every bracket and page latch has ended; repeated disposal there is inert.
The outermost disposal releases admission and can service a checkpoint. A refused cross-thread
disposal emits CheckpointLeaseThreadMismatch (Error 22), asserts in Debug and throws
StorageTransactionException without releasing admission.

The thread check precedes the disposal-identity lookup whenever the entering thread holds
synchronous admission. An already disposed copy therefore also refuses cross-thread disposal
while that thread holds another scope. Default/inert leases and disposed copies when that
thread holds no scope do nothing.
Use synchronous scopes without an await that could move disposal to another thread.

CheckpointBlocked (Verbose 21, Checkpoints keyword) reports the deferred holder class and owner.
Storage CheckpointBusy (Verbose 19) independently reports physical brackets refusing truncation.
See [events](design.md#transaction-eventsource) and [checkpoint triggering](../assimalign-cohesion-database-storage/design.md#checkpoint-triggering).

## Example

For an initialized model storage, keep the scope outside the physical bracket:

```csharp
var interlock = TransactionCheckpointInterlock.ForStorage(storage);
using (var scope = interlock.Enter("Catalog", 42, cancellationToken))
{
    using var bracket = storage.BeginTransaction("Catalog", 42);
    // Perform the engine's synchronous catalog writes.
    bracket.Commit();
}
```

The scope uses the same admission as coordinator checkpoints; the physical bracket's class and
owner independently identify storage-level blockers.

## Related reference

[Assembly](index.md) · [Design](design.md) · [TransactionCoordinator](transaction-coordinator.md) · [Storage](../assimalign-cohesion-database-storage/storage.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionCheckpointInterlock.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCoordinatorRecoveryTests.cs`.
