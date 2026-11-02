# TransactionSavepoint

An opaque, memory-only undo scope owned by one coordinator and transaction context.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public sealed class TransactionSavepoint { }
```

## Behavior and exceptions

There is no public constructor or member. Obtain a token from
[TransactionCoordinator.CreateSavepointAsync](transaction-coordinator.md#savepoint-controls).
It belongs to the exact coordinator and current lifecycle context used to create it. Callers
serialize operations for the same transaction, including reads and lock requests.

Release removes the token and descendants while retaining their effects. Rollback-to undoes
effects after it, discards descendants and retains the target for reuse. Root end invalidates
all tokens. Tokens have no SQL name and are not durable. Passing a foreign, stale, released,
discarded or ended token raises a kernel refusal classified as CK04008 by GetUndoFailureCode;
the token itself adds no exceptions.

## Example

Given an initialized coordinator, adapted from TransactionSavepointTokenTests:

```csharp
var context = await coordinator.BeginAsync(IsolationLevel.Snapshot);
TransactionSavepoint outer = await coordinator.CreateSavepointAsync(context);
var inner = await coordinator.CreateSavepointAsync(context);
await coordinator.RollbackToSavepointAsync(context, outer); // Discards inner, retains outer.
await coordinator.RollbackToSavepointAsync(context, outer); // Target can be reused.
await coordinator.ReleaseSavepointAsync(context, outer);
await coordinator.RollbackAsync(context);
```

No write is committed. A later use of `inner` or `outer` is refused with CK04008.

## Related reference

[Assembly](index.md) · [TransactionCoordinator](transaction-coordinator.md) · [Design](design.md#in-memory-undo-markers-and-model-adapters)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/TransactionSavepoint.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionSavepointTokenTests.cs`.
