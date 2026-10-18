# GqlDeleteStage

Deletes bound entities and removes their variables from scope.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlDeleteStage(IReadOnlyList<string> Variables, bool Detach = false, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

Detach also removes incident relationships. Repeated references are deleted once; affected counts report actual deletions. Value bindings, restricted connected-node deletion and later references to deleted variables use COHDBG003; malformed/unbound deletion operands use COHDBG001. A direct RETURN after DELETE is COHDBL001; a following MATCH or NEXT can continue. Writes remain atomic with the rest of the statement. See [DELETE](../../../../database/graph/language/statements/delete.md).

The record constructor stores its operands; semantic validation belongs to the planner. Location
is the optional source span inherited from [GqlStage](gql-stage.md). Text parsing returns diagnostics;
session execution rejects parser errors with DatabaseParseException. Stage diagnostic codes have
no assigned SQLSTATE. See [composition diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) DELETE a MATCH (b) RETURN b")).GqlExpression;
Console.WriteLine(query.Stages[1] is GqlDeleteStage); // True
```

The parser retains Match, Delete, Match, Return stages; execution applies deletion before the later match.

## Related reference

[GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Delete.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
