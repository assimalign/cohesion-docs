# ProtocolStartupMessage

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed record ProtocolStartupMessage(ProtocolVersion Version, string Database, string Principal)
{
    public IReadOnlyList<string> Capabilities { get; init; }
    public byte[] Encode();
    public static ProtocolStartupMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Retains the startup prefix (version/database/principal). Protocol 2.1 optionally appends capability
count/names; an absent extension is an empty list. A 2.0 peer reads its original prefix. Capability
lists allow at most 64 unique names and 16,384 encoded bytes. Invalid names, duplicates, truncation,
malformed UTF-8 or trailing bytes throw ProtocolException. Capability agreement is completed by
Ready, not by this request alone.

## Example

```csharp
var startup = new ProtocolStartupMessage(ProtocolVersion.Current, "app", "reader")
{ Capabilities = [ProtocolCapability.Diagnostics.Name] };
var decoded = ProtocolStartupMessage.Decode(startup.Encode());
Console.WriteLine(decoded.Capabilities[0]); // diagnostics
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolStartupMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
