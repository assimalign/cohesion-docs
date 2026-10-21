# GraphProtocolMessageType

Message identifiers scoped to graph endpoints.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public enum GraphProtocolMessageType : byte
{
    Execute = 5,
    ResultHeader = 6,
    ResultRow = 7,
    ResultComplete = 8,
    Transaction = 9,
    ExecutePaths = 64,
    Path = 65,
    PathsComplete = 66,
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [GraphProtocol](graph-protocol.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphProtocolMessageType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
