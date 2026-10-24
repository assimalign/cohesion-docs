# OqlUpdateAssignment

One SET target and its value expression in an [OqlUpdateExpression](oql-update-expression.md).

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlUpdateAssignment : OqlExpression
{
    public OqlUpdateAssignment(OqlPathExpression path, OqlExpression value, Location? location = null);
    public OqlPathExpression Path { get; }
    public OqlExpression Value { get; }
}
```

## Behavior and exceptions

The constructor retains Path, Value and the optional assignment span without validation. The
planner normalizes the optional alias and rejects invalid targets (CD02016) and duplicate or
prefix-overlapping targets (CD01017), including overlap with UNSET. Both codes are carried by
DatabaseException and have no assigned SQLSTATE. Value reads the pre-update document; assignments
do not observe earlier assignments. SET-to-MISSING removes a named member or writes null to an
existing array element; SET-to-NULL writes JSON null. Missing arrays and out-of-range indices fail
with CD02016. Invalid patched JSON or storage depth uses CD02018, also without SQLSTATE.

## Example

This parser case retains two assignments and their complete source spans:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "UPDATE items AS d SET d.details.score = 5, d.tags[0] = 'red' " +
    "UNSET d.category WHERE d.amount > 3");
var update = (OqlUpdateExpression)statement.OqlExpression;
OqlUpdateAssignment assignment = update.Assignments[1];
Console.WriteLine(assignment.Path.Segments[^1].Index); // 0
Console.WriteLine(assignment.Value is OqlLiteralExpression); // True
```

## Related reference

[Assembly](index.md) · [UPDATE path rules](../../../../database/document/language/statements/update.md#set-and-unset-paths)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlUpdateAssignment.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
