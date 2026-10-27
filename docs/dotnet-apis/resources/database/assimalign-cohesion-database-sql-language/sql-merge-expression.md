# SqlMergeExpression

The parsed MERGE target, USING source, ON predicate, ordered actions and optional RETURNING list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlMergeExpression : SqlQueryExpression
{
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
the plan; executing it is refused with CK00001 in this release step. See
[MERGE](../../../../database/sql/language/statements/merge.md) for scopes, diagnostics and association rules.

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

## Related reference

[Assembly](index.md) · [SqlMergeWhenClause](sql-merge-when-clause.md) · [SqlFromItem](sql-from-item.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlMergeExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlMergeParserTests.cs`.
