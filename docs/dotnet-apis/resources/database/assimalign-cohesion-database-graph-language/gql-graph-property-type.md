# GqlGraphPropertyType

A scalar property declaration in a graph type.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlGraphPropertyType(string Name, GqlSchemaPropertyType Type, bool Required);
```

## Behavior and exceptions

Name is the property key; Type is the supported scalar identity; Required represents NOT NULL.
The AST record carries syntax values. Write-time type/requiredness violations use CG03003.

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlGraphPropertyType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
