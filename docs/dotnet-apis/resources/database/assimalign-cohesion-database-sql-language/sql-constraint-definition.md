# SqlConstraintDefinition

Retains a parsed table or column constraint and its parent-delete action.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlReferentialAction { Restrict, Cascade, NoAction, SetNull, SetDefault }
public sealed class SqlConstraintDefinition
{
    public string? Name { get; }
    public SqlConstraintKind Kind { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlTableReference? ReferencedTable { get; }
    public IReadOnlyList<string> ReferencedColumns { get; }
    public SqlReferentialAction OnDelete { get; }
    public SqlExpression? CheckExpression { get; }
    public string? CheckExpressionText { get; }
}
```

## Behavior and exceptions

The parser constructs this read-only type. Foreign-key lists retain matching column order;
OnDelete is NoAction when omitted in SQL. SetNull assigns NULL to referencing columns and
SetDefault requests their defaults. See [constraints](../../../../database/sql/language/constraints.md)
for cascade chains, final-state checks and atomicity. Property access adds no exceptions.
Parser.Parse returns diagnostics; execution rejects invalid text with DatabaseParseException.
ON UPDATE and deferred constraints remain COHDBL001.

## Example

```csharp
var parsed = (SqlCreateTableExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE TABLE c (pid INT, FOREIGN KEY (pid) REFERENCES p(id) ON DELETE SET NULL)")).SqlExpression;
Console.WriteLine(parsed.Constraints[0].OnDelete); // SetNull
```

Omitting ON DELETE produces NoAction. This differs from the optional constructor argument of
[SqlCatalogConstraint](../assimalign-cohesion-database-sql-catalog/sql-catalog-constraint.md).

## Related reference

[Assembly](index.md) · [SqlColumnDefinition](sql-column-definition.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlConstraintDefinition.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlExpressionDefaultParserTests.cs`.
