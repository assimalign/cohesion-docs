# GqlSetPropertyItem

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSetPropertyItem(string Variable, string Property, GqlExpression Value,
    Location? Location = null) : GqlSetItem(Variable, Location);
```

## Behavior and exceptions

Value is a scalar expression evaluated against the current working entity. NULL removes Property;
INSERT's explicit NULL storage rule remains different. Composite literals are COHDBL001 and
computed composites COHDBG003. Node and relationship targets are accepted.

The record constructor retains its operands without semantic validation. The planner validates binding
scope and entity kinds. Unbound targets/invalid direct shapes use COHDBG001; path/group/scalar
targets use COHDBG003. OPTIONAL NULL targets do nothing. The codes have no assigned SQLSTATE.
Parser errors are diagnostics; session text execution rejects them with DatabaseParseException.
See [SET](../../../../database/graph/language/clauses/set.md).

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a.age = 5 RETURN a.age")).GqlExpression;
Console.WriteLine(((GqlSetPropertyItem)query.SetItems[0]).Property); // age
```

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlStage](gql-stage.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSetItem.Property.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlUpdateParserTests.cs`.
