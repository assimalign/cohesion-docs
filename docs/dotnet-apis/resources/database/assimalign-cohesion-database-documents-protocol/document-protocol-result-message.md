# DocumentProtocolResultMessage

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public sealed record DocumentProtocolResultMessage(ReadOnlyMemory<byte> Json)
{
    public byte[] Encode();
    public static DocumentProtocolResultMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Document (65) carries one valid JSON value: a whole source document or projected object. MISSING
members are omitted; NULL remains. Encode/Decode validate JSON and reject malformed UTF-8/JSON with
ProtocolException.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = new DocumentProtocolResultMessage("{"name":"Ada"}"u8.ToArray());
var decoded = DocumentProtocolResultMessage.Decode(message.Encode());
Console.WriteLine(Encoding.UTF8.GetString(decoded.Json.Span)); // {"name":"Ada"}
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](../assimalign-cohesion-database-documents/document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolResultMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
