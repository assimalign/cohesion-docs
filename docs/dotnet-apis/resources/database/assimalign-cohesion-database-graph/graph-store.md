# GraphStore

The sealed graph record and index facade over shared storage and transactions.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Changed members

```csharp
public sealed partial class GraphStore
{
    public static GraphStore Open(GraphStorage storage, TransactionCoordinator coordinator);
    public static void EnsureIndexFormat(GraphStorage storage);
    public ValueTask<StoredGraphNode> CreateNodeAsync(StoredGraphNode node,
        IReadOnlyDictionary<string, Guid> labelIds, TransactionContext context,
        CancellationToken cancellationToken = default);
    public IReadOnlyList<StoredGraphNode> GetNodes(string? label, TransactionSnapshot snapshot);
    public ValueTask<StoredGraphRelationship> CreateRelationshipAsync(
        StoredGraphRelationship relationship, Guid typeId, TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<StoredGraphRelationship>> GetIncidentAsync(ulong nodeId,
        TransactionSnapshot snapshot, CancellationToken cancellationToken = default);
    public IAsyncEnumerable<StoredGraphRelationship> GetIncidentAsync(ulong nodeId,
        GraphIncidentDirection direction, IReadOnlyCollection<Guid>? typeIds,
        TransactionSnapshot snapshot, CancellationToken cancellationToken = default);
    public ValueTask<StoredGraphNode?> UpdateNodeAsync(ulong id, IReadOnlyList<string> labels,
        IReadOnlyDictionary<string, object?> properties, IReadOnlyDictionary<string, Guid> labelIds,
        TransactionContext context, CancellationToken cancellationToken = default);
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
CreateRelationshipAsync also requires a nonempty catalog type GUID. The store persists it separately
from the type name and preserves it through property updates. It does not verify that typeId matches
relationship.Type; the engine supplies that pairing from Graph.Catalog.

CreateNodeAsync and UpdateNodeAsync require a label-name/catalog-GUID map. Each resulting label
must have a nonempty GUID; a missing or empty GUID throws ArgumentException and a null map throws
ArgumentNullException. The engine resolves these IDs from Graph.Catalog; the store trusts the
name/ID pairing and persists each ID beside its label.

UpdateNodeAsync replaces the complete labels/properties and UpdateRelationshipAsync replaces
properties while preserving identity, endpoints and type. Each returns the stored replacement or
null when absent. Every storage update publishes a new version, even for equal values; the GQL
engine filters pure no-op assignments before calling storage. Low-level maps retain explicit NULL;
GQL SET removes NULL keys before publication.

Format 4 persists revision-ordered MVCC chains. Old snapshots keep earlier records/index entries;
rollback removes replacements and restores stamps. Node updates maintain every label posting and property index;
relationship updates move both direction-specific adjacency entries, including for a self-loop. Replacements validate
encoding/index keys before mutation. Snapshot updates are first-updater-wins. With the original
ReadCommitted transaction context, the store refreshes after acquiring the writer lock. A caller-pinned
statement view retains its horizon and may conflict; the caller chooses whether to refresh/retry.

| Exception | Trigger |
| --- | --- |
| ArgumentNullException | Required collections/context, incident-read snapshot or Open storage/coordinator is null |
| ArgumentOutOfRangeException | Create identity is zero or incident direction is undefined |
| ArgumentException | Duplicate create identity, missing/empty label GUID, empty type GUID, blank name/key, unsupported scalar or non-finite float |
| InvalidOperationException | Inactive transaction; relationship endpoint absent in the current committed view |
| TransactionAbortedException | Changed/deleted entity since Snapshot or transaction ended while awaiting the writer lock |
| GraphElementTooLargeException | Entity exceeds 8,092 bytes or indexed scalar prefix exceeds 1,016 bytes |
| OperationCanceledException | Cancellation requested |
| StorageException | Storage, decoding or I/O failure |

EnsureIndexFormat checks graph record and B-tree formats before logical recovery scrub.
Records and tree registrations must be format 4. Older formats throw StorageCorruptionException
with CG06017 and the recreation remedy; no upgrade reader exists. Open also validates these
formats. The engine retains CG06017 alongside CK06003 and CG06018 in
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md);
no SQLSTATE is assigned.

## Label reads

GetNodes returns visible nodes in identity order. A null label enumerates all node identities;
a supplied label seeks its MVCC posting ranges and decodes only matching visible references.
An unknown label returns an empty list. Old snapshots retain their earlier membership after
relabeling or deletion; rollback and recovery restore postings with records. A null snapshot
throws ArgumentNullException. Malformed selected records/postings or a missing required tree
throw StorageCorruptionException.

## Incident reads

The streaming overload uses [GraphIncidentDirection](graph-incident-direction.md) relative to
nodeId and catalog type IDs to seek only selected adjacency ranges. A null typeIds selection reads
all types, an empty collection reads none, and duplicate IDs do not duplicate visits. Both returns
a self-loop once. Retain the snapshot throughout enumeration and dispose the enumerator on early
exit; await foreach handles disposal. Cross-range order is unspecified. The original unfiltered
list overload selects Both/all types and sorts by relationship identity.

Validation occurs during enumeration. A null snapshot throws ArgumentNullException; an undefined
direction throws ArgumentOutOfRangeException. Cancellation throws OperationCanceledException,
including for an empty selection. Malformed selected records/keys can throw StorageCorruptionException.
Enumeration takes no graph writer lock. The shared [BTreeCursor](../assimalign-cohesion-database-indexing/b-tree-cursor.md)
opens without reading pages and copies bounded batches with cancellation during latch acquisition
and page/entry reads. It holds no page or latch between moves. The list overload still materializes
its result. See [design](storage-design.md).

## Example

With an open storage/store and active transaction context:

```csharp
var labelIds = new Dictionary<string, Guid>
{
    ["P"] = Guid.Parse("10000000-0000-0000-0000-000000000001"),
    ["Admin"] = Guid.Parse("10000000-0000-0000-0000-000000000002")
};
ulong id = (ulong)storage.ReserveTransactionSequence();
var node = await store.CreateNodeAsync(new StoredGraphNode(id, ["P"],
    new Dictionary<string, object?> { ["x"] = 1 }), labelIds, context);
var replacement = await store.UpdateNodeAsync(node.Id, ["P", "Admin"],
    new Dictionary<string, object?> { ["x"] = 2 }, labelIds, context);
Console.WriteLine(store.GetNodes("Admin", context.Snapshot).Count); // 1
Console.WriteLine(replacement!.Value.Id == id); // True
Console.WriteLine(store.FindNode(id, context.Snapshot)!.Value.Properties["x"]); // 2
```

This follows GraphStoreUpdateTests. See the [complete example](examples/graph-store-tests.md)
for coordinator lifetime and historical reads.

## Related reference

[Assembly](index.md) · [Design](storage-design.md) · [StoredGraphNode](stored-graph-node.md) · [StoredGraphRelationship](stored-graph-relationship.md) · [GraphIncidentDirection](graph-incident-direction.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreLabelTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Storage/GraphStore.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Storage/GraphStore.Updates.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreUpdateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreAdjacencyTests.cs`.
