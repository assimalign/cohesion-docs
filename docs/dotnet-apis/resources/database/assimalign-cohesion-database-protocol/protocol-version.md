# ProtocolVersion

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public readonly record struct ProtocolVersion(ushort Major, ushort Minor) : IComparable<ProtocolVersion>
{
    public static ProtocolVersion Current { get; }
    public static bool TryNegotiate(ProtocolVersion requested, out ProtocolVersion negotiated);
    public int CompareTo(ProtocolVersion other);
    public override string ToString();
}
```

## Behavior and exceptions

Current is 2.1. TryNegotiate refuses different majors with false/default; otherwise selects the lower minor. Comparison orders major then minor. The 2.0 major introduced Documents JSON projection frames; 2.1 adds capability negotiation. These methods have no additional validation exception for unsupported versions; negotiation reports false.

## Example

```csharp
bool accepted = ProtocolVersion.TryNegotiate(new(2, 0), out var agreed);
Console.WriteLine($"{accepted}: {agreed}"); // True: 2.0
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ValueObjects/ProtocolVersion.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
