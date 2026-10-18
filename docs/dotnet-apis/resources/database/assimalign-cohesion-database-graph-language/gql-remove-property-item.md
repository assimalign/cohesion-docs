# GqlRemovePropertyItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlRemovePropertyItem(string Variable, string Property,
    Location? Location = null) : GqlRemoveItem(Variable, Location);
```

## Behavior and exceptions

Removes Property from a node or relationship. An absent key is a no-op; subsequent property
reads return NULL and keys(entity) excludes the removed key.

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [REMOVE](../../../../database/graph/language/clauses/remove.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) REMOVE a.age RETURN a.age")).GqlExpression;
Console.WriteLine(((GqlRemovePropertyItem)query.RemoveItems[0]).Property); // age
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlRemoveItem.Property.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
