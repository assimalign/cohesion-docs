# KeyValuePair Protocol design

The Protocol package owns the contracts exchanged by the KeyValuePair engine and remote client.

> **Status:** Implemented.

## Ownership and dependencies

The engine and typed client each reference `Assimalign.Cohesion.Database.KeyValuePair.Protocol`. The client also references
shared Database.Client, Protocol, Types and Connections. The Protocol package
references shared Database.Protocol; it references no engine or area root.

Messages, codecs, transferred values and the immutable family singleton have one owner. Engine
requests, planners, execution, Catalog, Storage and server lifetime stay in the model engine.

## Migration

All 5 types listed in the [assembly index](index.md) moved from the engine to this package.
Their namespace is now `Assimalign.Cohesion.Database.KeyValuePair.Protocol`; both assembly and package names match.
Replace old imports for those types, update package references and recompile consumers.
There are no type forwarders or duplicate DTOs. SQL and KeyValuePair each have a
ProtocolExecuteMessage, ProtocolResultHeaderMessage and ProtocolResultCompleteMessage; use namespace
aliases when consuming both families.

The extraction preserves wire bytes, limits, malformed-payload behavior and disk formats. Explicit
span-based codecs retain trimming and NativeAOT support. Codec tests live in the Protocol project;
engine, server and client integration tests remain with their respective projects.

[Assembly](index.md) · [Examples](examples/index.md)

## Terminal shared errors

The KeyValue server uses one ShutdownDrainTimeout deadline for terminal Error write, flush,
output completion and peer-input drain. TerminalDrainLinger starts after output completion and
bounds peer close. Capacity refusals follow the same rule. Each frame crosses the Stream boundary
in one complete write; explicit flush does not promise batching when the stream flushes on write.
The client consumes queued terminal Errors before sending its next request and discards the
connector. TLS terminal delivery remains a known gap (#1086). See
[shared framing and close contract](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)
and [server options](../assimalign-cohesion-database-keyvaluepair/key-value-database-server-options.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/Assimalign.Cohesion.Database.KeyValuePair.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
