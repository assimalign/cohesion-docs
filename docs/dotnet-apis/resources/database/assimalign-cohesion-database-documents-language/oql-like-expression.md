# OqlLikeExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlLikeExpression : OqlExpression
{
    public OqlLikeExpression(OqlExpression operand, OqlExpression pattern, OqlExpression? escape = null, bool negated = false, Location? location = null);
    public OqlExpression Operand { get; }
    public OqlExpression Pattern { get; }
    public OqlExpression? Escape { get; }
    public bool Negated { get; }
}
```

## Behavior and exceptions

Operand and Pattern use ordinal Unicode-scalar matching. Null Escape means default backslash; an
empty evaluated string disables escaping. MISSING precedes non-string/NULL results. Invalid
evaluated escapes or trailing pattern escapes fail COHDOC013; a malformed literal escape is OQL0002.
Evaluation honors cancellation with OperationCanceledException.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlLikeExpression(new OqlLiteralExpression("beta"), new OqlLiteralExpression("b%"));
Console.WriteLine(node.Escape is null); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/expressions/operators.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlLikeExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlPredicateParserTests.cs`.
