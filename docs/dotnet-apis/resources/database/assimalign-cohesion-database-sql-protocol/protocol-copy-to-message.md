# ProtocolCopyToMessage

SQL export request using the Execute envelope with exactly zero parameters.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public sealed record ProtocolCopyToMessage(string Statement)
{
    public byte[] Encode();
    public static ProtocolCopyToMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Statement carries the COPY text. Encode writes the statement string and a zero Int32 parameter
count, matching the parameterless ProtocolExecuteMessage payload. It throws ArgumentNullException
for null Statement. Decode requires that exact envelope; malformed strings, nonzero parameters,
truncation or trailing bytes throw ProtocolException with CK07005. Blank text reaches the server
as a CS00003 ParseFailure before CopyStart and keeps the session usable.

The payload travels as CopyTo (80), under separately negotiated
[SqlProtocol.OutboundStream](sql-protocol.md#outboundstream). Existing Execute bytes are unchanged.

## Example

From SqlCopyProtocolTests:

```csharp
var request = new ProtocolCopyToMessage("COPY t TO STDOUT");
byte[] payload = request.Encode();
Console.WriteLine(ProtocolCopyToMessage.Decode(payload) == request); // True
```

## Related reference

[Assembly](index.md) · [ProtocolCopyCompleteMessage](protocol-copy-complete-message.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/ProtocolCopyToMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/tests/SqlCopyProtocolTests.cs`.
