# Graph storage: create, update and snapshot reads

This example follows GraphStoreTests and GraphStoreUpdateTests on the integration branch.
Creation accepts complete records with caller-reserved IDs; updates retain those IDs and publish
new MVCC versions. Run this in a console project referencing Assimalign.Cohesion.Database.Graph and
Assimalign.Cohesion.Database.Transactions.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using Assimalign.Cohesion.Database.Graph.Storage;
using Assimalign.Cohesion.Database.Transactions;

using var storage = GraphStorage.Create(
    new MemoryStream(), new MemoryStream(), new MemoryStream(), "example");
await using var coordinator = new TransactionCoordinator(
    storage, storage.WriteAheadJournal, storage.Records);
GraphStore store = GraphStore.Open(storage, coordinator);

ulong Reserve() => (ulong)storage.ReserveTransactionSequence();
IReadOnlyDictionary<string, object?> Props(string key, object value) =>
    new Dictionary<string, object?> { [key] = value };

var labelIds = new Dictionary<string, Guid>
{
    ["Person"] = Guid.Parse("10000000-0000-0000-0000-000000000002"),
    ["Admin"] = Guid.Parse("10000000-0000-0000-0000-000000000003")
};
var seed = await coordinator.BeginAsync(IsolationLevel.Snapshot);
var alice = await store.CreateNodeAsync(
    new StoredGraphNode(Reserve(), ["Person"], Props("name", "Alice")), labelIds, seed);
var bob = await store.CreateNodeAsync(
    new StoredGraphNode(Reserve(), ["Person"], Props("name", "Bob")), labelIds, seed);
Guid typeId = Guid.Parse("10000000-0000-0000-0000-000000000001");
var edge = await store.CreateRelationshipAsync(
    new StoredGraphRelationship(Reserve(), alice.Id, bob.Id, "KNOWS", Props("since", 2020)), typeId, seed);
await coordinator.CommitAsync(seed);

var old = await coordinator.BeginAsync(IsolationLevel.Snapshot);
var writer = await coordinator.BeginAsync(IsolationLevel.Snapshot);
await store.UpdateNodeAsync(alice.Id, ["Person", "Admin"], Props("name", "Ada"), labelIds, writer);
await store.UpdateRelationshipAsync(edge.Id, Props("since", 2024), writer);
await coordinator.CommitAsync(writer);

Console.WriteLine(store.FindNode(alice.Id, old.Snapshot)!.Value.Properties["name"]); // Alice
Console.WriteLine(store.FindRelationship(edge.Id, old.Snapshot)!.Value.Properties["since"]); // 2020
var current = await coordinator.BeginAsync(IsolationLevel.Snapshot);
Console.WriteLine(store.GetNodes("Admin", old.Snapshot).Count); // 0
Console.WriteLine(store.GetNodes("Admin", current.Snapshot).Count); // 1
Console.WriteLine(store.FindNode(alice.Id, current.Snapshot)!.Value.Properties["name"]); // Ada
Console.WriteLine(store.FindRelationship(edge.Id, current.Snapshot)!.Value.Properties["since"]); // 2024
await foreach (var selected in store.GetIncidentAsync(alice.Id,
    GraphIncidentDirection.Outgoing, [typeId, typeId], current.Snapshot))
{
    Console.WriteLine(selected.Id == edge.Id); // True, once
}
await coordinator.CommitAsync(current);
await coordinator.CommitAsync(old);
```

Label and relationship-type GUIDs are supplied explicitly at this low level; the engine resolves them from the catalog and
the store does not validate its pairing with the type name. The selected read follows
GraphStoreAdjacencyTests and returns only outgoing KNOWS edges for Alice, without duplicate visits.

Old snapshots retain label postings, property-index and adjacency entries along with records. Rollback restores earlier
versions; a stale Snapshot updater throws TransactionAbortedException. Low-level Update always
publishes a version, even if all supplied values equal the visible values. GQL filters pure no-ops.

[All examples](storage.md) · [GraphStore](../graph-store.md) · [Assembly](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreUpdateTests.cs`.
