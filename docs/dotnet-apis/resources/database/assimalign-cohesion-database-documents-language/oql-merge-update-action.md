# OqlMergeUpdateAction

The matched MERGE UPDATE action, with ordinary SET/UNSET paths and an optional filter.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed record OqlMergeUpdateAction
{
    public OqlMergeUpdateAction(IReadOnlyList<OqlUpdateAssignment> assignments,
        IReadOnlyList<OqlPathExpression> unset, OqlExpression? predicate = null);
    public IReadOnlyList<OqlUpdateAssignment> Assignments { get; }
    public IReadOnlyList<OqlPathExpression> Unset { get; }
    public OqlExpression? Predicate { get; }
}
```

## Behavior and exceptions

The constructor copies mutation lists into read-only snapshots. Null lists or elements throw
ArgumentNullException. Planning validates nonempty mutations and target-relative destinations;
values and Predicate see both source and target. Existing CD02016/CD01017 path rules apply.
Predicate is the action WHERE residual after matching, not a target access-path predicate.
MERGE execution remains CD05022; see [the planning contract](../../../../database/document/language/statements/index.md#merge-planning).

## Example

```csharp
var parsed = (OqlQueryStatement)new OqlQueryParser().Parse(
    "MERGE INTO targets t USING sources s ON KEY s.key WHEN MATCHED THEN UPDATE SET t.n=s.n");
var update = ((OqlMergeExpression)parsed.OqlExpression).Update!;
Console.WriteLine(update.Assignments.Count); // 1
Console.WriteLine(update.Predicate is null); // True
```

## Related reference

[OqlMergeExpression](oql-merge-expression.md) · [OqlUpdateAssignment](oql-update-assignment.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMergeUpdateAction.cs`.
