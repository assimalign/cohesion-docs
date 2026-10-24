# GqlForStage

Expands a list using FOR or Cypher UNWIND.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlForStage(string Variable, GqlExpression List, bool IsUnwind = false, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

IsUnwind preserves the spelling. The list evaluates per row; NULL/empty lists produce no rows and non-lists use CG02013, including strings/byte arrays. Incoming bindings remain and Variable must be new (CG03003). Proven node/relationship element kinds survive expansion and can anchor MATCH/DELETE. Unknown/mixed elements stay values. Expansion shares the 100,000-row bound (CG05004). WITH ORDINALITY/OFFSET suffixes are CK00001. Entity lists remain unavailable as final wire results (CG05001; #1662). See [FOR/UNWIND](../../../../database/graph/language/clauses/for.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "UNWIND [1,2] AS x RETURN x")).GqlExpression;
Console.WriteLine(query.Stages[0] is GqlForStage); // True
```

Execution returns 1, then 2; the node has IsUnwind = true.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.For.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
