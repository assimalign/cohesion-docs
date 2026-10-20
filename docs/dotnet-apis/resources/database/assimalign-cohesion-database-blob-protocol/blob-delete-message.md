# BlobDeleteMessage

Requests deletion in the handshake-selected database.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Blob.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Blob.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Blob.Protocol`.

## Signature

```csharp
public sealed record BlobDeleteMessage(string Container, string Name);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

`Encode()` produces the message payload and `Decode(ReadOnlySpan<byte>)` reads it.
Use the model family on a shared `ProtocolChannel` for framing and validation.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [BlobReadMessage](blob-read-message.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/src/BlobDeleteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/docs/DESIGN.md`.
