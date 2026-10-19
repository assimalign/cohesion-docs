# SqlOnConflictAction

Selects the action taken for an INSERT arbiter conflict.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlOnConflictAction
{
    DoNothing,
    DoUpdate
}
```

## Behavior and exceptions

DoNothing skips the proposal. DoUpdate applies [SqlOnConflictClause.Assignments](sql-on-conflict-clause.md)
when its optional Where evaluates TRUE. FALSE/UNKNOWN skips it while retaining its row lock.
Only inserted and updated rows contribute RETURNING rows or affected counts.

The enum adds no exceptions. Executing DoUpdate without a target uses COHSQLE035 (42P10);
affecting a row twice uses COHSQLE036 (21000). Both actions can abort retryably on Snapshot
conflicts; [transaction retry rules](../../../../database/sql/language/clauses/on-conflict.md#isolation-locking-and-retries)
apply to the complete transaction.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t VALUES (1) ON CONFLICT DO NOTHING;");
var clause = ((SqlInsertExpression)statement.SqlExpression).OnConflict!;
Console.WriteLine(clause.Action == SqlOnConflictAction.DoNothing); // True
Console.WriteLine(clause.Assignments.Count); // 0
```

With a pre-existing unique key 1, execution skips the row and reports count 0.

## Related reference

[Assembly](index.md) · [SqlOnConflictClause](sql-on-conflict-clause.md) · [SqlOnConflictTargetKind](sql-on-conflict-target-kind.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlOnConflictClause.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlInsertParserTests.cs`.
