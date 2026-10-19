# ProtocolDiagnosticSeverity

## Signature

```csharp
public enum ProtocolDiagnosticSeverity : byte { Warning = 1, Error = 2 }
```

## Behavior and exceptions

Only Warning/Error travel in negotiated Diagnostics. Information entries are omitted by servers. The
enum itself stores values; ProtocolDiagnosticsMessage rejects any other wire value with
ProtocolException.

## Example

```csharp
var severity = ProtocolDiagnosticSeverity.Warning;
Console.WriteLine((byte)severity); // 1
```

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolDiagnosticSeverity.cs`.
