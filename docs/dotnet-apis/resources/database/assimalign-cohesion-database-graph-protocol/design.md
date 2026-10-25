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

## Path encoding and batching

[GraphProtocolPathMessage](graph-protocol-path-message.md) measures the complete path before
allocation and writes directly into a caller-owned Span<byte>. Encode() remains an owned-array
convenience. Scalar tags, integer bits, Decimal parts, Unicode strings, stored relationship
directions and repeated entities retain the same Path payload bytes. A payload above 16 MiB
is refused during measurement. There is no new capability, version or entity deduplication.

The server sends complete Path frames through pooled batches bounded by 64 KiB or 32 frames,
with cooperative latency and explicit result-boundary flushes. Large frames are written alone.
The client decodes each Path once into independently owned values through a one-slot queue.
See [server wire delivery](../assimalign-cohesion-database-graph/design.md#batched-path-delivery)
and [shared framing APIs](../assimalign-cohesion-database-protocol/design.md#bounded-batches-and-borrowed-payloads).

[Assembly](index.md) · [Examples](examples/index.md)

## Terminal shared errors

The Graph server uses one ShutdownDrainTimeout deadline for terminal Error write, flush,
output completion and peer-input drain. TerminalDrainLinger starts after output completion and
bounds peer close. Capacity refusals follow the same rule. Complete frames can share one bounded
Stream write; an oversized-for-batch frame is sent alone. A failed send faults the writer and
prevents later frames from being appended to an uncertain stream. A separate flush failure after
a successful write does not fault it. Explicit flushes retain the transport's own flush policy.
The client consumes queued terminal Errors before sending its next request and discards the
connector. TLS terminal delivery remains a known gap (#1086). See
[shared framing and close contract](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)
and [server options](../assimalign-cohesion-database-graph/graph-database-server-options.md).

## Execute transaction status

SQL and Graph negotiate transaction-status. After completed Execute output or statement failure, a one-byte Ready postamble reports Idle, Active or Failed. Clients drain it before completing exchanges and reset open/failed transactions on pool return. See [status codecs](../assimalign-cohesion-database-protocol/protocol-transaction-status-message.md).

## Sources

- **Path byte equivalence and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/tests/GraphPathEncodingTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/Assimalign.Cohesion.Database.Graph.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
