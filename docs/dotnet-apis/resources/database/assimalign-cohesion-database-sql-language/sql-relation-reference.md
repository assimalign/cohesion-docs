# SqlRelationReference

The common base for a named table and an aliased derived query in FROM or JOIN.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public abstract class SqlRelationReference
{
    public string? Alias { get; }
}
public sealed class SqlTableReference : SqlRelationReference
{
    public string TableName { get; }
    public string? SchemaName { get; }
}
```

## Behavior and exceptions

Constructors are internal (the base constructor is private protected). Obtain nodes from
SqlQueryParser; callers cannot supply additional relation subclasses. [SqlFromItem.Table](sql-from-item.md)
and [SqlJoinClause.Table](sql-join-clause.md) now return SqlRelationReference. Pattern-match the
named-table leaf before reading TableName/SchemaName, or the [derived leaf](sql-derived-table-reference.md)
before reading Query/Columns. Alias is inherited; it is required for a derived query and optional
for a named table. An explicit alias hides the underlying name in binding.

Accessors have no validation exceptions. Parser failures remain statement diagnostics; the engine
rejects unknown/hidden columns with `COHSQLE015` and duplicate relation bindings with `COHSQLE019`.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT * FROM dbo.t AS base, (SELECT id FROM t) AS d;");
var select = (SqlSelectExpression)statement.SqlExpression;
foreach (var item in select.From)
    Console.WriteLine(item.Table switch {
        SqlTableReference table => table.SchemaName + "." + table.TableName,
        SqlDerivedTableReference derived => derived.Alias,
        _ => ""
    });
// dbo.t
// d
```

Parsing needs no catalog; execution needs table t. The renderer preserves each relation's shape.

## Related reference

[Assembly](index.md) · [Design](design.md) · [FROM](../../../../database/sql/language/clauses/from.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlRelationReference.cs` and `SqlTableReference.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDerivedTableParserTests.cs`.
