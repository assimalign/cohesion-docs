# ProtocolMessageType

The shared frame identifiers, including negotiated structured diagnostics.

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public enum ProtocolMessageType : byte
{
    Startup = 1, Authenticate = 2, AuthenticateResponse = 3, Ready = 4,
    Error = 10, Ping = 11, Pong = 12, Terminate = 13, Diagnostics = 14
}
```

## Behavior and exceptions

Model identifiers remain scoped to a fixed endpoint family. Core Diagnostics (14) requires the
agreed diagnostics capability and is server-to-client before terminal Error or successful
completion. SQL and GQL adopt it; Documents uses Failure, Blob/Key-Value do not adopt it. The enum
stores byte values; ProtocolChannel rejects unknown or unagreed identifiers with ProtocolException.
Malformed frame ordering is a protocol violation, not a statement error.

## Example

```csharp
Console.WriteLine((byte)ProtocolMessageType.Diagnostics); // 14
Console.WriteLine(ProtocolCapability.Diagnostics.Name); // diagnostics
```

## Related reference

[ProtocolChannel](protocol-channel.md) · [ProtocolDiagnosticsMessage](protocol-diagnostics-message.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolMessageType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
