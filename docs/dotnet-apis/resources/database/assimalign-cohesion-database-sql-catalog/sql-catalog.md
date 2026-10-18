# SqlCatalog

The concrete durable SQL catalog; its identity allocator reserves values independently of row transactions.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Changed member

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

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCatalogColumn](sql-catalog-column.md) · [SqlCatalogIdentity](sql-catalog-identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalog.Identity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Identity.cs`.
