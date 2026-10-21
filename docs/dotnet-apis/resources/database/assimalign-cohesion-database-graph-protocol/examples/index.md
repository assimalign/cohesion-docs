# Graph Protocol examples

Encode and decode Graph payloads using the contract package.

> **Status:** Implemented.

Reference `Assimalign.Cohesion.Database.Graph.Protocol` for this example. No engine is required.

```csharp
using System;

using Assimalign.Cohesion.Database.Graph.Protocol;

var message = new GraphProtocolResultCompleteMessage(7);
byte[] payload = message.Encode();
var decoded = GraphProtocolResultCompleteMessage.Decode(payload);
Console.WriteLine(decoded.AffectedCount); // 7
```

The codec produces a payload; the shared ProtocolChannel supplies framing and the immutable model
family binding. Embedded and server applications reference the engine separately.

[Assembly](../index.md) · [Design](../design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/GraphProtocolResultCompleteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
