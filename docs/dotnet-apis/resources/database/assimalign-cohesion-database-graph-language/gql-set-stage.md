# GqlSetStage

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSetStage(IReadOnlyList<GqlSetItem> Items, Location? Location = null) : GqlStage(Location);
```

## Behavior and exceptions

Items executes in list order for each input row; later rows observe prior updates. The final
working copy publishes once per entity. Rows, including RETURN, carry the distinct mutation count.
See [GqlSetItem](gql-set-item.md) for item variants.

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [SET](../../../../database/graph/language/clauses/set.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a.x = 1 RETURN a.x")).GqlExpression;
Console.WriteLine(query.Stages[1] is GqlSetStage); // True
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Set.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
