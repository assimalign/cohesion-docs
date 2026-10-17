# ProtocolDiagnosticsMessage

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed record ProtocolDiagnosticsMessage(IReadOnlyList<ProtocolDiagnostic> Entries)
{
    public const int MaxEntryCount = 4096;
    public const int MaxPayloadLength = 1_048_576;
    public byte[] Encode();
    public static ProtocolDiagnosticsMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Encodes i32 count then severity, code, message, start/end and line for each entry, using big-endian integers. Null Entries throws ArgumentNullException at construction or init/with assignment. Invalid spans, severity, line, counts, UTF-8, trailing data or size limits throw ProtocolException. Codec bounds remain strict; SQL/GQL servers apply the outbound prefix-plus-COHDBP001 policy before encoding. They preserve the original outcome and session when diagnostics overflow. Information entries are never sent.

## Example

```csharp
var message = new ProtocolDiagnosticsMessage([
    new(ProtocolDiagnosticSeverity.Warning, "COHDBG010", "Unknown label", null, null, 1)
]);
var decoded = ProtocolDiagnosticsMessage.Decode(message.Encode());
Console.WriteLine(decoded.Entries[0].Code); // COHDBG010
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolDiagnosticsMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
