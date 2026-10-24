# GqlSetLabelsItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSetLabelsItem(string Variable, IReadOnlyList<string> Labels,
    bool IsColon = true, Location? Location = null) : GqlSetItem(Variable, Location);
```

## Behavior and exceptions

Labels adds the listed node labels; existing labels are no-ops. IsColon distinguishes colon
syntax from IS. Repeated colon syntax can carry multiple labels. Relationships retain an immutable
type and reject this item with CG03003.

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use CG05001; path/group/scalar
targets use CG03003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [SET](../../../../database/graph/language/clauses/set.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a IS Admin RETURN a.age")).GqlExpression;
Console.WriteLine(((GqlSetLabelsItem)query.SetItems[0]).IsColon); // False
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSetItem.Labels.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
