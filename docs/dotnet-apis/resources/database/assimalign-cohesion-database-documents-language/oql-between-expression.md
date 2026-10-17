# OqlBetweenExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlBetweenExpression : OqlExpression
{
    public OqlBetweenExpression(OqlExpression operand, OqlExpression lower, OqlExpression upper, bool negated = false, Location? location = null);
    public OqlExpression Operand { get; }
    public OqlExpression Lower { get; }
    public OqlExpression Upper { get; }
    public bool Negated { get; }
}
```

## Behavior and exceptions

Operand, Lower and Upper form an inclusive same-kind scalar range. MISSING takes precedence over NULL from any of them. Negated preserves unknown results. Constant path bounds can seek an index. Invalid grouping/aggregate contexts fail COHDOC001.

These AST constructors retain child references and do not execute a query. Parser-produced lists
are read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlBetweenExpression(new OqlLiteralExpression(2m),
    new OqlLiteralExpression(1m), new OqlLiteralExpression(3m));
Console.WriteLine(node.Negated); // False
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/operators.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlBetweenExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
