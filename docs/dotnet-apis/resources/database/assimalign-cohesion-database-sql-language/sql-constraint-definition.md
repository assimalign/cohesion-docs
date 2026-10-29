# SqlConstraintDefinition

Retains a parsed table or column constraint and its parent-delete and parent-update actions.

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
    public SqlReferentialAction OnUpdate { get; }
    public SqlExpression? CheckExpression { get; }
    public string? CheckExpressionText { get; }
}
```

## Behavior and exceptions

The parser constructs this read-only type. Foreign-key lists retain matching column order;
OnDelete and OnUpdate are independently NoAction when omitted in SQL. SetNull assigns NULL to referencing columns and
SetDefault requests their defaults. See [constraints](../../../../database/sql/language/constraints.md)
for cascade chains, final-state checks and atomicity. Property access adds no exceptions.
Parser.Parse returns diagnostics; execution rejects invalid text with DatabaseParseException.
Deferred constraints remain CK00001. ON UPDATE declarations accept all five actions in either
order beside ON DELETE. Duplicate/incomplete actions use CS00003. Changing referenced keys
under Cascade/SetNull/SetDefault retains CK00001 until action execution is implemented.

## Example

```csharp
var parsed = (SqlCreateTableExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE TABLE c (pid INT, FOREIGN KEY (pid) REFERENCES p(id) ON DELETE SET NULL ON UPDATE CASCADE)")).SqlExpression;
Console.WriteLine(parsed.Constraints[0].OnDelete); // SetNull
Console.WriteLine(parsed.Constraints[0].OnUpdate); // Cascade
```

Omitting ON DELETE produces NoAction. This differs from the optional constructor argument of
[SqlCatalogConstraint](../assimalign-cohesion-database-sql/sql-catalog-constraint.md).

## Related reference

[Assembly](index.md) · [SqlColumnDefinition](sql-column-definition.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlConstraintDefinition.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlExpressionDefaultParserTests.cs`.
