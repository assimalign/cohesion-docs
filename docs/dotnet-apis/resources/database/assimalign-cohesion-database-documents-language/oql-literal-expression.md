# OqlLiteralExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlLiteralExpression : OqlExpression
{
    public OqlLiteralExpression(object? value, Location? location = null);
    public object? Value { get; }
}
```

## Behavior and exceptions

Value retains a string, Boolean, null or number. Parser-produced numeric Value is a JsonElement
retaining every digit; planning/evaluation requires an exact decimal and rejects underflow, overflow
or lost nonzero fractional digits with COHDOC007 (22003). Malformed numeric syntax is OQL0004. The
public constructor stores the supplied value; it performs no numeric conversion.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlLiteralExpression(1m);
Console.WriteLine(node.Value); // 1
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/literals.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlLiteralExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlNumericLiteralTests.cs`.
