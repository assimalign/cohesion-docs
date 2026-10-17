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

## Example

```csharp
var capability = ProtocolCapability.Diagnostics;
Console.WriteLine(capability.Name); // diagnostics
Console.WriteLine(capability.MessageTypes[0]); // 14
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolCapability.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
