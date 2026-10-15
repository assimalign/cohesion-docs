# Assimalign.Cohesion.Amqp.Connections

Negotiates AMQP connections and encodes protocol frames and messages over Cohesion carriers.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Amqp](../index.md)

## Scope

The AMQP 1.0 transport layer for Cohesion: protocol-header negotiation, frame and message
codecs for the AMQP and SASL protocol phases, and server/client transports that establish AMQP
connections over carrier connections produced by the `Assimalign.Cohesion.Connections`
contracts. Carriers are selected by capability (a reliable, ordered byte stream), never by
protocol identity; both single-stream and multiplexed carriers are supported. Session, link,
and messaging semantics are out of scope — they belong to the sibling
`Assimalign.Cohesion.Amqp` library.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Key Types

- `AmqpServerTransport` — accepts AMQP connections over an `IDuplexConnectionListener` or
  `IMultiplexedConnectionListener`; owns and disposes the listener.
- `AmqpClientTransport` — establishes AMQP connections to an `EndPoint` over an
  `IDuplexConnectionFactory` or `IMultiplexedConnectionFactory`.
- `IAmqpConnection` / `AmqpConnection` — an AMQP connection over a carrier; `Open()` /
  `OpenAsync()` yield the (cached) connection context.
- `IAmqpConnectionContext` / `AmqpConnectionContext` — the opened context; **is** an
  `IDuplexPipe`, with `AsStream()`, `NegotiateAsync` / `SwitchProtocolAsync`, and
  `SendAsync` / `ReceiveAsync` for frames.
- `AmqpTransportOptions` — initial protocol header, auto-negotiation toggle, max frame size.
- `AmqpProtocolHeader`, `AmqpProtocolId`, `AmqpFrame`, `AmqpFrameType` — the wire-level value
  types.
- `AmqpPerformative` family — `AmqpOpenPerformative`, `AmqpBeginPerformative`,
  `AmqpAttachPerformative`, `AmqpFlowPerformative`, `AmqpTransferPerformative`,
  `AmqpDispositionPerformative`, `AmqpDetachPerformative`, `AmqpEndPerformative`,
  `AmqpClosePerformative`, and the SASL performatives.
- `AmqpMessage`, `AmqpSource`, `AmqpTarget`, `AmqpError`, `AmqpSymbol`,
  `AmqpDescribedValue` — the AMQP 1.0 data model.
- `AmqpProtocolCodec`, `AmqpFrameCodec`, `AmqpMessageCodec` — static encode/decode facades.
- `AmqpProtocolException` — the area exception root.

## Source Layout

- `src/` — transports, connection/context guided bases, performative and message DTOs, value
  types, and the public codec facades.
- `src/Abstractions` — `IAmqpConnection`, `IAmqpConnectionContext`.
- `src/Exceptions` — `AmqpProtocolException`.
- `src/Extensions` — carrier capability gating (`ThrowIfNotAmqpCarrier`).
- `src/Internal` — the concrete carrier connections (single-stream, multiplexed), the
  connection context, and the `AmqpEncoding` wire toolbox.

## Principal public types

| Type | Source file |
|---|---|
| `AmqpAttachPerformative` | `src/AmqpAttachPerformative.cs` |
| `AmqpBeginPerformative` | `src/AmqpBeginPerformative.cs` |
| `AmqpClientTransport` | `src/AmqpClientTransport.cs` |
| `AmqpClosePerformative` | `src/AmqpClosePerformative.cs` |
| `AmqpConnection` | `src/AmqpConnection.cs` |
| `AmqpConnectionContext` | `src/AmqpConnectionContext.cs` |
| `AmqpDescribedValue` | `src/AmqpDescribedValue.cs` |
| `AmqpDetachPerformative` | `src/AmqpDetachPerformative.cs` |
| `AmqpDispositionPerformative` | `src/AmqpDispositionPerformative.cs` |
| `AmqpEndPerformative` | `src/AmqpEndPerformative.cs` |
| `AmqpError` | `src/AmqpError.cs` |
| `AmqpFlowPerformative` | `src/AmqpFlowPerformative.cs` |
| `AmqpFrame` | `src/AmqpFrame.cs` |
| `AmqpFrameCodec` | `src/AmqpFrameCodec.cs` |
| `AmqpFrameType` | `src/AmqpFrameType.cs` |
| `AmqpMessage` | `src/AmqpMessage.cs` |

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/Assimalign.Cohesion.Amqp.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpAttachPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpBeginPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpClientTransport.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpClosePerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpConnection.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpConnectionContext.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpDescribedValue.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpDetachPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpDispositionPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpEndPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpError.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpFlowPerformative.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpFrame.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpFrameCodec.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpFrameType.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpMessage.cs`.
