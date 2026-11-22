# SqlQualifiedStarExpression

A relation's qualified wildcard in a SELECT list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlQualifiedStarExpression : SqlExpression
{
    public string TableName { get; }
    public string? SchemaName { get; }
}
```

## Behavior

The parser owns construction. TableName is the relation name or alias in t.*; SchemaName is
the optional schema in schema.table.*. Location is inherited from SqlExpression. Binding expands
the local input's own columns, including USING-hidden physical columns, preserving outer-join
nullability. Aliases hide base/schema names. An alias written on a star is discarded.

Unknown or ambiguous qualifiers use CS01015. Grouped stars retain CS05017; row-valued and qualified
RETURNING stars retain CK00001. Expansion exceeding 1,664 targets uses CS05097 / 54011. Accessors
add no validation exceptions. See [qualified wildcards](../../../../database/sql/language/clauses/join.md#qualified-wildcards).

## Related reference

[Assembly](index.md) · [SqlSelectExpression](sql-select-expression.md) · [SqlJoinClause](sql-join-clause.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlQualifiedStarExpression.cs`.
