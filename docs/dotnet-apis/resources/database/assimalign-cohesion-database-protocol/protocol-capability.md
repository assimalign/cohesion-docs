# ProtocolCapability

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed class ProtocolCapability
{
    public ProtocolCapability(string name, params byte[] messageTypes);
    public string Name { get; }
    public IReadOnlyList<byte> MessageTypes { get; }
    public static ProtocolCapability Diagnostics { get; }
}
```

## Behavior and exceptions

Immutable capability descriptor; message identifiers are copied. Diagnostics is the shared
diagnostics name and gates core frame 14. Names contain 1..128 ASCII lowercase letters, digits, dots
or hyphens and compare ordinally. Model names begin with lowercase family name plus a dot. An
extension with no new frame is allowed. Malformed names throw ProtocolException; null messageTypes
throws ArgumentNullException. Family construction validates scope and ownership.

[GraphProtocol.CompositeValues](../assimalign-cohesion-database-graph-protocol/graph-protocol.md) is a
model-owned example with no new message types: graph.composite-values changes Graph value encoding
only after protocol 2.1 negotiation. Diagnostics remains independently negotiable.

## Example

```csharp
var capability = ProtocolCapability.Diagnostics;
Console.WriteLine(capability.Name); // diagnostics
Console.WriteLine(capability.MessageTypes[0]); // 14
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Statement transaction status

[ProtocolTransactionStatusMessage](protocol-transaction-status-message.md) encodes the negotiated one-byte Ready postamble; ProtocolCapability.TransactionStatus advertises transaction-status. Startup Ready retains its existing grammar.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolCapability.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
