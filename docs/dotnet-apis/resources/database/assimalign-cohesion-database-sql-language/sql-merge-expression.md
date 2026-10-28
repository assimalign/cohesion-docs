# SqlMergeExpression

The parsed MERGE target, USING source, ON predicate, ordered actions and optional RETURNING list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlMergeExpression : SqlQueryExpression
{
    public IReadOnlyList<SqlCommonTableExpression> CommonTables { get; }
    public SqlTableReference Table { get; }
    public SqlFromItem Source { get; }
    public SqlExpression Condition { get; }
    public IReadOnlyList<SqlMergeWhenClause> Actions { get; }
    public IReadOnlyList<SqlSelectColumn>? Returning { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain the node through SqlQueryParser. CommandType is Merge. Actions
retain written order; Returning is null when absent. Source uses the existing read-relation and
join shapes. Parsing does not validate catalog names or action scopes. Malformed action syntax
produces parser diagnostics. Binding validates scopes, destination types and clause reachability;
CS05071 marks a clause after an unconditional clause of its match kind. Plain EXPLAIN describes
the plan without writes. Execution chooses the first qualifying action per candidate and applies
all actions atomically, including RETURNING preparation. A second modifying action on one physical
target uses CS05036 (21000); retained-state exhaustion uses CS05067 (53200) before writes. See
[MERGE](../../../../database/sql/language/statements/merge.md) for scopes, diagnostics and association rules.

CommonTables retains plain WITH definitions in declaration order, or an empty list when absent;
the setter is internal. Every definition binds and reachable definitions materialize once before
writes. WITH RECURSIVE ... MERGE is refused by parsing with CK00001, even for acyclic lists.

Execution locks and rechecks the latest concurrently committed modifying target under ReadCommitted,
reevaluating ON and ordered WHEN clauses with the original source. Snapshot target conflicts use
retryable CS04064 (40001). Mixed-action foreign keys see final rows; direct/referential overlaps
compose without CS05036. Statement-level uniqueness accepts key swaps rejected by PostgreSQL's
immediate checks and reuses keys freed by the statement's deletes. RETURNING describes direct
actions before referential effects.

## Example

Using the syntax covered by SqlMergeParserTests; parsing needs no catalog:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "MERGE INTO t USING s ON t.id=s.id WHEN MATCHED THEN DELETE RETURNING merge_action()");
var merge = (SqlMergeExpression)statement.SqlExpression;
Console.WriteLine(merge.Actions.Count);   // 1
Console.WriteLine(merge.Actions[0].Action); // Delete
Console.WriteLine(merge.Returning!.Count); // 1
```

With an active SqlDatabaseSession in a fresh database (adapted from SqlMergeExecutionTests):

```csharp
await session.ExecuteAsync("CREATE TABLE t (id INT PRIMARY KEY,v INT)");
await session.ExecuteAsync("CREATE TABLE s (id INT,v INT)");
await session.ExecuteAsync("INSERT INTO t VALUES(1,10)");
await session.ExecuteAsync("INSERT INTO s VALUES(1,11),(2,22)");
await using var result = (DatabaseResultSet)await session.ExecuteAsync(
    "MERGE INTO t USING s ON t.id=s.id WHEN MATCHED THEN UPDATE SET v=s.v " +
    "WHEN NOT MATCHED THEN INSERT VALUES(s.id,s.v) RETURNING t.id,t.v,merge_action()");
Console.WriteLine(result.AffectedCount); // 2
await foreach (var row in result.GetRowsAsync(CancellationToken.None))
    Console.WriteLine($"{row.GetValue(0)},{row.GetValue(1)},{row.GetValue(2)}");
```

The two rows, in unspecified order, are `1,11,UPDATE` and `2,22,INSERT`.
DatabaseResultSet is in `Assimalign.Cohesion.Database.Execution`.

## Related reference

[Assembly](index.md) · [SqlMergeWhenClause](sql-merge-when-clause.md) · [SqlFromItem](sql-from-item.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlMergeExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlMergeParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeExecutionTests.cs`.
