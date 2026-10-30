# DatabaseTransaction

The shared explicit-transaction lifecycle and kernel binding used by model implementations.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

The savepoint-related protected surface is:

```csharp
public abstract class DatabaseTransaction : IAsyncDisposable
{
    protected DatabaseTransaction(TransactionId id, IsolationLevel isolationLevel);
    protected DatabaseTransaction(TransactionCoordinator coordinator, TransactionContext context);
    protected TransactionCoordinator KernelCoordinator { get; }
    protected TransactionContext KernelContext { get; }
    protected internal bool HasSavepoints { get; }
    protected void UpdateSavepointContext(TransactionContext context);
    protected bool TryConfigureIsolationLevel(IsolationLevel isolationLevel, Func<bool> configure);
}
```

Kernel vocabulary belongs to `Assimalign.Cohesion.Database.Transactions`. The public lifecycle
exposes Id, IsolationLevel, State, CommitAsync, RollbackAsync and DisposeAsync; named controls
are supplied by [DatabaseSession's protected controls](design.md#named-savepoint-controls).

## Behavior and exceptions

The kernel-bound constructor retains the coordinator and context and throws ArgumentNullException
for either null argument. The identity-only constructor supplies no savepoint binding;
KernelCoordinator/KernelContext then throw InvalidOperationException. HasSavepoints reports a
live named scope. It prevents isolation configuration while active; releasing all names removes
that restriction. Returning a pooled connection discards names through rollback.

TryConfigureIsolationLevel invokes its callback only while configuration is admitted; it returns
false for unusable/busy transactions or active named savepoints. UpdateSavepointContext must run
inside that callback on its owning thread. A null configure callback throws ArgumentNullException.
UpdateSavepointContext throws ArgumentNullException for null,
InvalidOperationException outside admitted configuration, and ArgumentException for an inactive
context or different transaction identity/sequence. It replaces the handle used for later undo.

Incomplete undo exposes Faulted state while retaining a live transaction and fences work/commit
with CK04009. Only rollback to the same or an outer retained savepoint, or whole rollback, can
recover it. Ordinary Graph/Documents failures still require whole rollback; ordinary SQL
statement failures leave the transaction usable. These model policies are applied by sessions.

## Example

This derived implementation follows SavepointSeamTests; supply an owned coordinator and context:

```csharp
sealed class BoundTransaction : DatabaseTransaction
{
    public BoundTransaction(TransactionCoordinator coordinator, TransactionContext context)
        : base(coordinator, context) { }

    public bool ConfigureReadCommitted() => TryConfigureIsolationLevel(IsolationLevel.ReadCommitted, () =>
    {
        UpdateSavepointContext(KernelCoordinator.ReplaceIsolationLevel(
            KernelContext, IsolationLevel.ReadCommitted));
        return true;
    });

    protected override TransactionState GetKernelState() => KernelContext.State;
    protected override ValueTask CommitCoreAsync() => KernelCoordinator.CommitAsync(KernelContext);
    protected override ValueTask RollbackCoreAsync() => KernelCoordinator.RollbackAsync(KernelContext);
    protected override DatabaseException? GetOfflineRefusal() => null;
    protected override DatabaseException CreateAbortedException(Exception? cause, bool commit)
        => new("Aborted", cause);
}
```

On an active idle transaction without savepoints, ConfigureReadCommitted returns true and retains
its identity. With an active named scope it returns false without replacing the context. Model
implementations also supply their own offline/error translation and data-admission policy.

## Related reference

[Assembly](index.md) · [Design](design.md#named-savepoint-controls) · [TransactionCoordinator](../assimalign-cohesion-database-transactions/transaction-coordinator.md#savepoint-controls)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseTransaction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/SavepointSeamTests.cs`.
