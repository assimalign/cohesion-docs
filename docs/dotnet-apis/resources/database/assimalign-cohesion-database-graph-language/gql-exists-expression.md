# GqlExistsExpression

A Boolean existence test over a correlated graph pattern with an isolated lexical scope.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlExistsExpression : GqlExpression
{
    public GqlExistsExpression(IReadOnlyList<GqlMatchClause> matchClauses,
        Location? location = null);
    public IReadOnlyList<GqlMatchClause> MatchClauses { get; }
    public override string ToString();
}
```

## Behavior and exceptions

Construction retains MatchClauses without validating it. Planning requires a nonempty list of
MATCH clauses with nonempty paths; invalid shapes report DatabaseException/CG05027. ToString
renders canonical explicit MATCH keywords even when the input used an implicit pattern list.
Location covers the complete EXISTS expression. Scalar NOT supplies NOT EXISTS.

Outer bindings become arguments; new bindings remain local. NULL pattern arguments match
nothing, so EXISTS always returns Boolean. Each occurrence of a shared AST binds independently
in its lexical scope. A proven incompatible correlated entity kind is CG01019; a runtime kind
mismatch is CG02016. Missing execution context/subplan/argument uses CG05027. Empty/unbraced
text uses CG00013; excluded query-body forms use CK00001. None has an assigned SQLSTATE.

## Example

Adapted from the round-trip cases in GqlExistsParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN EXISTS { (a)-[:T]->(b) }");
var exists = (GqlExistsExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(exists.MatchClauses.Count); // 1
var roundTrip = new GqlQueryParser().Parse("RETURN " + exists.ToString());
Console.WriteLine(roundTrip.Diagnostics.Count); // 0
```

Inspect statement.Diagnostics before execution. This uncorrelated example returns TRUE if a T
relationship exists. [The language reference](../../../../database/graph/language/expressions/exists.md)
includes a correlated recommendation query and placement rules.

## Related reference

[Assembly](index.md) · [GqlClauses](gql-clauses.md) · [GqlQueryExpression](gql-query-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlExistsExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlExistsParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphExistsTests.cs`.
