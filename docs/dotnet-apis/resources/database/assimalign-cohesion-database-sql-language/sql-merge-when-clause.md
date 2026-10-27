# SqlMergeWhenClause

One ordered MERGE WHEN clause with its match kind, optional condition and operation.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlMergeWhenClause
{
    public SqlMergeMatchKind MatchKind { get; }
    public SqlExpression? Condition { get; }
    public SqlMergeAction Action { get; }
    public IReadOnlyList<SqlAssignment> Assignments { get; }
    public IReadOnlyList<string>? Columns { get; }
    public IReadOnlyList<SqlExpression>? Values { get; }
    public bool IsDefaultValues { get; }
    public bool IsByTargetExplicit { get; }
    public Location Location { get; }
}

public enum SqlMergeMatchKind { Matched, NotMatchedByTarget, NotMatchedBySource }
public enum SqlMergeAction { Update, Delete, Insert, DoNothing }
```

Location is `Assimalign.Cohesion.Database.Language.Location`.

## Behavior and exceptions

Construction is internal. Condition is null without AND; Assignments is empty for non-UPDATE
operations. Columns is null when INSERT uses table column order; Values is null for non-value-row
forms. IsDefaultValues identifies INSERT DEFAULT VALUES. IsByTargetExplicit retains the optional
BY TARGET spelling. Location spans WHEN through the last action token, excluding the next clause
and trailing comments; CS05071 uses this span for unreachable actions. Getters do not execute or
validate the action. [MERGE](../../../../database/sql/language/statements/merge.md) binding checks
scope, action kind, destinations, widths and assignment families. Execution remains CK00001.

## Example

For the [SqlMergeExpression example](sql-merge-expression.md#example):

```csharp
var clause = merge.Actions[0];
Console.WriteLine(clause.MatchKind);          // Matched
Console.WriteLine(clause.Condition is null);  // True
Console.WriteLine(clause.Assignments.Count);  // 0
Console.WriteLine(clause.IsByTargetExplicit); // False
```

## Related reference

[Assembly](index.md) · [SqlMergeExpression](sql-merge-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlMergeWhenClause.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlMergeBindingTests.cs`.
