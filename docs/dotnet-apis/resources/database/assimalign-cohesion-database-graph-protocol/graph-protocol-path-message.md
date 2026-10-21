# GraphProtocolPathMessage

An ordered path retaining node identities, labels, properties, and directed relationships.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public sealed record GraphProtocolPathMessage(
    IReadOnlyList<GraphNode> Nodes, IReadOnlyList<GraphRelationship> Relationships)
{
    public int GetEncodedLength();
    public int Encode(Span<byte> destination);
    public byte[] Encode();
    public static GraphProtocolPathMessage Decode(ReadOnlySpan<byte> payload);
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

GetEncodedLength measures and validates the complete payload without allocating an encoded copy.
Encode(Span<byte>) writes directly into caller-owned storage and returns the byte count; a
destination larger than needed is allowed. Encode() allocates and returns an owned array.
All paths preserve the existing bytes, scalar property tags, Unicode and relationship direction.
No entity deduplication is performed. Decode reads the complete payload into owned graph values.
Use the model family on a shared ProtocolChannel for framing and validation.

## Exceptions

Measurement and encoding throw ProtocolException for invalid path shapes, zero identities,
disconnected edges, unsupported property values or non-finite numbers, and payloads above
16,777,216 bytes. GetEncodedLength
refuses oversize payloads before an encoded payload is allocated. Encode(Span<byte>) throws
ArgumentException when destination is too small; it does not promise an unchanged destination
on failure. Decode also rejects malformed lengths/counts, duplicate property names and trailing
bytes. These are codec failures, with no new model diagnostic code or SQLSTATE.

## Example

This uses the single-node form exercised by the codec and client tests:

```csharp
using System;
using System.Collections.Generic;

using Assimalign.Cohesion.Database.Graph.Protocol;

var node = new GraphNode(new GraphNodeId(1), ["Person"],
    new Dictionary<string, object?> { ["name"] = "Café 😀", ["score"] = 12.50m });
var message = new GraphProtocolPathMessage([node], []);
byte[] payload = new byte[message.GetEncodedLength()];
int written = message.Encode(payload);
var decoded = GraphProtocolPathMessage.Decode(payload);
Console.WriteLine(written == payload.Length); // True
Console.WriteLine(decoded.Nodes[0].Properties["name"]); // Café 😀
Console.WriteLine(decoded.Relationships.Count); // 0
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [GraphProtocol](graph-protocol.md) · [GraphNode](graph-node.md) · [GraphNodeId](graph-node-id.md) · [GraphRelationship](graph-relationship.md) · [GraphRelationshipId](graph-relationship-id.md)

## Sources

- **Span encoding, byte equivalence and oversize refusal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/tests/GraphPathEncodingTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphProtocolPathMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
