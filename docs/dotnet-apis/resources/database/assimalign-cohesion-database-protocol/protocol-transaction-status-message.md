# ProtocolTransactionStatus and ProtocolTransactionStatusMessage

Namespace: `Assimalign.Cohesion.Database.Protocol`.

```csharp
public enum ProtocolTransactionStatus : byte
{
    Idle = (byte)'I',
    Active = (byte)'T',
    Failed = (byte)'E'
}

public sealed record ProtocolTransactionStatusMessage(
    ProtocolTransactionStatus Status, bool HasRetainedState)
{
    public byte[] Encode();
    public static ProtocolTransactionStatusMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

ProtocolCapability.TransactionStatus has Name transaction-status. SQL, Graph and Documents
negotiate an exact two-byte Ready postamble after completed Execute exchanges or statement failures
(also Graph ExecutePaths and Documents Command). Startup Ready retains its version/capability encoding.

| Byte | Meaning |
| --- | --- |
| 0 | ASCII I (Idle), T (Active) or E (Failed) |
| 1 | 0 for no retained session state; 1 when the family reset is needed before pooling |

HasRetainedState exposes the second byte. SQL sets it for sequence observations and timeout settings
different from the session's initial host values; Graph/Documents send false. Idle certifies no open
explicit transaction, Active accepts statements and Failed requires rollback. Pool return rolls back
active/failed work, then invokes ProtocolMessageFamily.ResetStatement only if the latest Ready still
reports retained state. Reuse requires Idle and false.

Encode throws ProtocolException for an invalid Status. Decode throws ProtocolException (CK07002)
unless payload length is exactly two, byte 0 is I/T/E and byte 1 is 0/1. One-byte payloads have no
compatibility reader. No frame identifier or protocol version changes.

## Example

From ProtocolTransactionStatusTests:

```csharp
var ready = new ProtocolTransactionStatusMessage(ProtocolTransactionStatus.Idle, true);
byte[] payload = ready.Encode(); // [0x49, 0x01]
var decoded = ProtocolTransactionStatusMessage.Decode(payload);
Console.WriteLine(decoded.HasRetainedState); // True
```

[Protocol capability](protocol-capability.md) · [Ready negotiation](protocol-ready-message.md) · [Pool reset](../assimalign-cohesion-database-client/design.md#transaction-status-and-pool-reset)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolTransactionStatus.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolTransactionStatusMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolTransactionStatusTests.cs`.
