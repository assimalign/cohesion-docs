# GqlInsertStage

Inserts node/path patterns for each incoming row.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlInsertStage(IReadOnlyList<GqlPathPattern> Paths, bool IsCreate = false, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

IsCreate distinguishes compatibility CREATE. Maps evaluate scalar expressions against the incoming scope; new variables in the same INSERT are unavailable to those expressions (CG05001). Composite property syntax is CK00001; evaluated composite values are CG03003. Any write stage acquires the writer lock before execution of the first stage. Later MATCH sees inserted entities and newly introduced labels/types. Empty patterns use CG05001. See [INSERT](../../../../database/graph/language/statements/insert.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "FOR x IN [1] INSERT (:P {k:x * 2})")).GqlExpression;
Console.WriteLine(query.Stages[1] is GqlInsertStage); // True
```

Execution inserts one P node with k = 2 and reports one affected entity.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Insert.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
