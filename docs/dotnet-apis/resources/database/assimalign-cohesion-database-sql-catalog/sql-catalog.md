# SqlCatalog

The concrete durable SQL catalog; its identity allocator reserves values independently of row transactions.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Namespace members

```csharp
public IReadOnlyList<SqlCatalogNamespace> Namespaces { get; }
public ValueTask CreateSchemaAsync(SqlCatalogNamespace schema, CancellationToken cancellationToken = default);
public ValueTask CreateSchemaAsync(SqlCatalogNamespace schema, SqlCatalogScriptPublication publication,
    CancellationToken cancellationToken = default);
public ValueTask DropSchemaAsync(string name, bool cascade = false, CancellationToken cancellationToken = default);
public ValueTask DropSchemaAsync(string name, bool cascade, SqlCatalogScriptPublication publication,
    CancellationToken cancellationToken = default);
public SqlCatalogSnapshot CaptureSnapshot();
```

Namespaces is an immutable current capture. [CaptureSnapshot](sql-catalog-snapshot.md) captures
namespace/table/index/function/domain descriptions and default collation together. Namespace
creation/deletion commits durably before publication; DdlGeneration advances with publication.
CASCADE drops contained tables/indexes/identity counters and removes incoming FKs while preserving
outside tables. Receipt overloads validate the target, preceding receipt and generation atomically;
a guarded absent drop can publish its absence receipt.

Null required objects throw ArgumentNullException; blank drop names ArgumentException. Duplicates,
missing namespaces, RESTRICT dependencies, oversized records and stale publications use
SqlCatalogException. Cancellation before publication uses OperationCanceledException; persistence
uses StorageException and offline access StorageOfflineException. The SQL executor owns locks,
ownership and dbo/virtual-namespace protection; direct catalog operations serialize publication only.

```csharp
await catalog.CreateSchemaAsync(new SqlCatalogNamespace("sales"));
var capture = catalog.CaptureSnapshot();
await catalog.DropSchemaAsync("sales", cascade: true);
Console.WriteLine(capture.Namespaces.Any(s => s.Name == "sales")); // True
Console.WriteLine(catalog.Namespaces.Any(s => s.Name == "sales")); // False
```

This follows SqlCatalogNamespaceTests with an open catalog. Low-level open does not seed dbo;
SQL database creation does. See [SqlCatalogNamespace](sql-catalog-namespace.md).

## Identity member

```csharp
public sealed partial class SqlCatalog
{
    public long AllocateIdentity(SqlCatalogTable table);
}
```

## Behavior and exceptions

The supplied table must be the currently published description and contain an identity column.
Under the catalog lock, allocation commits a reservation of up to 32 values before returning any.
It preserves the table object and DdlGeneration and does not publish script receipts. Allocation
uses Int128 intermediate arithmetic and the integer type's direction-dependent bounds.

Data rollback, failed constraints and restart do not reclaim allocations. Reopening starts after
the last reserved value, including after clean shutdown, so unused values become gaps. Generated
allocations for that table identity never repeat and are not ordered by data commit. DROP TABLE or
dropping its identity column atomically retires the counter; recreation uses a new ObjectId and START.

Null table throws ArgumentNullException. A stale description, missing identity or exhausted counter
throws SqlCatalogException. Reservation persistence can throw StorageException. An unconfirmed
reservation commit fences the catalog through StorageOfflineException until recovery/reopen;
no caller receives a value before the reservation commit succeeds. SQL translates identity contract
failures to `COHSQLE025`.

## Example

With an open concrete catalog:

```csharp
var table = await catalog.CreateTableAsync("dbo", "ids", [
    new SqlCatalogColumn("id", new DatabaseTypeInfo(DatabaseType.Int64),
        identity: new SqlCatalogIdentity(false, 100, 5))
]);
var generation = catalog.DdlGeneration;
Console.WriteLine(catalog.AllocateIdentity(table)); // 100
Console.WriteLine(catalog.AllocateIdentity(table)); // 105
Console.WriteLine(catalog.DdlGeneration == generation); // True
```

The co-located reservation/reopen test then allocates 260 from the reopened table description.

## Rename members

```csharp
public ValueTask<SqlCatalogTable> RenameTableAsync(SqlCatalogTable expected, string newName,
    IReadOnlyDictionary<string, string> checkExpressions, CancellationToken cancellationToken = default);
public ValueTask<SqlCatalogTable> RenameTableAsync(SqlCatalogTable expected, string newName,
    IReadOnlyDictionary<string, string> checkExpressions, SqlCatalogScriptPublication publication,
    CancellationToken cancellationToken = default);
public ValueTask<SqlCatalogTable> RenameColumnAsync(SqlCatalogTable expected, string columnName, string newName,
    IReadOnlyDictionary<string, string> checkExpressions, CancellationToken cancellationToken = default);
public ValueTask<SqlCatalogTable> RenameColumnAsync(SqlCatalogTable expected, string columnName, string newName,
    IReadOnlyDictionary<string, string> checkExpressions, SqlCatalogScriptPublication publication,
    CancellationToken cancellationToken = default);
```

Supply the current immutable expected table and engine-rendered canonical text for every CHECK.
The catalog does not parse SQL. Table/column definitions, incoming/self FKs and index column lists
publish in one catalog transaction, preserving ObjectId, physical ordinals, row versions and trees.
Generated constraint/index names stay unchanged. Publication overloads also validate the expected
script target, preceding receipt and generation. DdlGeneration advances after publication.

Null arguments throw ArgumentNullException, blank required names ArgumentException; stale definitions,
collisions or incomplete/oversized CHECK metadata throw SqlCatalogException. Invalid UTF-16 uses
DatabaseTypeException, persistence StorageException and cancellation before publication
OperationCanceledException. The SQL engine performs ownership/dependency checks and rewrites CHECK
identifiers before calling these members.

### Rename example

With an open concrete catalog, adapted from SqlCatalogTests.Rename:

```csharp
var parent = await catalog.CreateTableAsync("dbo", "p", [
    new SqlCatalogColumn("id", new DatabaseTypeInfo(DatabaseType.Int32), isNullable: false)
], ["id"], CancellationToken.None);
var id = parent.ObjectId;
parent = await catalog.RenameTableAsync(parent, "renamed", new Dictionary<string, string>());
parent = await catalog.RenameColumnAsync(parent, "id", "key_id", new Dictionary<string, string>());
Console.WriteLine(parent.ObjectId == id); // True
Console.WriteLine(parent.PrimaryKeyColumns[0]); // key_id
```

This table has no CHECKs, so the dictionaries are empty. Use each returned definition as the next
expected argument; the earlier definition is stale after publication.

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCatalogColumn](sql-catalog-column.md) · [SqlCatalogIdentity](sql-catalog-identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalog.Identity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalog.Rename.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Rename.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Identity.cs`.
