# OqlAggregateQuantifier

Controls duplicate inputs to one OQL aggregate call.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public enum OqlAggregateQuantifier
{
    None,
    All,
    Distinct
}
```

## Behavior and exceptions

None (0) is the omitted modifier; All (1) explicitly includes every admitted input. Distinct (2)
includes each admitted value once using grouping equality. All eleven native aggregate names
and single-argument registered aggregates accept these modifiers. Registered functions retain
their NULL/MISSING policy. COUNT(*) must use None.

The enum has no guards. Binding a directly constructed [OqlCallExpression](oql-call-expression.md)
with an undefined value, scalar quantifier or quantified star fails with DatabaseException/CD01029.
Text parsing uses CD00011 for scalar/star modifiers and CD00006 for invalid arity. Neither
CD01029 nor CD00011 has an assigned SQLSTATE.

## Example

Adapted from OqlAggregateParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT COUNT(DISTINCT x) FROM c");
var call = (OqlCallExpression)((OqlSelectExpression)statement.OqlExpression)
    .Projections[0].Expression;
Console.WriteLine(call.Quantifier); // Distinct
```

## Related reference

[Assembly](index.md) · [OqlCallExpression](oql-call-expression.md) · [Aggregate functions](../../../../database/document/language/functions/aggregate-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlAggregateQuantifier.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlAggregateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Aggregates.cs`.
