# GqlMemberExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlMemberExpression : GqlExpression
{
    public GqlMemberExpression(GqlExpression operand, string member, Location? location = null);
    public GqlExpression Operand { get; }
    public string Member { get; }
}
```

## Behavior and exceptions

Stores the operand and ordinal member name. Maps, nodes and relationships support lookup;
missing fields/properties or NULL containers return NULL. Map-valued access may chain. Entity
access returns a scalar property and passes wire Execute's entity-result gate. Constructor inputs
are not semantically validated: missing operand/member fails planning with CG05001. A non-map,
non-entity non-NULL container fails evaluation with CG02013. Direct stored-property a.b.c remains
CK00001 in text.

GQL uses NULL without a MISSING state. Constructed values obey depth 64, 65,536 total values
including root and 16 MiB encoded bytes; evaluated limit failures roll back statement writes.
Possible composite wire results require graph.composite-values under protocol 2.1 (CG07016
without it, before reads/writes; no assigned SQLSTATE). See the [language reference](../../../../database/graph/language/expressions/property-access.md).

## Example

```csharp
var map = new GqlMapExpression([
    new KeyValuePair<string, GqlExpression>("k", new GqlLiteralExpression(1L))]);
var node = new GqlMemberExpression(map, "k");
Console.WriteLine(node.Member); // k
// RETURN {k: 1}.k returns Int64 1.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CASE](gql-case-expression.md) · [Maps](gql-map-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlMemberExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphValueExpressions.cs`.
