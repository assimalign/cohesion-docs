# GqlCastExpression

An explicit scalar conversion with Cohesion SQL value rules.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlCastExpression : GqlExpression
{
    public GqlCastExpression(GqlExpression operand, GqlCastType target,
        Location? location = null);
    public GqlExpression Operand { get; }
    public GqlCastType Target { get; }
}
```

## Behavior and exceptions

Construction retains Operand/Target without evaluating or additional validation exceptions.
[GqlCastType](gql-cast-type.md) resolves the written type. Planning checks scope and signature
before execution/EXPLAIN: unbound/invalid ASTs use COHDBG001; proven incompatible kinds use
COHDBG003. Invalid dynamic values, text, fractions, overflow and nonzero floating underflow
use COHDBG013. No SQLSTATE is assigned to these codes. NULL converts to NULL.

CAST follows Cohesion SQL's conversion rules. It refuses fractional integer inputs and renders
Boolean strings uppercase. Cypher toInteger truncates; toInteger/toFloat/toBoolean return NULL
for unparsable strings. Temporal results cannot become stored Graph properties.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN CAST('42' AS BIGINT) AS n")).GqlExpression;
var cast = (GqlCastExpression)query.Projections[0].Expression;
Console.WriteLine(cast.Target); // Int64
// Execution returns Int64 42; CAST('2.5' AS BIGINT) instead fails COHDBG013.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Scalar functions](../../../../database/graph/language/functions/scalar-functions.md) · [CAST](../../../../database/graph/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCastExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
