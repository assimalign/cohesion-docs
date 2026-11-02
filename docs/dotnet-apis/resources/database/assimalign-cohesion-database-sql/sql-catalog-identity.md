# SqlCatalogIdentity

Immutable generation metadata; the allocation counter is stored separately.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

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
Table extension 8 stores this facet; its owned sequence uses kind 18 and kind-19 reservations.
Kind 16 is retired. SQL format 32 refuses format 30 and earlier files without an upgrade reader.
Allocation uses [SqlCatalog.AllocateIdentity](sql-catalog.md). SQL identity allocations also update
session currval/lastval; direct catalog allocation has no session observation context.

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
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Format 30 added ON UPDATE action metadata in table extension 8.
Format 31 added partial-index predicates in index extension 2. Current format 32 adds per-column
direction and NULL placement in mandatory index extension 3 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 11.

## Related reference

[Assembly](index.md) · [Design](catalog-design.md) · [IDENTITY](../../../../database/sql/language/clauses/identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalogIdentity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/Catalog/SqlCatalogTests.Identity.cs`.
