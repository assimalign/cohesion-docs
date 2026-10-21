# ProtocolExecuteMessage

The Key-Value execute request: statement text plus named parameter values. Each value uses one component of the shared scalar tuple codec; this request shape belongs to the Key-Value family.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

Assembly: `Assimalign.Cohesion.Database.KeyValuePair.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

## Signature

```csharp
public sealed record ProtocolExecuteMessage(string Statement, IReadOnlyDictionary<string, byte[]> Parameters);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

`Encode()` produces the message payload and `Decode(ReadOnlySpan<byte>)` reads it.
Use the model family on a shared `ProtocolChannel` for framing and validation.
`Create(string)` builds a request without parameters.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [KeyValueProtocol](key-value-protocol.md) · [KeyValueProtocolMessageType](key-value-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/ProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/docs/DESIGN.md`.
