# ProtocolCopyCompleteMessage

Terminal row and byte counts for a SQL export.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public sealed record ProtocolCopyCompleteMessage(long RowCount, long ByteCount)
{
    public const int MaximumChunkLength = 16 * 1024;
    public byte[] Encode();
    public static ProtocolCopyCompleteMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Encode writes exactly 16 bytes: two big-endian Int64 counters. RowCount excludes the optional
header; ByteCount counts every CopyData payload byte, including a header. Encode and Decode
require nonnegative counters and RowCount <= ByteCount. Decode also requires exactly 16 bytes.
Violations throw ProtocolException with CK07005. The client verifies ByteCount against received
bytes before returning RowCount from CopyToAsync. MaximumChunkLength bounds each data frame;
completion uses CopyComplete (83) under [OutboundStream](sql-protocol.md#outboundstream).

## Example

From SqlCopyProtocolTests:

```csharp
byte[] payload = new ProtocolCopyCompleteMessage(2, 10).Encode();
var complete = ProtocolCopyCompleteMessage.Decode(payload);
Console.WriteLine(payload.Length);     // 16
Console.WriteLine(complete.RowCount);  // 2
Console.WriteLine(complete.ByteCount); // 10
```

## Related reference

[Assembly](index.md) · [ProtocolCopyToMessage](protocol-copy-to-message.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/ProtocolCopyCompleteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/tests/SqlCopyProtocolTests.cs`.
