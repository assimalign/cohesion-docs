# GqlPatternComprehensionExpression

A correlated path pattern with an optional match filter and required collected projection.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlPatternComprehensionExpression : GqlExpression
{
    public GqlPatternComprehensionExpression(GqlPathPattern pattern,
        GqlExpression? predicate, GqlExpression projection, Location? location = null);
    public GqlPathPattern Pattern { get; }
    public GqlExpression? Predicate { get; }
    public GqlExpression Projection { get; }
    public override string ToString();
}
```

## Behavior and exceptions

Construction retains operands and location without validation or execution. Pattern must contain
a relationship chain, including one quantified to zero hops. Pattern.Variable introduces a local
path name that can shadow an outer name; other existing pattern names correlate. Predicate keeps
TRUE matches only. Projection collects every retained match in order into an object?[]; duplicates
and NULL values remain. NULL correlated entities and no matches produce an empty array.
ToString renders equivalent GQL syntax. See [pattern comprehensions](../../../../database/graph/language/expressions/pattern-comprehensions.md).

Engine validation raises DatabaseException/CG05028 for malformed ASTs or an aggregate projection;
missing execution context or collection plan uses the same code. Unbound names use CG05001,
proven incompatible correlated kinds CG01019, runtime kinds CG02016, and non-Boolean predicates
CG02013. Text malformed shapes use CG00014 or CG00002; excluded path prefixes use CK00001.
These codes have no SQLSTATE. Inspect parser diagnostics before planning.

In-process lists may contain entities. Final entity-valued wire lists use CG05001 until #1662,
even when empty. Scalar composite results require graph.composite-values (CG07016 without it).
Collection shares statement work and memory; exhaustion uses CG05004 and CG05020.

## Example

Adapted from GqlPatternComprehensionParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN [p=(a)-[r:T]->(b) WHERE b.k > 1 | p]");
var expression = (GqlPatternComprehensionExpression)
    statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(expression.Pattern.Variable);       // p
Console.WriteLine(expression.Predicate is not null);  // True
Console.WriteLine(expression.Projection is not null); // True
```

This builds a path-valued list expression; executing its final result requires the in-process API.

## Related reference

[Assembly](index.md) · [GqlPathPattern](gql-path-pattern.md) · [GqlListComprehensionExpression](gql-list-comprehension-expression.md) · [GqlExistsExpression](gql-exists-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPatternComprehensionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlPatternComprehensionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphPatternComprehensionTests.cs`.
