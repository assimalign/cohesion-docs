# OqlMissingExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlMissingExpression : OqlExpression
{
    public OqlMissingExpression(Location? location = null);

}
```

## Behavior and exceptions

Represents the SQL++ MISSING literal as a distinct node rather than a null literal. Optional
Location retains its span. The constructor accepts an absent location without additional validation
exceptions; evaluation produces an absent value omitted from projected objects.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlMissingExpression();
Console.WriteLine(node.Location is null); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/missing.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMissingExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlSqlppBasisTests.cs`.
