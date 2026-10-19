# StoredGraphNode

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

## Signature

```csharp
public readonly record struct StoredGraphNode(ulong Id, IReadOnlyList<string> Labels,
    IReadOnlyDictionary<string, object?> Properties);
```

## Behavior and exceptions

This value record carries one entity version. The constructor retains collections without validation.
Create now accepts the complete record: reserve a nonzero Id from the backing storage's
ReserveTransactionSequence. The store validates names/scalar properties, copies collections and
rejects duplicate identities. Updates keep identity.
Earlier snapshots can read earlier values in the format-3 chain.

[GraphStore](graph-store.md) documents ArgumentException/ArgumentNullException for invalid values,
ArgumentOutOfRangeException for zero create IDs and GraphElementTooLargeException for bounds.
Record construction alone does not write storage.

## Example

```csharp
var record = new StoredGraphNode((ulong)storage.ReserveTransactionSequence(),
    ["Person"], new Dictionary<string, object?> { ["name"] = "Ada" });
var stored = await store.CreateNodeAsync(record, context);
Console.WriteLine(stored.Id == record.Id); // True
```

This assumes an active caller context; see the [complete example](examples/graph-store-tests.md).

## Related reference

[GraphStore](graph-store.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/ValueObjects/StoredGraphNode.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/tests/GraphStoreUpdateTests.cs`.
