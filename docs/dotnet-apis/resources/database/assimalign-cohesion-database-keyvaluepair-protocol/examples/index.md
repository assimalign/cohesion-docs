# KeyValuePair Protocol examples

Encode and decode KeyValuePair payloads using the contract package.

> **Status:** Implemented.

Reference `Assimalign.Cohesion.Database.KeyValuePair.Protocol` for this example. No engine is required.

```csharp
using System;

using Assimalign.Cohesion.Database.KeyValuePair.Protocol;

var message = ProtocolExecuteMessage.Create("GET k");
byte[] payload = message.Encode();
var decoded = ProtocolExecuteMessage.Decode(payload);
Console.WriteLine(decoded.Statement); // GET k
```

The codec produces a payload; the shared ProtocolChannel supplies framing and the immutable model
family binding. Embedded and server applications reference the engine separately.

[Assembly](../index.md) · [Design](../design.md)

## Source-backed tests

- [Protocol message tests](protocol-message-tests.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/ProtocolExecuteMessage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
