# OqlWindowDefinition

A query-local WINDOW declaration.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlWindowDefinition : OqlExpression
{
    public OqlWindowDefinition(string name, OqlWindowSpecification specification,
        Location? location = null);
    public string Name { get; }
    public OqlWindowSpecification Specification { get; }
}
```

## Behavior and exceptions

Null name or specification throws ArgumentNullException. Planning rejects empty/duplicate names
with CD01041 and unknown/self/forward references with CD01042. Names are ordinal and case-sensitive;
definitions belong to [OqlSelectExpression.Windows](oql-select-expression.md). Text WINDOW follows
GROUP BY/GROUP AS/HAVING and precedes final ORDER BY/paging. See
[window grammar and quoting](../../../../database/document/language/statements/select.md#windows).

## Example

```csharp
var window = new OqlWindowDefinition("w", new OqlWindowSpecification([], []));
Console.WriteLine(window.Name); // w
```

## Related reference

[OqlWindowExpression](oql-window-expression.md) · [OqlWindowSpecification](oql-window-specification.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlWindowDefinition.cs`.
