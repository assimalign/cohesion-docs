# GqlGraphNodeType

An identifying label with implied labels and property rules.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlGraphNodeType(string Label,
    IReadOnlyList<string> ImpliedLabels, IReadOnlyList<GqlGraphPropertyType> Properties);
```

## Behavior and exceptions

Label is the identifying label. ImpliedLabels and Properties describe its requirements.
This AST record carries the supplied lists; it is not the copied immutable GraphNodeType API.
Schema execution validates identifying/implied overlap and references (CG03011).

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlGraphNodeType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
