# SqlCatalogIdentity

Immutable generation metadata; the allocation counter is stored separately.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public sealed class SqlCatalogIdentity
{
    public SqlCatalogIdentity(bool isAlways, long start = 1, long increment = 1);
    public bool IsAlways { get; }
    public long Start { get; }
    public long Increment { get; }
}
```

## Behavior and exceptions

IsAlways distinguishes ALWAYS from BY DEFAULT. Start and Increment define the progression;
construction does not allocate. Unlike the SQL parser, the API's start default is always 1, so
supply a negative start explicitly for descending generation. Zero increment throws
ArgumentOutOfRangeException. Other metadata checks belong to catalog publication/load: one
identity per table, signed built-in integer storage, direction-dependent bounds, no domain/default,
and NOT NULL. Invalid metadata raises SqlCatalogException.

[SqlCatalogColumn.Identity](sql-catalog-column.md) holds this facet and forces IsNullable false.
Table extension 6 stores the definition (the identity facet originated in extension 5); catalog kind 16 stores the last durable reserved
value. Current SQL format 21 refuses older files. Allocation uses [SqlCatalog.AllocateIdentity](sql-catalog.md).

## Example

With an open concrete catalog:

```csharp
var table = await catalog.CreateTableAsync("dbo", "ids", [
    new SqlCatalogColumn("id", new DatabaseTypeInfo(DatabaseType.Int64),
        identity: new SqlCatalogIdentity(false, 100, 5))
]);
Console.WriteLine(table.Columns[0].IsNullable); // False
Console.WriteLine(catalog.AllocateIdentity(table)); // 100
Console.WriteLine(catalog.AllocateIdentity(table)); // 105
```

These calls allocate values without inserting rows. After reopening, the test's next allocation
is 260 because the first 32-value reservation ended at 255.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 adds namespace records of kind 15. Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## Related reference

[Assembly](index.md) · [Design](design.md) · [IDENTITY](../../../../database/sql/language/clauses/identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogIdentity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Identity.cs`.
