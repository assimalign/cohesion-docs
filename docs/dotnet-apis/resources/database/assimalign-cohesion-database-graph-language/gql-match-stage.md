# GqlMatchStage

Matches or NULL-extends every incoming binding.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlMatchStage(GqlMatchClause Clause, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

Clause carries paths, its predicate and Optional. It validates against the incoming scope and prefers carried entities as anchors. All comma-pattern variables are declared before inline maps are validated; maps filter complete bindings, including references to later pattern parts. Unknown labels/types affect this stage only and retain warning codes CG01010/CG01011. Missing paths and unbound references use CG05001; a value used as an entity uses CG03003. See [MATCH](../../../../database/graph/language/statements/match.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) LET n = a.name RETURN n")).GqlExpression;
Console.WriteLine(query.Stages[0] is GqlMatchStage); // True
```

Execution returns each matched node name, or NULL for a missing name property.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Match.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
