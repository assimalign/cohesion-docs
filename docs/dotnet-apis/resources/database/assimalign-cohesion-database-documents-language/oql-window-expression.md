# OqlWindowExpression

A ranking/navigation call with an inline/copied specification or direct window name.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlWindowExpression : OqlExpression
{
    public OqlWindowExpression(OqlCallExpression call,
        OqlWindowSpecification? specification = null, string? name = null,
        Location? location = null);
    public OqlCallExpression Call { get; }
    public OqlWindowSpecification? Specification { get; }
    public string? Name { get; }
}
```

## Behavior and exceptions

Null call throws ArgumentNullException. Construction retains children. Planning requires
exactly one of Name or Specification (CD01043). Calls occur only in SELECT projections and
SELECT ORDER BY at their query level (CD01044); nested windows use CD01045. Frames, aggregate
OVER and value windows are deferred. See [defaults and name resolution](../../../../database/document/language/statements/select.md#windows).

## Example

```csharp
var parsed = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT ROW_NUMBER() OVER () AS n FROM items");
var select = (OqlSelectExpression)parsed.OqlExpression;
Console.WriteLine(select.Windows.Count); // 0: the specification is inline
```

## Related reference

[Specification](oql-window-specification.md) · [Definition](oql-window-definition.md) · [Function kind](oql-window-function-kind.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlWindowExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlWindowParserTests.cs`.
