# ProtocolTransactionStatus and ProtocolTransactionStatusMessage

Namespace: `Assimalign.Cohesion.Database.Protocol`.

```csharp
public enum ProtocolTransactionStatus : byte
{
    Idle = (byte)'I',
    Active = (byte)'T',
    Failed = (byte)'E'
}

public sealed record ProtocolTransactionStatusMessage(ProtocolTransactionStatus Status)
{
    public byte[] Encode();
    public static ProtocolTransactionStatusMessage Decode(ReadOnlySpan<byte> payload);
}
```

ProtocolCapability.TransactionStatus has Name transaction-status. SQL and Graph negotiate a one-byte Ready postamble after completed Execute exchanges or statement failures; startup Ready retains its existing encoding. Encode rejects invalid enum values; Decode requires exactly one valid byte, otherwise ProtocolException. Idle certifies no open explicit transaction; Active accepts statements; Failed requires rollback. Pool return resets open/failed transactions and reuses only certified Idle sessions.

[Protocol capability](protocol-capability.md) · [Ready negotiation](protocol-ready-message.md) · [Pool reset](../assimalign-cohesion-database-client/design.md#transaction-status-and-pool-reset)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolTransactionStatus.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolTransactionStatusMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolTransactionStatusTests.cs`.
