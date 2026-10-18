# StoredGraphRelationship

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

## Signature

```csharp
public readonly record struct StoredGraphRelationship(ulong Id, ulong SourceId, ulong TargetId,
    string Type, IReadOnlyDictionary<string, object?> Properties);
```

## Behavior and exceptions

This value record carries one entity version. The constructor retains collections without validation.
Create now accepts the complete record: reserve a nonzero Id from the backing storage's
ReserveTransactionSequence. The store validates names/scalar properties, copies collections and
rejects duplicate identities. Updates keep identity, endpoints and type.
Earlier snapshots can read earlier values in the format-2 chain.

[GraphStore](graph-store.md) documents ArgumentException/ArgumentNullException for invalid values,
ArgumentOutOfRangeException for zero create IDs and GraphElementTooLargeException for bounds.
Record construction alone does not write storage.

## Example

```csharp
var record = new StoredGraphRelationship((ulong)storage.ReserveTransactionSequence(),
    source.Id, target.Id, "KNOWS", new Dictionary<string, object?> { ["since"] = 2024 });
var stored = await store.CreateRelationshipAsync(record, context);
Console.WriteLine(stored.Id == record.Id); // True
```

This assumes an active caller context and existing endpoints; see the [complete example](examples/graph-store-tests.md).

## Related reference

[GraphStore](graph-store.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/src/ValueObjects/StoredGraphNode.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/tests/GraphStoreUpdateTests.cs`.
