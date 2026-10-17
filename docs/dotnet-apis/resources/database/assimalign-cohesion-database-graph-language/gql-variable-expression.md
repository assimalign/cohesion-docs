# GqlVariableExpression

A reference to a bound node, relationship or path in a value expression.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlVariableExpression : GqlExpression
{
    public GqlVariableExpression(string variable, Location? location = null);
    public string Variable { get; }
}
```

## Behavior and exceptions

Variable is a case-sensitive binding name. The constructor stores it and the optional source span
without validation exceptions. The planner rejects unbound references with COHDBG001 before data
access. Property access uses GqlPropertyExpression instead; a path has no properties (COHDBG003).
Bare variables permit identity equality and IS NULL tests in WHERE or RETURN. Node/relationship
identity and ordered path identities compare independently of object materialization.

An embedded [GqlProjection](gql-projection.md) can return the bound object in a DatabaseRow.
Wire Execute refuses entity kinds with COHDBG001 before reads or writes; ExecutePaths requires
exactly one bare binding from read-only MATCH. Star over one named binding expands to that form.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse("MATCH (a) RETURN DISTINCT a");
var projection = statement.GqlExpression.Projections[0];
var variable = (GqlVariableExpression)projection.Expression;
Console.WriteLine(variable.Variable); // a
var handBuilt = new GqlProjection(new GqlVariableExpression("a"), "node");
```

Executing the parsed statement through a path request returns one singleton path per distinct
matched node. The hand-built projection requires the same `a` binding in its query.

## Related reference

[Assembly](index.md) · [GqlQueryExpression](gql-query-expression.md) · [RETURN](../../../../database/graph/language/clauses/return.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlVariableExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
