# GqlSubscriptExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlSubscriptExpression : GqlExpression
{
    public GqlSubscriptExpression(GqlExpression operand, GqlExpression index, Location? location = null);
    public GqlExpression Operand { get; }
    public GqlExpression Index { get; }
}
```

## Behavior and exceptions

Stores the container and index. Lists require integral runtime indexes, with negative indexes
counting from the end. Maps/nodes/relationships accept string keys. Out-of-range indexes, absent
fields and NULL containers/indexes return NULL. Constructor inputs are retained without semantic
validation. Missing expressions fail planning with COHDBG001; invalid evaluated container/index
types use COHDBG013. Slicing is not represented and remains COHDBL001 in text.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (COHDBG016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/property-access.md).

## Example

```csharp
var node = new GqlSubscriptExpression(
    new GqlListExpression([new GqlLiteralExpression(1L)]), new GqlLiteralExpression(-1L));
// RETURN [1][-1] returns Int64 1.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSubscriptExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphValueExpressions.cs`.
