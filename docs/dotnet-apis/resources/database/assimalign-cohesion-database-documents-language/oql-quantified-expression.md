# OqlQuantifiedExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlQuantifiedExpression : OqlExpression
{
    public OqlQuantifiedExpression(string variable, OqlExpression collection,
        OqlExpression predicate, bool isEvery = false, Location? location = null);
    public string Variable { get; }
    public OqlExpression Collection { get; }
    public OqlExpression Predicate { get; }
    public bool IsEvery { get; }
}
```

## Behavior and exceptions

IsEvery false represents ANY/SOME; true represents EVERY. Collection evaluates in the outer
scope; Predicate sees the inner Variable, which may shadow an outer name. NULL/MISSING predicate
results count as not true. Empty ANY is false and empty EVERY true. MISSING collections propagate
MISSING; NULL/non-array collections yield NULL. See the [truth table](../../../../database/document/language/expressions/quantified-predicates.md).

Construction retains child references and adds no validation exceptions. Planning rejects
META(variable) on this binding with DatabaseException/CD05001, even when the name shadows the
source alias. Collection and Predicate inherit their enclosing clause's aggregate permission and
use its group. An aggregate argument can contain this predicate, but cannot contain another
aggregate inside it (CD05001). Aggregate EVERY is a separate [OqlCallExpression](oql-call-expression.md)
aliasing BOOL_AND. See the [nesting examples](../../../../database/document/language/functions/aggregate-functions.md#examples).
Nested SELECT and multi-binding text forms use CK00001.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT * FROM items d WHERE SOME x IN d.tags SATISFIES x = 1 END");
var predicate = (OqlQuantifiedExpression)((OqlSelectExpression)statement.OqlExpression).Predicate!;
Console.WriteLine(predicate.Variable); // x
Console.WriteLine(predicate.IsEvery);  // False
```

A document with tags [1, null] passes; tags [] or [null] does not.

## Related reference

[Assembly](index.md) · [OqlUnnestExpression](oql-unnest-expression.md) · [OqlInExpression](oql-in-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQuantifiedExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlArrayParserTests.cs`.
