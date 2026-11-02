# SqlCatalogIndex

Published index description with immutable column, predicate-dependency and key-order vectors.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public SqlCatalogIndex(ulong tableObjectId, string name, IReadOnlyList<string> columnNames,
    bool isUnique, DatabaseObjectOwner owner = DatabaseObjectOwner.Adhoc,
    string? owningSchema = null, bool isPrimaryKey = false, string? predicate = null,
    IReadOnlyList<string>? predicateColumns = null,
    IReadOnlyList<string>? predicateFunctionCalls = null,
    IReadOnlyList<SqlCatalogIndexKeyOrder>? keyOrders = null);

public IReadOnlyList<SqlCatalogIndexKeyOrder> KeyOrders { get; }

public enum SqlCatalogIndexKeyOrder
{
    AscendingNullsLast = 0,
    AscendingNullsFirst = 1,
    DescendingNullsLast = 2,
    DescendingNullsFirst = 3
}
```

## Behavior and exceptions

Omitting keyOrders supplies ASC NULLS LAST per key. The constructor copies the inputs;
KeyOrders is aligned with ColumnNames. An invalid vector width/value, or any nondefault
primary-key order, throws SqlCatalogException with CS03085 (SQLSTATE 42P17).
A primary key must also be unique (ArgumentException) and unconditional (CS03082, 42P16).
Blank predicate text or dependencies without text use CS03084 (42P17).

A null column list throws ArgumentNullException; a blank name throws ArgumentException;
an empty key list uses CS03059 (42P17). Constructing a description does not build or publish
an index. The catalog persists effective orders in format 32's mandatory index extension 3;
malformed loaded vectors use CS06059 and unsupported extensions CS06060.

## Example

Inspect a directed index as in SqlDirectedIndexTests, with an open database and session:

```csharp
await session.ExecuteAsync("CREATE TABLE t(k INT)");
await session.ExecuteAsync("CREATE INDEX ix ON t(k DESC NULLS LAST)");
var index = database.Catalog.GetIndexes(database.Catalog.Tables.Single().ObjectId).Single();
Console.WriteLine(index.KeyOrders[0]); // DescendingNullsLast
```

## Related reference

[Assembly](index.md) · [SqlCatalog](sql-catalog.md) · [CREATE INDEX](../../../../database/sql/language/statements/create-index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalogIndex.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalogIndexKeyOrder.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDirectedIndexTests.cs`.
