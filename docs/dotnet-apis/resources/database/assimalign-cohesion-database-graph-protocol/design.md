# Graph Protocol design

The Protocol package owns the contracts exchanged by the Graph engine and remote client.

> **Status:** Implemented.

## Ownership and dependencies

The engine and typed client each reference `Assimalign.Cohesion.Database.Graph.Protocol`. The client also references
shared Database.Client, Protocol, Types and Connections. The Protocol package
references shared Database.Protocol and Database.Types; it references no engine or area root.

Messages, codecs, transferred values and the immutable family singleton have one owner. Engine
requests, planners, execution, Catalog, Storage and server lifetime stay in the model engine.

GraphValueCodec is the sole public codec facade. Graph and Graph.Protocol compile the stateless
GraphValueEncoding helper with local resource accessors. Successful codec calls perform no resource
lookups; error paths resolve messages under the culture active at failure. Mutable state, family
singletons and event sources are not linked between assemblies.

## Migration

All 13 types listed in the [assembly index](index.md) moved from the engine to this package.
Their namespace is now `Assimalign.Cohesion.Database.Graph.Protocol`; both assembly and package names match.
Replace old imports for those types, update package references and recompile consumers.
There are no type forwarders or duplicate DTOs. SQL and KeyValuePair each have a
ProtocolExecuteMessage, ProtocolResultHeaderMessage and ProtocolResultCompleteMessage; use namespace
aliases when consuming both families.

The extraction preserves wire bytes, limits, malformed-payload behavior and disk formats. Explicit
span-based codecs retain trimming and NativeAOT support. Codec tests live in the Protocol project;
engine, server and client integration tests remain with their respective projects.

[Assembly](index.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/Assimalign.Cohesion.Database.Graph.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
