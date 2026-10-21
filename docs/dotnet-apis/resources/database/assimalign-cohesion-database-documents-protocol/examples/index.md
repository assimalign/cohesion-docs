# Documents Protocol examples

Encode and decode Documents payloads using the contract package.

> **Status:** Implemented.

Reference `Assimalign.Cohesion.Database.Documents.Protocol` for this example. No engine is required.

```csharp
using System;

using Assimalign.Cohesion.Database.Documents.Protocol;

var message = DocumentProtocolExecuteMessage.Create("SELECT 1");
byte[] payload = message.Encode();
var decoded = DocumentProtocolExecuteMessage.Decode(payload);
Console.WriteLine(decoded.Statement); // SELECT 1
```

The codec produces a payload; the shared ProtocolChannel supplies framing and the immutable model
family binding. Embedded and server applications reference the engine separately.

[Assembly](../index.md) · [Design](../design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
