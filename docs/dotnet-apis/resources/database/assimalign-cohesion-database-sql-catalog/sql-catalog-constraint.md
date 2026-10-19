# SqlCatalogConstraint

Immutable persisted foreign-key or CHECK metadata.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`.

## Signature

```csharp
public enum SqlCatalogReferentialAction : byte
{ Restrict = 0, Cascade, NoAction, SetNull, SetDefault }
public sealed class SqlCatalogConstraint
{
    public SqlCatalogConstraint(string name, SqlCatalogConstraintKind kind,
        IReadOnlyList<string> columns, string? referencedSchema = null,
        string? referencedTable = null, IReadOnlyList<string>? referencedColumns = null,
        string? checkExpression = null,
        SqlCatalogReferentialAction onDelete = SqlCatalogReferentialAction.Restrict,
        IReadOnlyList<string>? checkFunctionCalls = null);
    public string Name { get; }
    public SqlCatalogConstraintKind Kind { get; }
    public IReadOnlyList<string> Columns { get; }
    public string? ReferencedSchema { get; }
    public string? ReferencedTable { get; }
    public IReadOnlyList<string> ReferencedColumns { get; }
    public string? CheckExpression { get; }
    public IReadOnlyList<string> CheckFunctionCalls { get; }
    public SqlCatalogReferentialAction OnDelete { get; }
}
```

## Behavior and exceptions

The constructor snapshots column/call lists. Its optional action remains Restrict; the SQL
parser supplies NoAction for omitted ON DELETE. Pass the intended action explicitly when building
catalog metadata. Format 20 persists NoAction, SetNull and SetDefault; every older database format
is refused at open with no upgrade path. Compiled schema version 7 retains only Restrict/Cascade;
reconciling it with a newer catalog action throws SqlSchemaMigrationException.

ArgumentNullException rejects null columns. ArgumentException rejects blank names, duplicate
columns, unknown enum values, mismatched/empty foreign-key lists, incomplete target names,
foreign-key CHECK text, or CHECK metadata with reference facets or a non-Restrict action. Blank
recorded function identities are also invalid. These are constructor validation exceptions,
separate from SQL execution diagnostics.

## Example

Adapted from SqlReferentialActionTests and SqlCatalogConstraintTests:

```csharp
var reference = new SqlCatalogConstraint("fk_c", SqlCatalogConstraintKind.Reference,
    ["pid"], "dbo", "p", ["id"], onDelete: SqlCatalogReferentialAction.SetDefault);
Console.WriteLine(reference.OnDelete); // SetDefault
```

## Related reference

[Assembly](index.md) · [Constraints](../../../../database/sql/language/constraints.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/src/SqlCatalogConstraint.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/tests/SqlCatalogConstraintTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`.
