# SqlCatalogNamespace

Immutable, persisted SQL namespace metadata, including empty namespaces.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public sealed class SqlCatalogNamespace
{
    public SqlCatalogNamespace(string name, DatabaseObjectOwner owner = DatabaseObjectOwner.Adhoc,
        string? owningSchema = null);
    public string Name { get; }
    public DatabaseObjectOwner Owner { get; }
    public string? OwningSchema { get; }
}
```

## Behavior and exceptions

Name retains spelling; directory identity is case-insensitive. Owner/OwningSchema identify compiled
ownership, independently of the SQL namespace name. The constructor rejects blank names or invalid
ownership metadata with ArgumentException and unknown owner values with ArgumentOutOfRangeException.
SQL engine creation seeds dbo and protects it from DROP; opening a low-level SqlCatalog does not seed it.
Namespace records use kind 15, introduced in format 21. Current format 25 refuses earlier files
without an upgrade path.

## Example

With an open catalog, following SqlCatalogNamespaceTests:

```csharp
await catalog.CreateSchemaAsync(new SqlCatalogNamespace("Sales", DatabaseObjectOwner.Schema, "App"));
var captured = catalog.CaptureSnapshot();
await catalog.CreateSchemaAsync(new SqlCatalogNamespace("empty"));
Console.WriteLine(captured.Namespaces.Single(s => s.Name == "Sales").OwningSchema); // App
Console.WriteLine(captured.Namespaces.Any(s => s.Name == "empty")); // False
```

## Related reference

[SqlCatalog](sql-catalog.md) · [SqlCatalogSnapshot](sql-catalog-snapshot.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogNamespace.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogNamespaceTests.cs`.
