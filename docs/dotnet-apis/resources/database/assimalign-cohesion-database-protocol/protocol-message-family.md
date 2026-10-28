# ProtocolMessageFamily

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed class ProtocolMessageFamily
{
    public ProtocolMessageFamily(string name, params byte[] messageTypes);
    public ProtocolMessageFamily(string name, IReadOnlyList<ProtocolCapability> capabilities,
        string? resetStatement, params byte[] messageTypes);
    public string Name { get; }
    public IReadOnlyList<ProtocolCapability> Capabilities { get; }
    public string? ResetStatement { get; }
    public bool Supports(ProtocolMessageType type);
}
```

## Behavior and exceptions

Snapshots endpoint identifiers and capabilities. Model identifiers are 5..9 or 64..255. A capability
may gate only its own family identifiers, except the shared Diagnostics and TransactionStatus descriptors. Invalid scope,
duplicate name/identifier or reserved identifier throws ArgumentException; null lists/descriptors
throw ArgumentNullException. Supports recognizes defined core frames, while ProtocolChannel
additionally gates their negotiated use.

ResetStatement names the text that clears retained session state, or null for a family without
such a reset. A non-null empty/whitespace value throws ArgumentException. SqlProtocol.Family
declares `DISCARD ALL`; Graph and Documents declare null. Pool return invokes it only when
[Ready.HasRetainedState](protocol-transaction-status-message.md) is true, after any needed rollback.

## Example

```csharp
var family = new ProtocolMessageFamily("Example",
    [ProtocolCapability.Diagnostics], null, 64);
Console.WriteLine(family.Supports(ProtocolMessageType.Diagnostics)); // True
// Channel agreement still gates reading or writing frame 14.
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolMessageFamily.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
