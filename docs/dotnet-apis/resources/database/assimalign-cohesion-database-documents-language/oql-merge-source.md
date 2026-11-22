# OqlMergeSource

Retains a MERGE collection, subquery or expression source and its alias.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed record OqlMergeSource
{
    public OqlMergeSource(string collection, string? alias, OqlExpression? expression = null);
    public string Collection { get; }
    public string? Alias { get; }
    public OqlExpression? Expression { get; }
}
```

## Behavior and exceptions

Null collection throws ArgumentNullException. Expression=null selects the collection; expression
sources use an empty collection name. Collections/paths can supply implicit names, while a
parenthesized query requires an alias. Generic expression aliases require AS in text. Construction
retains children without executing. Direct invalid source shape uses CD01039. See
[MERGE sources and scopes](../../../../database/document/language/statements/index.md#merge-planning).

## Example

```csharp
var source = new OqlMergeSource("sources", "s");
Console.WriteLine(source.Expression is null); // True
```

## Related reference

[OqlMergeExpression](oql-merge-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMergeSource.cs`.
