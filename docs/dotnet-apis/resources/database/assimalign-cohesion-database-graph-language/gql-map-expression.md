# GqlMapExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlMapExpression : GqlExpression
{
    public GqlMapExpression(IEnumerable<KeyValuePair<string, GqlExpression>> entries,
        Location? location = null);
    public IReadOnlyList<KeyValuePair<string, GqlExpression>> Entries { get; }
}
```

## Behavior and exceptions

Copies entries in source order. Evaluation creates a read-only ordinal string dictionary and
retains nested values and NULLs. Constructor acceptance is not semantic validation. A null entry list,
null keys, duplicate ordinal keys or missing value expressions fail planning with COHDBG001.
Text duplicate keys use GQL0006. Evaluated bounds/errors use COHDBG013; stored properties remain
scalar-only (COHDBG003 for composite parameters, COHDBL001 for composite syntax).

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (COHDBG016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/map-values.md).

## Example

```csharp
var one = new GqlLiteralExpression(1L);
var node = new GqlMapExpression([
    new KeyValuePair<string, GqlExpression>("k", new GqlListExpression([one]))]);
Console.WriteLine(node.Entries[0].Key); // k
// RETURN {k: [1]} returns one read-only map cell.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlMapExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphValueExpressions.cs`.
