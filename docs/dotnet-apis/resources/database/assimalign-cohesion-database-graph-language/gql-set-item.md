# GqlSetItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public abstract record GqlSetItem
{
    public string Variable { get; }
    public Location? Location { get; }
}
```

## Behavior and exceptions

This assembly-owned variant family has a private protected constructor. Variable names the bound
entity and Location retains the optional source span. Public variants are
[property](gql-set-property-item.md), [property map](gql-set-properties-item.md) and
[labels](gql-set-labels-item.md). External derivation is unavailable.

Accessors add no validation exceptions. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [SET](../../../../database/graph/language/clauses/set.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a.x = 1 RETURN a.x")).GqlExpression;
Console.WriteLine(query.SetItems[0].Variable); // a
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSetItem.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
