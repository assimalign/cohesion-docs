# SqlColumnDefinition

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlColumnDefinition
{
    public string ColumnName { get; }
    public string DataType { get; }
    public DatabaseTypeInfo? ResolvedType { get; }
    public bool IsNullable { get; }
    public bool IsPrimaryKey { get; }
    public SqlExpression? DefaultValue { get; }
    public SqlIdentityDefinition? Identity { get; }
    public IReadOnlyList<SqlConstraintDefinition> Constraints { get; }
    public string? CollationName { get; }
}
```

## Behavior and exceptions

ResolvedType holds the built-in DatabaseTypeInfo resolved during parsing, including contextual
multi-word type names and modifiers. It is null for engine-resolved domains or invalid types, so
inspect diagnostics before use. DataType retains the parsed declaration; DefaultValue may contain
typed literal nodes. No public constructor is exposed. Invalid modifiers use SQL0005; an unresolved
engine type uses COHSQLE010 (42704).
Identity holds the [generation facet](sql-identity-definition.md), or null for an ordinary column.
It implies IsNullable false. Explicit NULL with NOT NULL, PRIMARY KEY or IDENTITY in either order
is SQL0003, including table primary keys. Engine identity definition/assignment failures use
COHSQLE025; inspect diagnostics before consuming a recovered node.

Parser.Parse returns diagnostics rather than throwing for malformed text; engine execution or
SqlQueryRequest.FromSql rejects erroneous statements with DatabaseParseException.

DefaultValue also retains column-free arithmetic, CAST and Stable/Volatile call nodes.
[DEFAULT](../../../../database/sql/language/clauses/default.md) describes declaration folding,
per-row evaluation and ADD COLUMN freezing; [SqlConstraintDefinition](sql-constraint-definition.md)
exposes all five ON DELETE actions.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("CREATE TABLE t (v DOUBLE PRECISION)");
var table = (SqlCreateTableExpression)statement.SqlExpression;
Console.WriteLine(table.Columns[0].ResolvedType!.Type); // Float64
```

## Related reference

[Assembly](index.md) · [SQL language](../../../../database/sql/language/index.md) · [SqlLiteralText](sql-literal-text.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlColumnDefinition.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTypeAliasTests.cs`.
