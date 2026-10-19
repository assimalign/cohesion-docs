# IGraphStore migration

The integration branch exposes the sealed [GraphStore](graph-store.md), with concrete
TransactionContext parameters. IGraphStore and DefaultGraphStore are absent.

## Updated usage

```csharp
GraphStore store = GraphStore.Open(storage, coordinator);
TransactionContext context = await coordinator.BeginAsync(IsolationLevel.Snapshot);
ulong id = (ulong)storage.ReserveTransactionSequence();
var node = await store.CreateNodeAsync(new StoredGraphNode(id, ["P"],
    new Dictionary<string, object?>()), context);
await coordinator.CommitAsync(context);
```

Creation takes a complete record with a reserved ID. See [GraphStore](graph-store.md) for
changed create/update signatures, exceptions and MVCC behavior, and the
[complete example](examples/graph-store-tests.md) for storage lifetime.

## Related reference

[Assembly](index.md) · [GraphStore](graph-store.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/GraphStore.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/tests/GraphStoreTests.cs`.
