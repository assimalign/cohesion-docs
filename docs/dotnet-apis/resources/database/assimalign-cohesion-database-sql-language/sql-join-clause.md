# SqlJoinClause

Describes one join's kind, added relation, and ON predicate or ordered USING names.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlJoinClause
{
    public SqlJoinType JoinType { get; }
    public SqlRelationReference Table { get; }
    public SqlExpression? Condition { get; }
    public IReadOnlyList<string> UsingColumns { get; }
}
```

## Behavior and exceptions

The parser owns construction. Condition holds ON, or null for USING/CROSS. UsingColumns preserves
written order and is empty for ON/CROSS. SqlJoinType has Inner, LeftOuter, RightOuter, FullOuter
and Cross values; [SupportsJoin](sql-language-profile.md) accepts Inner, LeftOuter, RightOuter and Cross.
INNER/LEFT/RIGHT require exactly one ON or USING. CROSS accepts neither.
Table is now a [SqlRelationReference](sql-relation-reference.md), with named-table and
[derived-query](sql-derived-table-reference.md) leaves, plus
[unaliased grouped joins](sql-joined-table-reference.md).

Malformed USING or CROSS with a condition reports CS00003. Missing INNER/LEFT/RIGHT conditions and
FULL report CK00001. Accessors do not throw validation exceptions. Planning checks USING
names and common types with CS01015 before reading data. See [JOIN](../../../../database/sql/language/clauses/join.md)
for merged column order, common-type coercion, NULL extension and qualified-column behavior.

RIGHT preserves written column order, makes the written left subtree nullable, and reads the
right value/collation for merged USING columns. A direct RIGHT or unaliased grouped source in
UPDATE FROM / DELETE USING fails CS05069 before reads/writes (no SQLSTATE); a derived SELECT
containing RIGHT executes.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT * FROM b LEFT OUTER JOIN c USING (id, name) CROSS JOIN d;");
var select = (SqlSelectExpression)statement.SqlExpression;
var join = select.From[0].Joins[0];
Console.WriteLine(join.Condition is null); // True
Console.WriteLine(string.Join(", ", join.UsingColumns)); // id, name
Console.WriteLine(select.From[0].Joins[1].JoinType); // Cross
```

## Related reference

[Assembly](index.md) · [SqlFromItem](sql-from-item.md) · [SqlSelectExpression](sql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlJoinClause.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlJoinType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`.
