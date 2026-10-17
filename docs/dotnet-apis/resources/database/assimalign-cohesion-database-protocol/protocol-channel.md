# ProtocolChannel

Namespace: `Assimalign.Cohesion.Database.Protocol`.

## Signature

```csharp
public sealed class ProtocolChannel : IAsyncDisposable
{
    public ProtocolChannel(Stream stream, ProtocolMessageFamily family, bool leaveOpen = false);
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

Permanently binds a stream to one family. Negotiate selects the local/peer intersection and applies it once; SetAgreement applies a validated Ready once on the client. A repeated, unsupported or invalid agreement throws ProtocolException. Gated reads/writes, including Diagnostics, fail until agreed. Null stream/family throws ArgumentNullException. Dispose closes the stream unless leaveOpen. No family switching, pipelining or model payload dispatch is added.

## Example

```csharp
var family = new ProtocolMessageFamily("Example", [ProtocolCapability.Diagnostics], 64);
await using var channel = new ProtocolChannel(new MemoryStream(), family);
var agreement = channel.Negotiate(ProtocolVersion.Current, ["diagnostics"]);
Console.WriteLine(agreement.Capabilities[0]); // diagnostics
```

## Related reference

[Assembly](index.md) · [Negotiation and wire ordering](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/src/ProtocolChannel.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
