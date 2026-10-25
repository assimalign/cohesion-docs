# ProtocolReadyMessage

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed record ProtocolReadyMessage(ProtocolVersion Version, IReadOnlyList<string> Capabilities)
{
    public byte[] Encode();
    public static ProtocolReadyMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Carries agreed version and two-sided capability intersection in server registration order. Empty
payload means 2.0 with no capabilities; encoding that agreement stays empty. 2.1 writes version and
names. Null Capabilities throws ArgumentNullException at construction and init/with assignment.
Encode/Decode reject malformed version/list/payload with ProtocolException. The client also rejects
unsolicited capability agreements.

## Example

```csharp
var ready = ProtocolReadyMessage.Decode(ReadOnlySpan<byte>.Empty);
Console.WriteLine(ready.Version); // 2.0
Console.WriteLine(ready.Capabilities.Count); // 0
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Statement transaction status

[ProtocolTransactionStatusMessage](protocol-transaction-status-message.md) encodes the negotiated one-byte Ready postamble; ProtocolCapability.TransactionStatus advertises transaction-status. Startup Ready retains its existing grammar.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolReadyMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
