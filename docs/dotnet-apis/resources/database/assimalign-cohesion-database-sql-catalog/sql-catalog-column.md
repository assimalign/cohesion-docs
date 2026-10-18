# SqlCatalogColumn

Immutable catalog column metadata, including an optional identity facet.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public sealed class SqlCatalogColumn
{
    public SqlCatalogColumn(string name, DatabaseTypeInfo type, bool isNullable = true,
        string? defaultLiteral = null, Collation? collation = null, string? domainName = null,
        IReadOnlyList<string>? defaultFunctionCalls = null, SqlCatalogIdentity? identity = null);
    public SqlCatalogIdentity? Identity { get; }
    public bool IsNullable { get; }
}
```

## Behavior and exceptions

Identity is null for an ordinary column. Supplying [SqlCatalogIdentity](sql-catalog-identity.md)
forces IsNullable false even if isNullable is true. The facet is immutable and separate from the
durable allocation counter. Existing name/type/default/collation/domain metadata remains available.
DefaultFunctionCalls is copied into a read-only collection; identity validity is checked when the
catalog publishes/loads the table, not fully by this constructor.

Null name/type throws ArgumentNullException. Blank name/domain name, collation on a non-string
type, or a blank function identity throws ArgumentException. Invalid identity metadata at publication
or load throws SqlCatalogException; use no domain or DEFAULT and one built-in integer identity.

## Example

```csharp
var column = new SqlCatalogColumn("id", new DatabaseTypeInfo(DatabaseType.Int64),
    identity: new SqlCatalogIdentity(true, 100, 5));
Console.WriteLine(column.IsNullable); // False
Console.WriteLine(column.Identity!.Start); // 100
```

See [allocation](sql-catalog.md#example) for publishing the column and obtaining values.

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCatalogIdentity](sql-catalog-identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogColumn.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Identity.cs`.
