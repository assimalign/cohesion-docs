# GraphNodeId

A strongly-typed node identity, unique within a graph database.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public readonly record struct GraphNodeId(ulong Value) : IComparable<GraphNodeId>;
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

The underlying `Value` is an unsigned 64-bit value; comparison follows that value.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [GraphNode](graph-node.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/ValueObjects/GraphNodeId.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
