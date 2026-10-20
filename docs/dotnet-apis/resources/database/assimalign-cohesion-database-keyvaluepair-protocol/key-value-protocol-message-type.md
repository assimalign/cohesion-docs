# KeyValueProtocolMessageType

Message identifiers owned by the KeyValuePair endpoint.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

Assembly: `Assimalign.Cohesion.Database.KeyValuePair.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

## Signature

```csharp
public enum KeyValueProtocolMessageType : byte
{
    Execute = 5,
    ResultHeader = 6,
    ResultRow = 7,
    ResultComplete = 8,
    Transaction = 9,
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [KeyValueProtocol](key-value-protocol.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/KeyValueProtocolMessageType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/docs/DESIGN.md`.
