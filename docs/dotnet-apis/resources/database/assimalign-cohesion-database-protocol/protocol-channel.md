# ProtocolChannel

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed class ProtocolChannel : IAsyncDisposable
{
    public ProtocolChannel(Stream stream, ProtocolMessageFamily family, bool leaveOpen = false,
        bool batchWrites = false);
    public ProtocolMessageFamily Family { get; }
    public ProtocolFrameReader Reader { get; }
    public ProtocolFrameWriter Writer { get; }
    public IReadOnlyList<string> Capabilities { get; }
    public ProtocolReadyMessage Negotiate(ProtocolVersion version, IReadOnlyList<string> peerCapabilities);
    public void SetAgreement(ProtocolReadyMessage agreement);
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

Permanently binds a stream to one family. Negotiate selects the local/peer intersection and applies
it once; SetAgreement applies a validated Ready once on the client. A repeated, unsupported or
invalid agreement throws ProtocolException. Gated reads/writes, including Diagnostics, fail until
agreed. Null stream/family throws ArgumentNullException. Dispose closes the stream unless leaveOpen.
No family switching, pipelining or model payload dispatch is added.

batchWrites: true selects a pooled writer bounded by 64 KiB including frame headers or 32
frames. A larger frame is written alone under the existing 16 MiB payload limit. Serialize
writes, flushes and disposal, and explicitly flush result and latency boundaries. Disposal
discards pending bytes. Failed sends fault the writer; later writes/flushes throw IOException.
The default remains immediate writes. Reader supports callback-scoped borrowed payloads while
ReadFrameAsync still returns owned memory. See [framing APIs and example](design.md#bounded-batches-and-borrowed-payloads).

## Example

```csharp
var family = new ProtocolMessageFamily("Example", [ProtocolCapability.Diagnostics], null, 64);
await using var channel = new ProtocolChannel(new MemoryStream(), family);
var agreement = channel.Negotiate(ProtocolVersion.Current, ["diagnostics"]);
Console.WriteLine(agreement.Capabilities[0]); // diagnostics
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- **Borrowed channel reads and batch behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolBatchTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolChannel.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
