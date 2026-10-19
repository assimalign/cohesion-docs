# GqlLetStage

Adds bindings to each incoming row in assignment order.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlLetStage(IReadOnlyList<GqlProjection> Bindings, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

Each binding needs an alias naming its new variable. Later assignments see earlier assignments; incoming names remain. Values may be NULL, scalars, lists, maps or entities. Rebinding is COHDBG003; missing names/aliases, empty bindings or aggregates are COHDBG001. Known scalar member owners use COHDBG003, invalid runtime containers COHDBG013. See [LET](../../../../database/graph/language/clauses/let.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1, y = x + 1 RETURN y")).GqlExpression;
Console.WriteLine(query.Stages[0] is GqlLetStage); // True
```

Execution returns one row containing 2.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Let.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
