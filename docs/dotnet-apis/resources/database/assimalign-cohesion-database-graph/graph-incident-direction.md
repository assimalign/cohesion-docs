# GraphIncidentDirection

Selects relationship direction relative to the node in a filtered incident read.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public enum GraphIncidentDirection
{
    Both,
    Outgoing,
    Incoming
}
```

## Behavior and exceptions

Outgoing selects relationships whose source is the incident node; Incoming selects those whose
target is that node. Both selects both directions and returns each self-loop once. These values
belong to the storage API; the engine's GraphDirection is a separate enum.

[GraphStore.GetIncidentAsync](graph-store.md#incident-reads) throws ArgumentOutOfRangeException
for an undefined value during enumeration. Its catalog type selection is independent of direction.

## Example

With a seeded store, a retained reader snapshot and the nonempty typeId used at creation:

```csharp
await foreach (var edge in store.GetIncidentAsync(node.Id,
    GraphIncidentDirection.Outgoing, [typeId, typeId], reader.Snapshot))
{
    Console.WriteLine(edge.SourceId == node.Id); // True for every result
}
```

Duplicate IDs select the same range once. A seeded self-loop appears once with Outgoing, Incoming
or Both. This follows GraphStoreAdjacencyTests; the [complete example](examples/graph-store-tests.md)
includes creation and snapshot lifetime.

## Related reference

[Assembly](index.md) · [GraphStore](graph-store.md) · [Design](storage-design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Storage/GraphIncidentDirection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreAdjacencyTests.cs`.
