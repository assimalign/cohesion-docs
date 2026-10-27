# SqlJoinedTableReference

An unaliased parenthesized join retaining its component namespaces and association.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlJoinedTableReference : SqlRelationReference
{
    public SqlFromItem Item { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain the node from SqlQueryParser. Item retains the grouped relation,
joins and ON scopes. Its inherited Alias is null. Aliased groups instead lower to an aliased
SELECT * derived relation, hiding the component names and accepting a complete column-alias list.
Groups may start with a derived query and nest. Canonical rendering, CREATE VIEW and reopen
preserve the association. Getters add no validation exceptions.

Direct grouped sources in UPDATE FROM / DELETE USING fail with CS05069 before reads/writes
(no SQLSTATE). A derived SELECT containing RIGHT or FULL executes. FULL groups retain their
written children and common-typed USING COALESCE keys; unsupported FULL conditions use CS05070.
See [JOIN](../../../../database/sql/language/clauses/join.md) for column order, USING and NULL rules.

## Example

From `SqlRightJoinParserTests.Parse_RightGroup_ShouldRetainComponentNamespaces`; parsing needs no catalog:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT * FROM a LEFT JOIN (b RIGHT JOIN c USING (id)) ON a.id = c.id");
var select = (SqlSelectExpression)statement.SqlExpression;
var group = (SqlJoinedTableReference)select.From[0].Joins[0].Table;
Console.WriteLine(((SqlTableReference)group.Item.Table).TableName); // b
Console.WriteLine(group.Item.Joins[0].JoinType);                    // RightOuter
```

## Related reference

[Assembly](index.md) · [SqlFromItem](sql-from-item.md) · [SqlRelationReference](sql-relation-reference.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlJoinedTableReference.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlRightJoinParserTests.cs`.
