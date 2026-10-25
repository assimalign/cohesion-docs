# SqlCatalogView

Immutable canonical view metadata with frozen columns and resolved dependencies.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public sealed class SqlCatalogView
{
    public SqlCatalogView(string schema, string name, string definition,
        IReadOnlyList<SqlCatalogColumn> columns, IReadOnlyList<ulong> tableDependencies,
        IReadOnlyList<string> dependencies, IReadOnlyList<string> functionCalls,
        DatabaseObjectOwner owner = DatabaseObjectOwner.Adhoc, string? owningSchema = null);
    public string Schema { get; }
    public string Name { get; }
    public string Definition { get; }
    public IReadOnlyList<SqlCatalogColumn> Columns { get; }
    public IReadOnlyList<ulong> TableDependencies { get; }
    public IReadOnlyList<string> Dependencies { get; }
    public IReadOnlyList<string> FunctionCalls { get; }
    public DatabaseObjectOwner Owner { get; }
    public string? OwningSchema { get; }
}
```

## Behavior and exceptions

The constructor copies collections and normalizes columns to nullable metadata, retaining
names, types/modifiers, domains and collations. TableDependencies contains distinct stored ObjectIds;
Dependencies contains distinct case-insensitive view/column/function/domain identities;
FunctionCalls preserves traversal order. Definition must be engine-validated canonical SQL with
expanded stars and qualified bound inputs. The catalog does not parse or bind SQL.

Null collections throw ArgumentNullException. Blank names/definition, empty or invalid columns,
duplicate column names, blank dependency/call identities, zero table identities and invalid
ownership throw ArgumentException. Construction alone does not publish a view.
[SqlCatalog.CreateViewAsync](sql-catalog.md#view-members) enforces publication invariants and the
8,092-byte record cap, with SqlCatalogException carrying CS03033 for view-specific refusals.

## Example

After executing CREATE TABLE t (id INT) and CREATE VIEW v AS SELECT id FROM t through
a SQL session whose database exposes Catalog:

```csharp
SqlCatalogView view = database.Catalog.Views.Single(v => v.Name == "v");
Console.WriteLine(view.Columns[0].Name);       // id
Console.WriteLine(view.Columns[0].IsNullable); // True
Console.WriteLine(view.TableDependencies.Count); // 1
```

These assertions follow the view execution tests. Prefer session DDL to hand-building bound
dependency identities.

## Related reference

[Assembly](index.md) · [SqlCatalog](sql-catalog.md) · [SqlCatalogSnapshot](sql-catalog-snapshot.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalogView.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlViewTests.TypeFacets.cs`.
