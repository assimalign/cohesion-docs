# SqlFromItem

One comma-separated FROM item: a relation followed by its joins in written order.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlFromItem
{
    public SqlRelationReference Table { get; }
    public IReadOnlyList<SqlJoinClause> Joins { get; }
}
```

## Behavior and exceptions

The parser owns construction. [SqlSelectExpression.From](sql-select-expression.md) contains one
item per comma source. Each item's joins associate to the left; ON scope excludes other comma
items and later relations. There is no public constructor or accessor validation exception.
Malformed text produces parser diagnostics; unknown/out-of-scope names fail engine planning with
COHSQLE015, even on empty inputs.
Table is a [SqlRelationReference](sql-relation-reference.md): pattern-match SqlTableReference
for names or [SqlDerivedTableReference](sql-derived-table-reference.md) for an aliased SELECT.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT * FROM a, b LEFT OUTER JOIN c USING (id, name) CROSS JOIN d;");
var select = (SqlSelectExpression)statement.SqlExpression;
Console.WriteLine(select.From.Count); // 2
Console.WriteLine(((SqlTableReference)select.From[0].Table).TableName); // a
Console.WriteLine(select.From[1].Joins.Count); // 2
Console.WriteLine(select.From[1].Joins[0].JoinType); // LeftOuter
```

This parser example requires no tables. Executing it requires the named relations and valid USING
columns. The renderer preserves the comma grouping and ordered USING names.

## Related reference

[Assembly](index.md) · [SqlJoinClause](sql-join-clause.md) · [FROM](../../../../database/sql/language/clauses/from.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlFromItem.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`.
