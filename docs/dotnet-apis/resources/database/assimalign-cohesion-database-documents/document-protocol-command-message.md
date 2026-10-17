# DocumentProtocolCommandMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolCommandMessage(DocumentProtocolOperation Operation, string Collection = "",
    string Id = "", DocumentVersion? ExpectedVersion = null, ReadOnlyMemory<byte> Content = default)
{
    public byte[] Encode();
    public static DocumentProtocolCommandMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Command (69) selects Get/Put/Delete or collection management. Content and optional expected version apply to document mutations. Encode validates operation and field combinations with ProtocolException; malformed payloads/trailing bytes fail Decode. A decoded blank collection name is handled inside the server statement boundary as COHDOC001, preserving the session.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit
the shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to
establish response completion.

## Example

```csharp
var message = new DocumentProtocolCommandMessage(DocumentProtocolOperation.Get, "items", "a");
var decoded = DocumentProtocolCommandMessage.Decode(message.Encode());
Console.WriteLine(decoded.Operation); // Get
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolCommandMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
