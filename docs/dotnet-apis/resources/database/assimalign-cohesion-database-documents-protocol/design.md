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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/Assimalign.Cohesion.Database.Documents.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
