# GraphPath

A matched path containing ordered graph entities from one statement snapshot.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public sealed record GraphPath(IReadOnlyList<GraphNode> Nodes, IReadOnlyList<GraphRelationship> Relationships);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

Nodes follow traversal order, including repeats. Relationships retain stored directions. A valid
path has at least one node and one fewer relationships than nodes.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [GraphNode](graph-node.md) · [GraphRelationship](graph-relationship.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphPath.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
