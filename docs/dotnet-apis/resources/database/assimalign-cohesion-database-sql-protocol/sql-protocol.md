# SqlProtocol

The immutable Sql endpoint message family.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Sql.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public static partial class SqlProtocol
{
    public static ProtocolMessageFamily Family { get; }
    public static ProtocolCapability OutboundStream { get; }
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. The package split adds no disk format change. No compatibility assembly
or type forwarder is provided.

The Sql family owns identifiers 5–9 and 80–84 and advertises diagnostics, transaction-status and outbound-stream capabilities. Importing
this namespace also brings its `ProtocolMessageType` extension identifiers into scope.

Family.ResetStatement is `DISCARD ALL`. The exact two-byte statement Ready contains transaction
I/T/E and retained-state 0/1. Pool return uses the reset only when the latest Ready reports retained
state, after any needed rollback. See [Ready status](../assimalign-cohesion-database-protocol/protocol-transaction-status-message.md).

## OutboundStream

The independently negotiated `sql.outbound-stream` capability owns COPY frames 80–84. Version 2.0
peers cannot negotiate it and keep existing Execute bytes. Sending these frames without negotiation
or violating sequence/payload rules throws ProtocolException (CK07005).

| Identifier | Name | Payload |
| --- | --- | --- |
| 80 | CopyTo | [Statement and zero parameter count](protocol-copy-to-message.md) |
| 81 | CopyStart | Empty; begins UTF-8 output |
| 82 | CopyData | Nonempty bytes, at most 16 KiB |
| 83 | CopyComplete | [Row and byte counts](protocol-copy-complete-message.md) |
| 84 | CopyAcknowledged | Empty; acknowledges one destination write |

One data frame may be outstanding. The client writes it to the destination before acknowledgment;
completion verifies byte counts, then drains negotiated Ready. A pre-start Error preserves the
session; an incomplete started transfer discards it. See [CopyToAsync](../assimalign-cohesion-database-sql-client/sql-connection.md#copytoasync).

SqlCopyProtocolTests checks the capability name and message set:

```csharp
Console.WriteLine(SqlProtocol.OutboundStream.Name); // sql.outbound-stream
Console.WriteLine(string.Join(",", SqlProtocol.OutboundStream.MessageTypes)); // 80,81,82,83,84
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/SqlProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
