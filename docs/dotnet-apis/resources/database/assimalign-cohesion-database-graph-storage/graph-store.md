# GraphStore

The sealed graph record and index facade over shared storage and transactions.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

## Changed members

```csharp
public sealed partial class GraphStore
{
    public static GraphStore Open(GraphStorage storage, TransactionCoordinator coordinator);
    public static void EnsureIndexFormat(GraphStorage storage);
    public ValueTask<StoredGraphNode> CreateNodeAsync(StoredGraphNode node,
        TransactionContext context, CancellationToken cancellationToken = default);
    public ValueTask<StoredGraphRelationship> CreateRelationshipAsync(
        StoredGraphRelationship relationship, TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask<StoredGraphNode?> UpdateNodeAsync(ulong id, IReadOnlyList<string> labels,
        IReadOnlyDictionary<string, object?> properties, TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask<StoredGraphRelationship?> UpdateRelationshipAsync(ulong id,
        IReadOnlyDictionary<string, object?> properties, TransactionContext context,
        CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

Open returns the concrete store, which owns neither storage nor coordinator. Mutations require an
active caller transaction and leave commit/rollback to it. Create now takes complete
[StoredGraphNode](stored-graph-node.md)/[StoredGraphRelationship](stored-graph-relationship.md)
records. Reserve their nonzero IDs from the backing storage's ReserveTransactionSequence before
creation; do not invent or recycle IDs. Creation copies values and refuses an existing identity.

UpdateNodeAsync replaces the complete labels/properties and UpdateRelationshipAsync replaces
properties while preserving identity, endpoints and type. Each returns the stored replacement or
null when absent. Every storage update publishes a new version, even for equal values; the GQL
engine filters pure no-op assignments before calling storage. Low-level maps retain explicit NULL;
GQL SET removes NULL keys before publication.

Format 2 persists revision-ordered MVCC chains. Old snapshots keep earlier records/index entries;
rollback removes replacements and restores stamps. Node updates maintain every property index;
relationship updates move both adjacency entries (one for a self-loop). Replacements validate
encoding/index keys before mutation. Snapshot updates are first-updater-wins. With the original
ReadCommitted transaction context, the store refreshes after acquiring the writer lock. A caller-pinned
statement view retains its horizon and may conflict; the caller chooses whether to refresh/retry.

| Exception | Trigger |
| --- | --- |
| ArgumentNullException | Required collections/context or Open storage/coordinator is null |
| ArgumentOutOfRangeException | Create identity is zero |
| ArgumentException | Duplicate create identity, blank name/key, unsupported scalar or non-finite float |
| InvalidOperationException | Inactive transaction; relationship endpoint absent in the current committed view |
| TransactionAbortedException | Changed/deleted entity since Snapshot or transaction ended while awaiting the writer lock |
| GraphElementTooLargeException | Entity exceeds 8,092 bytes or indexed scalar prefix exceeds 1,016 bytes |
| OperationCanceledException | Cancellation requested |
| StorageException | Storage, decoding or I/O failure |

EnsureIndexFormat checks graph record and B-tree formats before logical recovery scrub.
Format 1 records throw StorageCorruptionException with the recreation remedy; no upgrade reader
exists. Open also validates the record format.

## Example

With an open storage/store and active transaction context:

```csharp
ulong id = (ulong)storage.ReserveTransactionSequence();
var node = await store.CreateNodeAsync(new StoredGraphNode(id, ["P"],
    new Dictionary<string, object?> { ["x"] = 1 }), context);
var replacement = await store.UpdateNodeAsync(node.Id, ["P", "Admin"],
    new Dictionary<string, object?> { ["x"] = 2 }, context);
Console.WriteLine(replacement!.Value.Id == id); // True
Console.WriteLine(store.FindNode(id, context.Snapshot)!.Value.Properties["x"]); // 2
```

This follows GraphStoreUpdateTests. See the [complete example](examples/graph-store-tests.md)
for coordinator lifetime and historical reads.

## Related reference

[Assembly](index.md) · [Design](design.md) · [StoredGraphNode](stored-graph-node.md) · [StoredGraphRelationship](stored-graph-relationship.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/GraphStore.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/GraphStore.Updates.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/tests/GraphStoreUpdateTests.cs`.
