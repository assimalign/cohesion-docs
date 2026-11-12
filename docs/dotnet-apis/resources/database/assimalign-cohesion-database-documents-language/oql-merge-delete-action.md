# OqlMergeDeleteAction

The matched MERGE DELETE action and its optional WHERE predicate.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed record OqlMergeDeleteAction
{
    public OqlMergeDeleteAction(OqlExpression? predicate = null);
    public OqlExpression? Predicate { get; }
}
```

## Behavior and exceptions

Construction stores the optional predicate without executing or validating it. Predicate=null
means no action filter. A supplied filter sees both matched target and source; EXPLAIN retains
it as a residual after key matching. This has the same Predicate shape as UPDATE and INSERT
actions. MERGE execution is not available yet and refuses CD05022. See
[MERGE planning](../../../../database/document/language/statements/index.md#merge-planning).

## Example

```csharp
var delete = new OqlMergeDeleteAction();
Console.WriteLine(delete.Predicate is null); // True
```

## Related reference

[OqlMergeExpression](oql-merge-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMergeDeleteAction.cs`.
