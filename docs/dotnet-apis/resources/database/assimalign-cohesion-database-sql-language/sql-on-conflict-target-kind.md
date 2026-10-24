# SqlOnConflictTargetKind

Selects how INSERT conflict arbiters are identified.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlOnConflictTargetKind
{
    None,
    Columns,
    Constraint
}
```

## Behavior and exceptions

None selects every unique index, for DO NOTHING only. Columns selects all unique indexes whose
column set equals the inference set, ignoring order and repeated names. Constraint names one unique
index, including a primary-key index or standalone unique index. See [SqlOnConflictClause](sql-on-conflict-clause.md).

The enum adds no exceptions. Execution without a matching unique arbiter, or DO UPDATE with None,
raises DatabaseException/CS03035 (42P10). Untargeted DO NOTHING on a table without unique indexes
inserts normally.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1) ON CONFLICT ON CONSTRAINT uq_email DO NOTHING;");
var clause = ((SqlInsertExpression)statement.SqlExpression).OnConflict!;
Console.WriteLine(clause.TargetKind == SqlOnConflictTargetKind.Constraint); // True
Console.WriteLine(clause.ConstraintName); // uq_email
```

The parser retains the name; the engine verifies it against t's unique indexes during planning.

## Related reference

[Assembly](index.md) · [SqlOnConflictClause](sql-on-conflict-clause.md) · [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlOnConflictTargetKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlInsertParserTests.cs`.
