# GqlListPredicateKind

Selects a list predicate's quantification rule.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlListPredicateKind
{
    All,
    Any,
    None,
    Single
}
```

## Behavior and exceptions

All requires every test to pass; Any requires at least one; None requires no TRUE test; Single
requires exactly one. Empty input returns TRUE for All/None and FALSE for Any/Single. Every kind
returns NULL for a NULL list. Unknown test results follow the
[three-valued rules](../../../../database/graph/language/expressions/list-predicates.md).
An undefined enum value in a hand-built AST fails engine planning with DatabaseException/CG05025
(no SQLSTATE).

## Example

From the parser test shape:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN single(x IN [TRUE,FALSE] WHERE x)");
var expression = (GqlListPredicateExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(expression.Kind == GqlListPredicateKind.Single); // True
```

Execution returns TRUE.

## Related reference

[Assembly](index.md) · [GqlListPredicateExpression](gql-list-predicate-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlListPredicateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlReduceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReduceTests.cs`.
