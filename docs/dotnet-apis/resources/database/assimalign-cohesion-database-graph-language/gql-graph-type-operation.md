# GqlGraphTypeOperation

The four current graph-type operations.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlGraphTypeOperation { Set, Add, Alter, Drop }
```

## Behavior and exceptions

Set replaces the complete type; Add requires new names; Alter replaces complete existing
declarations; Drop selects existing names. Empty Add/Alter/Drop preserves state. Reading a value
adds no exceptions; schema admission validates names and references with CG03011.

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlGraphTypeCommand.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
