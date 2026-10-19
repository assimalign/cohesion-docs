# GqlFilterStage

Keeps incoming rows whose predicate evaluates to TRUE.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlFilterStage(GqlExpression Predicate, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

FILTER and FILTER WHERE share this node. FALSE/NULL remove rows; an evaluated non-Boolean uses COHDBG013. Unbound names, missing operands and aggregates use COHDBG001. It filters after an optional MATCH has already NULL-extended its rows. See [FILTER](../../../../database/graph/language/clauses/filter.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1 FILTER WHERE x = 1 RETURN x")).GqlExpression;
Console.WriteLine(query.Stages[1] is GqlFilterStage); // True
```

Execution returns one row containing 1.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Filter.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
