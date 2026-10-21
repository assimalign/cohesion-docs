# Documents Protocol design

The Protocol package owns the contracts exchanged by the Documents engine and remote client.

> **Status:** Implemented.

## Ownership and dependencies

The engine and typed client each reference `Assimalign.Cohesion.Database.Documents.Protocol`. The client also references
shared Database.Client, Protocol, Types and Connections, plus model-neutral Execution. The Protocol package
references shared Database.Protocol and Database.Types; it references no engine or area root.

Messages, codecs, transferred values and the immutable family singleton have one owner. Engine
requests, planners, execution, Catalog, Storage and server lifetime stay in the model engine.

Internal DocumentProtocolJson and DocumentProtocolWire helpers live with the codecs. They retain
strict UTF-8 parsing, complete JSON value validation and payload bounds.

## Migration

All 14 types listed in the [assembly index](index.md) moved from the engine to this package.
Their namespace is now `Assimalign.Cohesion.Database.Documents.Protocol`; both assembly and package names match.
Replace old imports for those types, update package references and recompile consumers.
There are no type forwarders or duplicate DTOs. SQL and KeyValuePair each have a
ProtocolExecuteMessage, ProtocolResultHeaderMessage and ProtocolResultCompleteMessage; use namespace
aliases when consuming both families.

The extraction preserves wire bytes, limits, malformed-payload behavior and disk formats. Explicit
span-based codecs retain trimming and NativeAOT support. Codec tests live in the Protocol project;
engine, server and client integration tests remain with their respective projects.

[Assembly](index.md) · [Examples](examples/index.md)

## Terminal shared errors

The Document server uses one ShutdownDrainTimeout deadline for terminal Error write, flush,
output completion and peer-input drain. TerminalDrainLinger starts after output completion and
bounds peer close. Capacity refusals follow the same rule. Each frame crosses the Stream boundary
in one complete write; explicit flush does not promise batching when the stream flushes on write.
The client consumes queued terminal Errors before sending its next request and discards the
connector. TLS terminal delivery remains a known gap (#1086). See
[shared framing and close contract](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)
and [server options](../assimalign-cohesion-database-documents/document-database-server-options.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/Assimalign.Cohesion.Database.Documents.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
