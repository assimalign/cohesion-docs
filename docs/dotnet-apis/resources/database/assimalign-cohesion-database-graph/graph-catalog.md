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
public IReadOnlyList<GraphLabelMetadata> GetLabels(TransactionSnapshot snapshot);
public ValueTask SaveLabelAsync(GraphLabelMetadata label, TransactionContext context,
    CancellationToken cancellationToken = default);
```

`GraphCatalog.Open` returns a sealed `GraphCatalog` over graph storage and its transaction coordinator.
`GraphLabelMetadata` and `GraphRelationshipTypeMetadata` carry stable GUIDs, case-sensitive names,
ownership, and owning schema. `GraphPropertyKeyMetadata` declares optional value type and
requiredness. `GraphIndexMetadata` names a node-property index.

Reads use a snapshot; writes join the supplied transaction without committing it. Changing a
schema-owned definition throws DatabaseObjectLockedException. Null storage/coordinator arguments
throw ArgumentNullException. FindLabel returns null when the exact case-sensitive name is absent.

## Example

With storage and its coordinator already open and recovery complete:

```csharp
var catalog = GraphCatalog.Open(storage, coordinator);
var writer = await coordinator.BeginAsync(IsolationLevel.Snapshot);
await catalog.SaveLabelAsync(new GraphLabelMetadata(Guid.NewGuid(), "Person"), writer);
await coordinator.CommitAsync(writer);
var reader = await coordinator.BeginAsync(IsolationLevel.Snapshot);
Console.WriteLine(catalog.FindLabel("Person", reader.Snapshot)!.Name); // Person
Console.WriteLine(catalog.FindLabel("person", reader.Snapshot) is null); // True
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
