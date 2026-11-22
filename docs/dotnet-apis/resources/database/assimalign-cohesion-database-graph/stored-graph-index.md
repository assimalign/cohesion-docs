# StoredGraphIndex

A snapshot-visible node-property index and its exact/text kind.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`.

## Signature

```csharp
public readonly record struct StoredGraphIndex(string Label, string PropertyKey, bool IsText = false);
```

## Behavior

[GraphStore.GetIndexes](graph-store.md#index-kinds-and-write-accounting) returns each visible
label/property/kind once. IsText is true for case-sensitive Unicode trigram postings and false
for exact scalar keys. The optional constructor argument preserves exact-index construction.
Both kinds can coexist on a label/property pair. This value carries no tree identity or name;
[GraphIndexMetadata](graph-index-metadata.md) provides named catalog metadata.

## Related reference

[Assembly](index.md) · [Storage design](storage-design.md#secondary-indexes)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Storage/ValueObjects/StoredGraphIndex.cs`.
