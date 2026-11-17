# GqlGraphEdgeType

A directed edge declaration with node-type endpoints.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlGraphEdgeType(string Type, string Source, string Target,
    IReadOnlyList<GqlGraphPropertyType> Properties);
```

## Behavior and exceptions

Type identifies the directed relationship type; Source and Target name identifying node
types. Properties carries scalar rules. The AST record does not publish or validate stored
edges; execution resolves endpoints (CG03011) and enforces writes (CG03013/CG03003).

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlGraphEdgeType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
