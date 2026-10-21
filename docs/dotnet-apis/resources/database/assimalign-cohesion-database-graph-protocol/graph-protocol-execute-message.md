# GraphProtocolExecuteMessage

Carries GQL text and pre-encoded named parameters for Execute or ExecutePaths.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public sealed record GraphProtocolExecuteMessage(
    string Statement, IReadOnlyDictionary<string, byte[]> Parameters)
{
    public static GraphProtocolExecuteMessage Create(string statement);
    public byte[] Encode();
    public static GraphProtocolExecuteMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior and exceptions

Parameters maps names to complete component bytes. With graph.composite-values, encode each using
[GraphValueCodec](graph-value-codec.md), including scalar parameters. Without it, use the baseline
DatabaseValueCodec component. This record does not negotiate or select that encoding for callers.
Create supplies an empty parameter dictionary. Encode writes the text, count, names and component
lengths; surrounding lengths are big-endian. Component internals retain their own format.

Decode throws ProtocolException for invalid counts, truncated strings or invalid parameter lengths.
It returns the component byte arrays without validating their values; the receiving exchange applies
the agreed codec. Malformed composite values become ProtocolViolation. Record construction stores
the supplied members without semantic validation. Typed client code normally performs the encoding.

## Example

After the capability is accepted:

```csharp
var message = new GraphProtocolExecuteMessage("RETURN $xs", new Dictionary<string, byte[]> {
    ["xs"] = GraphValueCodec.Encode(new object?[] { 1L, null })
});
var decoded = GraphProtocolExecuteMessage.Decode(message.Encode());
var xs = (object?[])GraphValueCodec.Decode(decoded.Parameters["xs"])!;
Console.WriteLine(xs.Length); // 2
```

## Related reference

[Assembly](index.md) · [GraphProtocol](graph-protocol.md) · [GraphValueCodec](graph-value-codec.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphCompositeWireTests.cs`.
