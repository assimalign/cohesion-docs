# GqlRemoveStage

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlRemoveStage(IReadOnlyList<GqlRemoveItem> Items, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

Items removes keys or node labels in list order. Missing keys/labels and OPTIONAL NULL targets
are no-ops. Updated carried aliases refresh after the stage. See [GqlRemoveItem](gql-remove-item.md).

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [REMOVE](../../../../database/graph/language/clauses/remove.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) REMOVE a.x RETURN a.x")).GqlExpression;
Console.WriteLine(query.Stages[1] is GqlRemoveStage); // True
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Remove.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
