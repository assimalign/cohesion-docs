# OqlObjectExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlObjectExpression : OqlExpression
{
    public OqlObjectExpression(IReadOnlyList<KeyValuePair<string, OqlExpression>> members, Location? location = null);
    public IReadOnlyList<KeyValuePair<string, OqlExpression>> Members { get; }
}
```

## Behavior and exceptions

Members retain named expressions in order. Evaluation omits MISSING, preserves NULL and keeps the
last included value for a repeated case-sensitive key. A later MISSING does not remove an earlier
value. Invalid evaluated JSON is COHDOC008.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlObjectExpression([
    new KeyValuePair<string, OqlExpression>("a", new OqlLiteralExpression(1m))
]);
Console.WriteLine(node.Members[0].Key); // a
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/object-constructors.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlObjectExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
