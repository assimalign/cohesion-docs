# ProtocolDiagnostic

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed record ProtocolDiagnostic(ProtocolDiagnosticSeverity Severity,
    string? Code, string? Message, int? Start, int? End, int Line);
```

## Behavior and exceptions

Model-neutral wire entry with no dependency on language/type diagnostic assemblies. Severity accepts
Warning (1) or Error (2) in the codec. Offsets are both null for unlocated entries, otherwise 0 <=
Start <= End; End is exclusive and Line is one-based. Code and Message may be null; null/empty
fields encode empty and decode null. The record stores values; ProtocolDiagnosticsMessage validates
wire fields and throws ProtocolException for malformed entries.

## Example

```csharp
var entry = new ProtocolDiagnostic(ProtocolDiagnosticSeverity.Warning,
    "CG01010", "Unknown label", 9, 15, 1);
Console.WriteLine(entry.Code); // CG01010
```

## Related reference

[ProtocolDiagnosticSeverity](protocol-diagnostic-severity.md)


[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolDiagnostic.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
