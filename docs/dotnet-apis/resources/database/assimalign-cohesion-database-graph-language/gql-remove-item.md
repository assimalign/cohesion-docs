# GqlRemoveItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public abstract record GqlRemoveItem
{
    public string Variable { get; }
    public Location? Location { get; }
}
```

## Behavior and exceptions

This assembly-owned variant family has a private protected constructor. Variable names the entity;
Location retains the optional source span. The public variants remove a
[property](gql-remove-property-item.md) or [node labels](gql-remove-labels-item.md).

Accessors add no validation exceptions. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [REMOVE](../../../../database/graph/language/clauses/remove.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) REMOVE a.x RETURN a.x")).GqlExpression;
Console.WriteLine(query.RemoveItems[0].Variable); // a
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlRemoveItem.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
