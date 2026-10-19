# GqlSetPropertiesItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSetPropertiesItem(string Variable,
    IReadOnlyDictionary<string, GqlExpression> Properties, bool Merge = false,
    Location? Location = null) : GqlSetItem(Variable, Location);
```

## Behavior and exceptions

Merge false represents literal-map replacement (=); true represents +=. Replacement removes
absent keys, merge preserves them, and NULL values remove keys in either mode. Labels remain
unchanged. Whole-map parameter/expression text is COHDBL001; properties must be scalar.

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [SET](../../../../database/graph/language/clauses/set.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a += {age:1} RETURN a.age")).GqlExpression;
Console.WriteLine(((GqlSetPropertiesItem)query.SetItems[0]).Merge); // True
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSetItem.Properties.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
