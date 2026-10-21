# DocumentProtocolOperation

Operations carried by a Documents Command frame.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public enum DocumentProtocolOperation : byte
{
    Get = 1,
    Put = 2,
    Delete = 3,
    CreateCollection = 4,
    GetCollection = 5,
    DropCollection = 6,
    ListCollections = 7,
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [DocumentProtocol](document-protocol.md) · [Document](document.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolOperation.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
