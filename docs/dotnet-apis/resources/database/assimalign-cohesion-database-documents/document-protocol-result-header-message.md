# DocumentProtocolResultHeaderMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolResultHeaderMessage(IReadOnlyList<(string Name, byte Type)> Columns, bool WholeDocuments)
{
    public byte[] Encode();
    public static DocumentProtocolResultHeaderMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

ResultHeader (67) carries ordered column names/types and WholeDocuments. Both result shapes use Document frames; WholeDocuments requires exactly one column. Empty names/columns, undefined type tags, malformed fields or trailing bytes throw ProtocolException. The client also rejects duplicate names and invalid projected object members.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit
the shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to
establish response completion.

## Example

```csharp
var message = new DocumentProtocolResultHeaderMessage([("name", (byte)DatabaseType.String)], false);
var decoded = DocumentProtocolResultHeaderMessage.Decode(message.Encode());
Console.WriteLine(decoded.Columns[0].Name); // name
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolResultHeaderMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
