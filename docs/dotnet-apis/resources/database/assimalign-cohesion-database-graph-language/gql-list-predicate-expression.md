# GqlListPredicateExpression

A three-valued predicate over a lexically bound list iterator.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlListPredicateExpression : GqlExpression
{
    public GqlListPredicateExpression(GqlListPredicateKind kind, string variable,
        GqlExpression list, GqlExpression predicate, Location? location = null);
    public GqlListPredicateKind Kind { get; }
    public string Variable { get; }
    public GqlExpression List { get; }
    public GqlExpression Predicate { get; }
}
```

Location is `Assimalign.Cohesion.Database.Language.Location`.

## Behavior and exceptions

Construction retains operands and location without validation or execution. List evaluates once
in the enclosing scope; Variable shadows an outer name only in Predicate. Kind selects the
[all/any/none/single truth table](../../../../database/graph/language/expressions/list-predicates.md).
NULL input returns NULL. Only decisive results short-circuit iteration.

Engine validation raises DatabaseException with CG01018 for an absent/empty iterator, CG05025
for a malformed predicate, invalid Kind or body aggregate, CG02015 for a non-list source and
CG02013 for a non-Boolean/non-NULL test. Known incompatible kinds fail planning even on empty
input. Unbound names/parameters use CG05001; excessive reduce shape/type planning uses CG05026.
None has an assigned SQLSTATE. Check parser diagnostics before planning.

## Example

Adapted from GqlReduceParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN all(x IN $xs WHERE x > 0)");
var expression = (GqlListPredicateExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(expression.Kind);     // All
Console.WriteLine(expression.Variable); // x
```

With xs = [1,2], execution returns TRUE; with xs = [], it also returns TRUE.

## Related reference

[Assembly](index.md) · [GqlListPredicateKind](gql-list-predicate-kind.md) · [GqlReduceExpression](gql-reduce-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlListPredicateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlReduceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReduceTests.cs`.
