# Sql Protocol design

The Protocol package owns the contracts exchanged by the Sql engine and remote client.

> **Status:** Implemented.

## Ownership and dependencies

The engine and typed client each reference `Assimalign.Cohesion.Database.Sql.Protocol`. The client also references
shared Database.Client, Protocol, Types and Connections. The Protocol package
references shared Database.Protocol; it references no engine or area root.

Messages, codecs, transferred values and the immutable family singleton have one owner. Engine
requests, planners, execution, Catalog, Storage and server lifetime stay in the model engine.

## Migration

All 5 types listed in the [assembly index](index.md) moved from the engine to this package.
Their namespace is now `Assimalign.Cohesion.Database.Sql.Protocol`; both assembly and package names match.
Replace old imports for those types, update package references and recompile consumers.
There are no type forwarders or duplicate DTOs. SQL and KeyValuePair each have a
ProtocolExecuteMessage, ProtocolResultHeaderMessage and ProtocolResultCompleteMessage; use namespace
aliases when consuming both families.

The extraction preserves wire bytes, limits, malformed-payload behavior and disk formats. Explicit
span-based codecs retain trimming and NativeAOT support. Codec tests live in the Protocol project;
engine, server and client integration tests remain with their respective projects.

[Assembly](index.md) · [Examples](examples/index.md)

## Statement results

The shared handshake fixes SqlProtocol.Family for the connection. The client sends one Execute
at a time; there is no pipelining or multiplexing. A successful row result sends ResultHeader,
zero or more ResultRow frames, then ResultComplete. A non-row result sends ResultComplete alone.
Error ends the exchange without ResultComplete. Negotiated Diagnostics immediately precedes
Error or successful completion.

| Path | Encoding and failure boundary |
| --- | --- |
| Writing statement | Prepare all header/row payloads before commit or explicit statement completion; DML enumeration/disposal runs inside its undo bracket. A failure precedes result frames and rolls back only the statement. DDL/control prepare fixed completion before execution. |
| Read-only SELECT, EXPLAIN, system view or SHOW | Encode during delivery after materialization. ExecutionFailure may end a partial result; the client discards all partial columns/rows. |

Each header and row has a **16 MiB (16,777,216-byte)** payload limit. There is no aggregate
encoded-result cap. Uncoded result failures and size refusals carry COHSQLE045; existing coded
failures retain their diagnostics. Size refusals map to SQLSTATE 54000, reporting encoded size
and frame limit. SQLSTATE adds no wire field. Writes retain prepared payloads until delivery;
reads retain only the current encoded frame alongside their materialized result.

ParseFailure before results and ExecutionFailure at either result phase leave the session ready
and any explicit transaction usable on a result failure. ParseFailure after result frames does
not certify a reusable response. Malformed payloads, invalid ordering and terminal errors discard
the connection. A transport failure during delivery is terminal even after a write committed;
it is never converted to an ordinary statement failure. The wire format and public codecs are unchanged.

See [result diagnostics](../../../../database/sql/language/diagnostics.md#result-encoding-and-delivery)
and the [client example](../assimalign-cohesion-database-sql-client/sql-connection.md#result-failure-example).

## Terminal shared errors

The Sql server uses one ShutdownDrainTimeout deadline for terminal Error write, flush,
output completion and peer-input drain. TerminalDrainLinger starts after output completion and
bounds peer close. Capacity refusals follow the same rule. Each frame crosses the Stream boundary
in one complete write; explicit flush does not promise batching when the stream flushes on write.
The client consumes queued terminal Errors before sending its next request and discards the
connector. TLS terminal delivery remains a known gap (#1086). See
[shared framing and close contract](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)
and [server options](../assimalign-cohesion-database-sql/sql-database-server-options.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlResultDeliveryConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlResultDeliveryWireConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/Assimalign.Cohesion.Database.Sql.Protocol.csproj`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
