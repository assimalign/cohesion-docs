# OqlWindowSpecification

Partition/order keys and an optional earlier named window to copy.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlWindowSpecification : OqlExpression
{
    public OqlWindowSpecification(IReadOnlyList<OqlExpression> partitionBy,
        IReadOnlyList<OqlOrdering> orderBy, string? reference = null, Location? location = null);
    public IReadOnlyList<OqlExpression> PartitionBy { get; }
    public IReadOnlyList<OqlOrdering> OrderBy { get; }
    public string? Reference { get; }
}
```

## Behavior and exceptions

Construction takes read-only snapshots. Null lists, members or ordering expressions throw
ArgumentNullException. Reference names are ordinal and case-sensitive. Planning permits only
earlier declarations (CD01042); a copy cannot supply PARTITION BY or replace an existing
ORDER BY (CD01043). Empty lists are valid. Default direction is ASC, with NULLS FIRST for ASC
and NULLS LAST for DESC. See [window equality, ordering and scope](../../../../database/document/language/statements/select.md#windows).

## Example

```csharp
var specification = new OqlWindowSpecification([], []);
Console.WriteLine(specification.Reference is null); // True: OVER ()
```

## Related reference

[OqlWindowExpression](oql-window-expression.md) · [OqlWindowDefinition](oql-window-definition.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlWindowSpecification.cs`.
