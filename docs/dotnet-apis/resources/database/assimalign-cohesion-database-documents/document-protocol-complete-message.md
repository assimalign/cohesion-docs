# DocumentProtocolCompleteMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolCompleteMessage(long ResultCount, long AffectedCount)
{
    public byte[] Encode();
    public static DocumentProtocolCompleteMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Complete (66) records ResultCount and AffectedCount as two big-endian Int64 values. Count is
nonnegative; affected count is at least -1 (-1 for reads). Invalid counts or a payload other than 16
bytes throw ProtocolException.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = new DocumentProtocolCompleteMessage(2, -1);
var decoded = DocumentProtocolCompleteMessage.Decode(message.Encode());
Console.WriteLine(decoded.ResultCount); // 2
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolCompleteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
