# OqlSourceBindingExpression

Base for the closed UNNEST and key JOIN/NEST AST family.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public abstract class OqlSourceBindingExpression : OqlExpression
{
    public OqlExpression Expression { get; }
    public string Variable { get; }
    public bool IsLeft { get; }
}
```

## Behavior and exceptions

The private protected constructor prevents external derivation. Sealed OqlUnnestExpression and
OqlJoinExpression leaves retain the array/key expression, bound variable and preservation flag.
Expression evaluates before introducing Variable, in the preceding binding scope.
OqlSelectExpression.Bindings preserves interleaved execution order. Accessors do not validate or
execute; engine binding reports semantic failures through DatabaseException.Diagnostics.

## Example

Checked against OqlKeyJoinParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT b.* FROM items AS a UNNEST a.keys AS k JOIN docs AS b ON META(b).id = k");
var query = (OqlSelectExpression)statement.OqlExpression;
Console.WriteLine(query.Bindings[0] is OqlUnnestExpression); // True
Console.WriteLine(query.Bindings[1] is OqlJoinExpression);   // True
Console.WriteLine(query.Bindings[1].Variable);              // b
```

## Related reference

[OqlJoinExpression](oql-join-expression.md) · [OqlUnnestExpression](oql-unnest-expression.md) · [OqlSelectExpression](oql-select-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlSourceBindingExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlKeyJoinParserTests.cs`.
