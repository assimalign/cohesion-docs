# SqlCatalogColumn

Immutable catalog column metadata, including an optional identity facet.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public sealed class SqlCatalogColumn
{
    public SqlCatalogColumn(string name, DatabaseTypeInfo type, bool isNullable = true,
        string? defaultLiteral = null, Collation? collation = null, string? domainName = null,
        IReadOnlyList<string>? defaultFunctionCalls = null, SqlCatalogIdentity? identity = null,
        string? missingValueLiteral = null);
    public SqlCatalogIdentity? Identity { get; }
    public bool IsNullable { get; }
    public string? DefaultLiteral { get; }
    public string? MissingValueLiteral { get; }
}
```

## Behavior and exceptions

Identity is null for an ordinary column. Supplying [SqlCatalogIdentity](sql-catalog-identity.md)
forces IsNullable false even if isNullable is true. The facet is immutable and separate from the
durable allocation counter. Existing name/type/default/collation/domain metadata remains available.
DefaultFunctionCalls is copied into a read-only collection; identity validity is checked when the
catalog publishes/loads the table, not fully by this constructor.

MissingValueLiteral is the frozen canonical ADD COLUMN literal used for physically absent trailing
fields; null means a missing field reads NULL. It is independent of the current insert DefaultLiteral
and is preserved through SET/DROP DEFAULT, renames and restart. Reading old rows never executes the
insert default. Later type narrowing validates the frozen literal even without live rows. Table
extension 6 persists this split, introduced by format 17; current SQL storage format is 21.

Null name/type throws ArgumentNullException. Blank name/domain name, collation on a non-string
type, or a blank function identity throws ArgumentException. Invalid identity metadata at publication
or load throws SqlCatalogException; use no domain or DEFAULT and one built-in integer identity.

DefaultLiteral retains canonical SQL for a scalar expression as well as a literal; DefaultFunctionCalls
records resolved call identities. SQL DDL folds Immutable defaults at declaration and runs Stable/Volatile
calls per assignment on the transaction clock. Opening/rebinding never evaluates an expression default.
ADD COLUMN freezes a nonvolatile result in MissingValueLiteral; a volatile default on live rows uses
COHSQLE027. See [DEFAULT](../../../../database/sql/language/clauses/default.md).

## Example

```csharp
var column = new SqlCatalogColumn("id", new DatabaseTypeInfo(DatabaseType.Int64),
    identity: new SqlCatalogIdentity(true, 100, 5));
Console.WriteLine(column.IsNullable); // False
Console.WriteLine(column.Identity!.Start); // 100
```

```csharp
var extra = new SqlCatalogColumn("v", new DatabaseTypeInfo(DatabaseType.Int32),
    defaultLiteral: "9", missingValueLiteral: "7");
Console.WriteLine(extra.DefaultLiteral);      // 9: future omitted-column writes
Console.WriteLine(extra.MissingValueLiteral); // 7: old missing fields
```

See [allocation](sql-catalog.md#example) for publishing the column and obtaining values.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 adds namespace records of kind 15. Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCatalogIdentity](sql-catalog-identity.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogColumn.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Identity.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogTests.Rename.cs`.
