# GraphCatalog

The `GraphCatalog` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Implemented.

Opens the metadata catalog belonging to one logical graph database.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

```csharp
public static GraphCatalog Open(GraphStorage storage, TransactionCoordinator coordinator);
public GraphLabelMetadata? FindLabel(string name, TransactionSnapshot snapshot);
public GraphLabelMetadata? FindLabel(Guid id, TransactionSnapshot snapshot);
public GraphRelationshipTypeMetadata? FindRelationshipType(Guid id, TransactionSnapshot snapshot);
public IReadOnlyList<GraphLabelMetadata> GetLabels(TransactionSnapshot snapshot);
public GraphIndexMetadata? FindIndex(string name, TransactionSnapshot snapshot);
public IReadOnlyList<GraphIndexMetadata> GetIndexes(TransactionSnapshot snapshot);
public GraphConstraintMetadata? FindConstraint(string name, TransactionSnapshot snapshot);
public IReadOnlyList<GraphConstraintMetadata> GetConstraints(TransactionSnapshot snapshot);
public IReadOnlyList<GraphConstraintMetadata> GetConstraints(Guid definitionId, TransactionSnapshot snapshot);
public ValueTask SaveLabelAsync(GraphLabelMetadata label, TransactionContext context,
    CancellationToken cancellationToken = default);
```

`GraphCatalog.Open` returns a sealed `GraphCatalog` over graph storage and its transaction coordinator.
`GraphLabelMetadata` and `GraphRelationshipTypeMetadata` carry stable GUIDs, case-sensitive names,
ownership, and owning schema. `GraphPropertyKeyMetadata` declares optional value type and
requiredness. `GraphIndexMetadata` names a node-property index.

[GraphConstraintMetadata](graph-constraint-metadata.md) names an independent type or existence
rule. Index/constraint names share one database-wide ordinal, case-sensitive namespace. Find
returns null when absent; database-wide lists use ordinal name order. Catalog property reads
combine named and declared rules; GraphSchema.GetPropertyKeysAsync deliberately reads declared
metadata only. Use [GraphSchema](graph-schema.md) for creation with existing-data validation and
coordinated physical index removal.

All catalog record kinds use v2. Earlier/future versions refuse with CG06019, with no legacy
reader. See [catalog format](catalog-design.md#on-disk-format).

Reads use a snapshot; writes join the supplied transaction without committing it. Changing a
schema-owned definition throws DatabaseObjectLockedException. Null storage/coordinator arguments
throw ArgumentNullException. FindLabel returns null when the exact case-sensitive name is absent.

GUID lookups select only the requested identity's visible definition, returning null for an absent
identity or the wrong kind. The identity directory and parent-scoped property/index directory are
in memory; reads still decode and check MVCC versions. Recreating a dropped name with a new GUID
keeps old-snapshot lookup and never returns that replacement for the old GUID.

Null snapshots throw ArgumentNullException; malformed records throw GraphCatalogException.
Save methods refuse an encoded metadata record above 8,092 bytes with GraphElementTooLargeException
before physical apply. Engine/GraphSchema calls translate that size failure to CG02009 and catalog
failures as described in [GraphCatalogException](graph-catalog-exception.md).

## Example

With storage and its coordinator already open and recovery complete:

```csharp
var catalog = GraphCatalog.Open(storage, coordinator);
var writer = await coordinator.BeginAsync(IsolationLevel.Snapshot);
var label = new GraphLabelMetadata(Guid.NewGuid(), "Person");
await catalog.SaveLabelAsync(label, writer);
await coordinator.CommitAsync(writer);
var reader = await coordinator.BeginAsync(IsolationLevel.Snapshot);
Console.WriteLine(catalog.FindLabel("Person", reader.Snapshot)!.Value.Name); // Person
Console.WriteLine(catalog.FindLabel("person", reader.Snapshot) is null); // True
Console.WriteLine(catalog.FindLabel(label.Id, reader.Snapshot)!.Value.Name); // Person
Console.WriteLine(catalog.FindRelationshipType(label.Id, reader.Snapshot) is null); // True
await coordinator.RollbackAsync(reader);
```

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](catalog-design.md).
- **Examples** — [Source-backed usage](examples/catalog.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph.Catalog/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/GraphCatalog.cs`.
- **Snapshot and name cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Catalog/GraphCatalogTests.cs`.
- **Identity snapshots and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Catalog/GraphCatalogIdentityTests.cs` and `../GraphHygieneTests.cs`.
