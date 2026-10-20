# DocumentProtocolFailureMessage

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public sealed record DocumentProtocolFailureMessage(ProtocolErrorCode Code, string Message, IReadOnlyList<Diagnostic> Diagnostics)
{
    public byte[] Encode();
    public static DocumentProtocolFailureMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Failure (72) is a terminal statement failure with protocol Code, composed Message and structured
Diagnostics. Retained entries preserve code/severity/span. Malformed fields, JSON or trailing bytes
are ProtocolException codec failures. The server bounds failure text to 4096 UTF-16 characters and
diagnostics to 256 with explicit truncation/omission markers. A valid Failure permits connection
reuse even after partial output; no Complete follows.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = new DocumentProtocolFailureMessage(ProtocolErrorCode.ExecutionFailure, "COHDOC002: Missing collection", []);
var decoded = DocumentProtocolFailureMessage.Decode(message.Encode());
Console.WriteLine(decoded.Code); // ExecutionFailure
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](../assimalign-cohesion-database-documents/document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolFailureMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
