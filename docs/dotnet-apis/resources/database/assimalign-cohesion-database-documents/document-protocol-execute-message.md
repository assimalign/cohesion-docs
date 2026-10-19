# DocumentProtocolExecuteMessage

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed record DocumentProtocolExecuteMessage(string Statement, ReadOnlyMemory<byte> Parameters)
{
    public byte[] Encode();
    public static DocumentProtocolExecuteMessage Decode(ReadOnlySpan<byte> payload);
    public static DocumentProtocolExecuteMessage Create(string statement);
}
```

## Behavior and exceptions

Execute (64) carries OQL plus a UTF-8 JSON object of parameters. Create(statement) supplies {}.
Encode validates JSON; Decode also rejects inconsistent lengths and trailing bytes with
ProtocolException.

Record construction stores fields; Encode/Decode enforce the wire grammar. Each payload must fit the
shared frame limit. Use the model protocol's exchange ordering, not these codecs alone, to establish
response completion.

## Example

```csharp
var message = DocumentProtocolExecuteMessage.Create("SELECT name FROM items");
var decoded = DocumentProtocolExecuteMessage.Decode(message.Encode());
Console.WriteLine(decoded.Statement); // SELECT name FROM items
```

## Related reference

[DocumentProtocol](document-protocol.md) · [Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Protocol/DocumentProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
