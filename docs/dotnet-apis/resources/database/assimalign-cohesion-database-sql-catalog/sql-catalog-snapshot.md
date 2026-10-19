# SqlCatalogSnapshot

An immutable capture of namespaces, tables, indexes, functions, domains and the default collation.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public sealed class SqlCatalogSnapshot
{
    public IReadOnlyList<SqlCatalogNamespace> Namespaces { get; }
    public IReadOnlyList<SqlCatalogTable> Tables { get; }
    public IReadOnlyList<SqlCatalogView> Views { get; }
    public IReadOnlyList<SqlCatalogFunction> Functions { get; }
    public IReadOnlyList<SqlCatalogDomain> Domains { get; }
    public Collation DefaultCollation { get; }
    public IReadOnlyList<SqlCatalogIndex> GetIndexes(ulong objectId);
    public bool TryGetTable(string schema, string name, out SqlCatalogTable table);
}
```

## Behavior and exceptions

Obtain a capture from SqlCatalog.CaptureSnapshot; construction is internal. All descriptions are
captured together under the publication lock. Later publications cannot change them. Namespaces
includes empty namespaces. The capture owns no storage/disposal lifetime. GetIndexes returns an
empty list when absent; TryGetTable compares schema/name without case and returns false/null
when absent. These members declare no validation exceptions. Capturing an offline catalog throws
StorageOfflineException. INFORMATION_SCHEMA.SCHEMATA projects Namespaces from the statement capture.

Views contains the immutable [SqlCatalogView](sql-catalog-view.md) definitions present at capture.
Later replacement or DROP leaves that list intact. Captures include view definitions alongside tables,
functions, domains and namespaces.

## Example

With an open catalog:

```csharp
await catalog.CreateSchemaAsync(new SqlCatalogNamespace("sales"));
var snapshot = catalog.CaptureSnapshot();
await catalog.DropSchemaAsync("sales");
Console.WriteLine(snapshot.Namespaces.Any(s => s.Name == "sales")); // True
Console.WriteLine(catalog.Namespaces.Any(s => s.Name == "sales")); // False
```

## Related reference

[SqlCatalog](sql-catalog.md) · [SqlCatalogNamespace](sql-catalog-namespace.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogSnapshot.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogNamespaceTests.cs`.
