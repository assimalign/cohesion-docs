# OqlInExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlInExpression : OqlExpression
{
    public OqlInExpression(OqlExpression operand, IReadOnlyList<OqlExpression> values, bool negated = false, Location? location = null);
    public OqlExpression Operand { get; }
    public IReadOnlyList<OqlExpression> Values { get; }
    public bool Negated { get; }
}
```

## Behavior and exceptions

Values are a nonempty explicit list; an array-valued item remains one item. Operand MISSING/NULL
retains its state. For a present operand, a structural match wins, then NULL, then MISSING, then
FALSE. Negated reverses only Boolean results. Invalid context/list shapes are rejected while
planning with COHDOC001.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlInExpression(new OqlLiteralExpression(1m), [new OqlLiteralExpression(1m)]);
Console.WriteLine(node.Negated); // False
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/operators.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlInExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
