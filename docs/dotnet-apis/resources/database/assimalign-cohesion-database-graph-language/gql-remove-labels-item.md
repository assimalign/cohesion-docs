# GqlRemoveLabelsItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlRemoveLabelsItem(string Variable, IReadOnlyList<string> Labels,
    bool IsColon = true, Location? Location = null) : GqlRemoveItem(Variable, Location);
```

## Behavior and exceptions

Removes the listed node labels, ignoring absent ones. IsColon distinguishes colon syntax from IS.
Relationship labels/types cannot be removed (CG03003).

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use CG05001; path/group/scalar
targets use CG03003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [REMOVE](../../../../database/graph/language/clauses/remove.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) REMOVE a IS Admin RETURN a.age")).GqlExpression;
Console.WriteLine(((GqlRemoveLabelsItem)query.RemoveItems[0]).IsColon); // False
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlRemoveItem.Labels.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
