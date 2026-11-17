# GqlGraphTypeCommand

Current graph-type language vocabulary.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlGraphTypeCommand
{
    public GqlGraphTypeOperation Operation { get; init; }
    public IReadOnlyList<GqlGraphNodeType> Nodes { get; init; } = [];
    public IReadOnlyList<GqlGraphEdgeType> Edges { get; init; } = [];
    public override string ToString();
}
```

## Behavior and exceptions

Operation selects SET/ADD/ALTER/DROP. Nodes and Edges carry declarations or identifying names
for DROP. ToString quotes identifiers and orders declarations, implied labels and properties
ordinally. The record is attached through GqlSchemaCommand.GraphType; it does not publish a type.
Programmatic schema-command validation rejects malformed combinations with CG05001.

See [current graph-type syntax](../../../../database/graph/language/statements/index.md#current-graph-type)
for command semantics, aliases, supported scalar types and refusals.

## Related reference

[Assembly](index.md) · [GqlSchemaCommand](gql-schema-command.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlGraphTypeCommand.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlGraphTypeParserTests.cs`.
