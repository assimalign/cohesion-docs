# DocumentProtocolCollectionMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolCollectionMessage(string Name)
{
    public byte[] Encode();
    public static DocumentProtocolCollectionMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Collection (71) carries a collection name. Collection commands return names rather than
session-bound engine handles. The codec rejects blank names, malformed UTF-8 and trailing data with
ProtocolException.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = new DocumentProtocolCollectionMessage("items");
var decoded = DocumentProtocolCollectionMessage.Decode(message.Encode());
Console.WriteLine(decoded.Name); // items
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolCollectionMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
