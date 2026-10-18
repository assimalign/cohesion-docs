# GqlNullIfExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlNullIfExpression : GqlExpression
{
    public GqlNullIfExpression(GqlExpression left, GqlExpression right, Location? location = null);
    public GqlExpression Left { get; }
    public GqlExpression Right { get; }
}
```

## Behavior and exceptions

Stores both operands without semantic constructor validation. Both evaluate once. TRUE equality
returns NULL; otherwise Left retains its runtime type. Equality uses exact numeric and structural
three-valued comparison, so UNKNOWN does not replace Left. Missing operands fail planning with
COHDBG001; evaluated value/access errors use COHDBG013. NULLIF is a case abbreviation, outside
the function registry.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (COHDBG016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/nullif.md).

## Example

```csharp
var one = new GqlLiteralExpression(1L);
var node = new GqlNullIfExpression(one, one);
Console.WriteLine(ReferenceEquals(node.Left, node.Right)); // True
// RETURN NULLIF(1, 1.0) returns NULL.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlNullIfExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphValueExpressions.cs`.
