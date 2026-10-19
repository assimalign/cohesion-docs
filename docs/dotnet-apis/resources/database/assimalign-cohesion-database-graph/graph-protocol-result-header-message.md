# GraphProtocolResultHeaderMessage

Describes ordered Graph row columns and negotiated composite-column flags.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed record GraphProtocolResultHeaderMessage(IReadOnlyList<(string Name, byte Type)> Columns)
{
    public byte[] Encode();
    public static GraphProtocolResultHeaderMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Columns keeps each name and raw type byte. Bits 0–6 contain DatabaseType; with
[graph.composite-values](graph-protocol.md), bit 7 marks a column that may return composites.
Direct COUNT projections report DatabaseType.Int64, including zero; other query metadata
remains DatabaseType.Null (unknown). The client removes the flag from its public
GraphColumn.Type. Encode/Decode retain it in this protocol record.

An unmarked row column uses one ordinary scalar tuple component with no added bytes. A marked
column uses tag 0 plus a scalar component, or tag 1 plus a little-endian Int32 byte length and
one [GraphValueCodec](graph-value-codec.md) component. Unsigned and Decimal scalar values use the latter to
preserve their CLR types. Lists decode to object?[], maps to read-only ordinal dictionaries.
A flagged header without negotiation is a protocol violation at the exchange boundary.

Record construction stores the supplied list without semantic validation. Decode throws
ProtocolException for invalid column counts, malformed/truncated names or missing type bytes.
The receiving exchange validates type flags, component counts and ordering. Values allow depth
64, 65,536 total values and 16 MiB encoded bytes; a row also fits 16 MiB. Server precommit checks
ensure a result size failure cannot leave committed statement writes.

## Example

```csharp
byte compositeType = (byte)((byte)DatabaseType.Null | 0x80);
var header = new GraphProtocolResultHeaderMessage([("value", compositeType)]);
var decoded = GraphProtocolResultHeaderMessage.Decode(header.Encode());
Console.WriteLine((decoded.Columns[0].Type & 0x80) != 0); // True
```

This header can describe a negotiated `RETURN [1, NULL] AS value` result. A scalar-only result
column keeps bit 7 clear, including on a negotiated connection.

## Related reference

[Assembly](index.md) · [GraphProtocol](graph-protocol.md) · [GraphValueCodec](graph-value-codec.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Protocol/GraphProtocolResultHeaderMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/Internal/GraphExecuteExchange.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphCompositeWireTests.cs`.
