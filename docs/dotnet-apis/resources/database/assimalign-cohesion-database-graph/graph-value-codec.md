# GraphValueCodec

Encodes scalar, list and map components for the negotiated Graph value format.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public static class GraphValueCodec
{
    public const int MaxDepth = 64;
    public const int MaxValueCount = 65_536;
    public const int MaxPayloadLength = 16_777_216;
    public static byte[] Encode(object? value);
    public static object? Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Encode emits one complete component. Scalars use DatabaseValueCodec components, with explicit
unsigned tags to preserve Byte, UInt16, UInt32 and UInt64. Lists accept BCL IList except byte[]
binary scalars; arrays must be one-dimensional and zero-based. Maps accept
IReadOnlyDictionary<string, object?>. Decode returns object?[] and read-only ordinal string maps,
retaining scalar runtime types. This is a transient wire format, not an index-key or stored-property
format. Use it only after negotiating [GraphProtocol.CompositeValues](graph-protocol.md).

Root depth is zero; the value count includes root and descendants. Byte limits include framing,
real UTF-8 bytes and scalar escaping. Encode throws DatabaseTypeException for unsupported types,
invalid array dimensions, malformed text or exceeded bounds; cyclic containers eventually exceed
the depth limit. Decode throws ProtocolException for malformed/oversized payloads, invalid tags,
counts, lengths, UTF-8, duplicate ordinal keys or trailing bytes. It checks depth, aggregate counts
and available minimum bytes before container/string allocation.

## Component format

All component counts and lengths are signed nonnegative little-endian Int32. This differs from
the surrounding Graph execute/header payload's big-endian lengths.

| Tag | Payload |
| --- | --- |
| 0 | Byte length, then exactly one DatabaseValueCodec scalar component |
| 1 | Item count, then recursive components |
| 2 | Entry count; repeated UTF-8 key byte length, key bytes and recursive value |
| 3 | Byte in one byte |
| 4 | UInt16 in two little-endian bytes |
| 5 | UInt32 in four little-endian bytes |
| 6 | UInt64 in eight little-endian bytes |

The server validates parameter bounds during planning with COHDBG003. Constructed values and row
sizes check within the statement transaction before commit with COHDBG013. An unnegotiated possible
composite result uses COHDBG016 before reads/writes (no SQLSTATE). Low-level malformed wire
components become ProtocolViolation. The standalone codec reports its exceptions above.

## Example

Adapted from GraphValueCodecTests:

```csharp
object? value = new Dictionary<string, object?> {
    ["nested"] = new object?[] { 0.1m, 3U, null }
};
byte[] bytes = GraphValueCodec.Encode(value);
var map = (IReadOnlyDictionary<string, object?>)GraphValueCodec.Decode(bytes)!;
var items = (object?[])map["nested"]!;
Console.WriteLine(items[0]!.GetType() == typeof(decimal)); // True
Console.WriteLine(items[1]!.GetType() == typeof(uint));    // True
Console.WriteLine(items[2] is null);                       // True
```

## Related reference

[Assembly](index.md) · [GraphProtocol](graph-protocol.md) · [Execute payload](graph-protocol-execute-message.md) · [Result header](graph-protocol-result-header-message.md) · [Value limits](../../../../database/graph/language/expressions/list-values.md#limits-and-wire-capability)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Protocol/GraphValueCodec.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueCodecTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphCompositeWireTests.cs`.
