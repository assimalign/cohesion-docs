# BTreeIndexManager and BTreeIndexManagerOptions

Namespace: `Assimalign.Cohesion.Database.Indexing`.

## Pruning API

`BTreeIndexManager.Create(BTreeIndexManagerOptions options)` creates the sealed manager. Options require Storage and TransactionSource; LockManager and ExistingIndexes remain optional. The added option and method are:

```csharp
// BTreeIndexManagerOptions
public Func<TransactionSequence>? PruneBound { get; init; }

// BTreeIndexManager
public ValueTask<long> PruneAsync(TransactionSequence pruneBound,
    CancellationToken cancellationToken = default);
```

The provider supplies a safe exclusive horizon for unique-insert pruning. Without a provider, inserts retain history. Explicit PruneAsync removes tombstoned entries strictly below the supplied proven bound and returns the removal count. Callers serialize it with catalog changes, statement apply and physical rollback; model engines arrange this through TransactionCoordinator. One call visits at most 64 leaves across registered trees. Cancellation/failure rolls back the current leaf; prior committed leaves stay reclaimed and continuation resumes on a later pass. Tree pages are retained.

See [pruning design](design.md#safe-index-history-pruning) and [coordinator](../assimalign-cohesion-database-transactions/transaction-coordinator.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndexManager.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndexManagerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/tests/BTreePruneTests.cs`.
