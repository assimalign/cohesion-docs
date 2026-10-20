# Sql Protocol examples

Encode and decode Sql payloads using the contract package.

> **Status:** Implemented.

Reference `Assimalign.Cohesion.Database.Sql.Protocol` for this example. No engine is required.

```csharp
using System;

using Assimalign.Cohesion.Database.Sql.Protocol;

var message = ProtocolExecuteMessage.Create("SELECT 1");
byte[] payload = message.Encode();
var decoded = ProtocolExecuteMessage.Decode(payload);
Console.WriteLine(decoded.Statement); // SELECT 1
```

The codec produces a payload; the shared ProtocolChannel supplies framing and the immutable model
family binding. Embedded and server applications reference the engine separately.

[Assembly](../index.md) · [Design](../design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/ProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
