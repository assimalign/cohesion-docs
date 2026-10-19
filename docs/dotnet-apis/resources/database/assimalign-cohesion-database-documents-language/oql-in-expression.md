# OqlInExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlInExpression : OqlExpression
{
    public OqlInExpression(OqlExpression operand, IReadOnlyList<OqlExpression> values, bool negated = false, Location? location = null);
    public OqlInExpression(OqlExpression operand, OqlExpression array, bool negated = false, Location? location = null);
    public OqlExpression Operand { get; }
    public OqlExpression? Array { get; }
    public IReadOnlyList<OqlExpression> Values { get; }
    public bool Negated { get; }
}
```

## Behavior and exceptions

The list overload retains Values and leaves Array null; an array-valued list item remains one item.
The array overload retains Array and sets Values to an empty list. Array membership propagates
MISSING from either operand; a NULL/non-array right operand gives NULL. NULL IN [] remains NULL;
for a valued operand an empty array gives FALSE. Both overloads retain child references. Operand MISSING/NULL
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

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse("SELECT 2 IN [1, 2] AS hit FROM items");
var select = (OqlSelectExpression)statement.OqlExpression;
var membership = (OqlInExpression)select.Projections[0].Expression;
Console.WriteLine(membership.Array is OqlArrayExpression); // True
Console.WriteLine(membership.Values.Count); // 0
```

With one source document, the query returns {"hit":true}.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/operators.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlInExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
