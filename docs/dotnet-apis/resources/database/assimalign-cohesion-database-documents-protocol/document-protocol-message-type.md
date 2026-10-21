# DocumentProtocolMessageType

Message identifiers scoped to document endpoints.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public enum DocumentProtocolMessageType : byte
{
    Execute = 64,
    Document = 65,
    Complete = 66,
    ResultHeader = 67,
    ResultRow = 68,
    Command = 69,
    StoredDocument = 70,
    Collection = 71,
    Failure = 72,
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [DocumentProtocol](document-protocol.md) · [Document](document.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolMessageType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
