# GraphProtocol

Declares the Graph endpoint family and its negotiated composite-value capability.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

## Signature

```csharp
public static class GraphProtocol
{
    public static ProtocolCapability CompositeValues { get; }
    public static ProtocolMessageFamily Family { get; }
}
```

## Behavior and exceptions

CompositeValues names graph.composite-values and adds no message identifier. Family is named Graph,
advertises diagnostics and composite values, and owns bytes 5–9 and 64–66. Protocol 2.1 negotiates
capabilities by intersection. This extension needs no version bump; 2.0 or 2.1 without the capability
keeps baseline scalar tuple parameters/rows. These static accessors introduce no validation exceptions.

With negotiation, Execute and ExecutePaths parameter components use [GraphValueCodec](graph-value-codec.md).
[ResultHeader](graph-protocol-result-header-message.md) bit 7 marks possibly composite columns;
unmarked scalar columns retain their baseline bytes. Possible composite results without agreement
fail COHDBG016 before reads, writer locks or writes, including empty matches/skipped CASE arms.
No SQLSTATE is assigned. The typed client refuses composite parameters locally with ExecutionFailure
before a frame is sent and preserves reuse. Path payloads and GraphPathResult are unchanged.

## Example

```csharp
Console.WriteLine(GraphProtocol.CompositeValues.Name); // graph.composite-values
var startup = new ProtocolStartupMessage(new ProtocolVersion(2, 1), "graph", "tester") {
    Capabilities = [GraphProtocol.CompositeValues.Name]
};
byte[] payload = startup.Encode();
// Send Startup on a Graph-bound ProtocolChannel; use Ready's accepted capabilities.
```

## Related reference

[Assembly](index.md) · [Design](../assimalign-cohesion-database-graph/design.md#graph-wire-family) · [GraphValueCodec](graph-value-codec.md) · [ProtocolCapability](../assimalign-cohesion-database-protocol/protocol-capability.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphCompositeWireTests.cs`.
