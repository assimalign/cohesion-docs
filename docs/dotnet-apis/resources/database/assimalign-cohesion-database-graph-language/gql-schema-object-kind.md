# GqlSchemaObjectKind

The target of a standalone schema command.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlSchemaObjectKind { Index, Constraint, Label, RelationshipType, GraphType }
```

## Behavior and exceptions

Selects a standalone schema target. GraphType selects GqlSchemaCommand.GraphType for
ALTER CURRENT GRAPH TYPE. SHOW CURRENT GRAPH TYPE is a read-only catalog surface.
Reading an enum value adds no exceptions.

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSchemaObjectKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
