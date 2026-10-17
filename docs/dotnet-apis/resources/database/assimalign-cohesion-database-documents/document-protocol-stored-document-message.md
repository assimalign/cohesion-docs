# DocumentProtocolStoredDocumentMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolStoredDocumentMessage(Document Document)
{
    public byte[] Encode();
    public static DocumentProtocolStoredDocumentMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

StoredDocument (70) carries identity, version and UTF-8 JSON content. Get returns zero such frames
for absence; Put returns its newly stored value. Invalid identity/content or malformed fields are
ProtocolException codec failures.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = new DocumentProtocolStoredDocumentMessage(new Document("a", new DocumentVersion(1), "null"u8.ToArray()));
var decoded = DocumentProtocolStoredDocumentMessage.Decode(message.Encode());
Console.WriteLine(decoded.Document.Id.Value); // a
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolStoredDocumentMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
