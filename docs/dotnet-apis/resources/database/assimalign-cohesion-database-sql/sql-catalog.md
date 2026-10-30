# SqlCatalog

The concrete durable SQL catalog; its identity allocator reserves values independently of row transactions.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

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

## Index predicate metadata

`SqlCatalogIndex` adds optional constructor arguments `string? predicate = null`,
`IReadOnlyList<string>? predicateColumns = null` and
`IReadOnlyList<string>? predicateFunctionCalls = null` after `isPrimaryKey`.
Its read-only Predicate, PredicateColumns and PredicateFunctionCalls properties expose canonical
SQL, referenced columns and resolved function-call identities. Dependency inputs are copied.
NULL Predicate means unconditional. A partial primary key throws SqlCatalogException/CS03082
(42P16); blank text or dependencies without a predicate throw CS03084 (42P17).

After creating a partial index through SQL, inspect it as in SqlPartialIndexTests:

```csharp
await session.ExecuteAsync("CREATE TABLE t(k INT, active BOOLEAN)");
await session.ExecuteAsync("CREATE INDEX ix ON t(k) WHERE active");
var index = database.Catalog.GetIndexes(database.Catalog.Tables.Single().ObjectId).Single();
Console.WriteLine(index.Predicate); // active
```

See [CREATE INDEX](../../../../database/sql/language/statements/create-index.md#partial-indexes)
for engine admission, maintenance and unresolved registrations.

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
uses Int128 intermediate arithmetic and the owned sequence's resolved bounds and cycle policy.

Data rollback, failed constraints and restart do not reclaim allocations. Reopening starts after
the last reserved value, including after clean shutdown, so unused values become gaps. Generated
allocations are not ordered by data commit; deliberate CYCLE/restart can repeat them. DROP TABLE or
dropping its identity column atomically retires the counter; recreation uses a new ObjectId and START.

Null table throws ArgumentNullException. A stale description, missing identity or exhausted counter
throws SqlCatalogException. Reservation persistence can throw StorageException. An unconfirmed
reservation commit fences the catalog through StorageOfflineException until recovery/reopen;
no caller receives a value before the reservation commit succeeds. SQL identity declaration and
assignment failures retain CS03025; allocator exhaustion retains CS02002 (22003). Identity removal
during statement planning uses retryable CS04067 (40001).

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
Using that stale definition throws SqlCatalogException with CS04064 (SQLSTATE mapping 40001),
as checked by SqlCatalogTests.Rename. The code is in Diagnostics, with no duplicate Message prefix.
Session SQL adds its statement span; a direct catalog call supplies no statement span.

## Related reference

[Assembly](index.md) · [Design](catalog-design.md) · [SqlCatalogColumn](sql-catalog-column.md) · [SqlCatalogIdentity](sql-catalog-identity.md)

## View members

```csharp
public IReadOnlyList<SqlCatalogView> Views { get; }
public bool TryGetView(string schema, string name, out SqlCatalogView view);
public ValueTask CreateViewAsync(SqlCatalogView view, bool orReplace = false,
    CancellationToken cancellationToken = default);
public ValueTask CreateViewAsync(SqlCatalogView view, bool orReplace,
    SqlCatalogScriptPublication publication, CancellationToken cancellationToken = default);
public ValueTask DropViewAsync(string schema, string name, bool cascade = false,
    CancellationToken cancellationToken = default);
public ValueTask DropViewAsync(string schema, string name, bool cascade,
    SqlCatalogScriptPublication publication, CancellationToken cancellationToken = default);
```

Views returns an immutable capture; TryGetView compares namespace/name without case and returns
false/null when absent. Offline access throws StorageOfflineException. Create accepts an
engine-validated [SqlCatalogView](sql-catalog-view.md); it checks relation collisions, live
dependencies, cycles and compatible append-only replacement. View-specific refusals and records
larger than 8,092 bytes throw SqlCatalogException with CS03033. Drop refuses absence/table
targets with CS03033 and RESTRICT dependencies with CS03302. CASCADE removes dependent
views atomically. Script-publication overloads commit receipt and definition/absence together.

Null view/publication throws ArgumentNullException; stale publication uses SqlCatalogException.
Cancellation before publication throws OperationCanceledException and persistence failures
StorageException. Catalog calls do not perform engine SQL binding, ownership admission or object
locking.

After creating v through session SQL:

```csharp
var before = database.Catalog.CaptureSnapshot();
await database.Catalog.DropViewAsync("dbo", "v");
Console.WriteLine(before.Views.Any(v => v.Name == "v")); // True
Console.WriteLine(database.Catalog.TryGetView("dbo", "v", out _)); // False
```

This follows immutable captures in SqlViewTests.Persistence and the catalog removal implementation.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Identity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Rename.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/Catalog/SqlCatalogTests.Rename.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/Catalog/SqlCatalogTests.Identity.cs`.
