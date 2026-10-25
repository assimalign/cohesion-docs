# GqlNextStage

Marks a segment boundary in a source-ordered statement.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlNextStage(Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

After RETURN, only referenceable projected bindings carry forward. After a modifying segment without RETURN, NEXT starts with one empty binding and no scope. Leading/trailing NEXT, NEXT after bare MATCH and NEXT immediately after WITH are CG00002 in text (invalid hand-built stages use CG05001). NEXT YIELD is CK00001. Segments excludes the boundary node; Stages retains it. See [NEXT](../../../../database/graph/language/clauses/next.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1 RETURN x AS next NEXT RETURN next")).GqlExpression;
Console.WriteLine(query.Stages[2] is GqlNextStage); // True
```

Execution returns one row containing 1; query.Segments.Count is 2.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Next.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
