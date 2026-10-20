# ProtocolResultHeaderMessage

Opens a streamed result: the column names and type identifiers of the rows that follow. Row frames (`SqlProtocolMessageType.ResultRow`) carry each row as shared tuple-codec bytes, one typed component per column.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Sql.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public sealed record ProtocolResultHeaderMessage(IReadOnlyList<(string Name, byte Type, int? MaxLength)> Columns);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

`Encode()` produces the message payload and `Decode(ReadOnlySpan<byte>)` reads it.
Use the model family on a shared `ProtocolChannel` for framing and validation.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [SqlProtocol](sql-protocol.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/ProtocolResultHeaderMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
