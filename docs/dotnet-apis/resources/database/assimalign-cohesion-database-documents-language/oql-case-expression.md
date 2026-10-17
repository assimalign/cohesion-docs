# OqlCaseExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCaseExpression : OqlExpression
{
    public OqlCaseExpression(OqlExpression? operand, IReadOnlyList<KeyValuePair<OqlExpression, OqlExpression>> branches, OqlExpression? otherwise = null, Location? location = null);
    public OqlExpression? Operand { get; }
    public IReadOnlyList<KeyValuePair<OqlExpression, OqlExpression>> Branches { get; }
    public OqlExpression? Otherwise { get; }
}
```

## Behavior and exceptions

Null Operand means searched CASE with SQL++ truth conversion; otherwise it means simple CASE with structural equality. Branches are ordered WHEN keys and THEN values. Otherwise null means implicit NULL. Unknown conditions fall through; a selected MISSING result stays MISSING. Only the selected result evaluates, but all branches are validated for binding, numeric and aggregate errors.

These AST constructors retain child references and do not execute a query. Parser-produced lists
are read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlCaseExpression(null, [
    new KeyValuePair<OqlExpression, OqlExpression>(
        new OqlLiteralExpression(true), new OqlLiteralExpression("yes"))
]);
Console.WriteLine(node.Otherwise is null); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/case.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCaseExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
