# Assimalign.Cohesion.Amqp.Connections

Establishes AMQP 1.0 connections over Cohesion carriers and encodes their frames and messages.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Amqp](../index.md)

## Scope

AMQP 1.0 connections for Cohesion, at the specification's transport layer (Part 2): protocol-header
negotiation, frame and message codecs for the AMQP and SASL protocol phases, and the listener and
factory that establish AMQP connections over carrier connections produced by the
`Assimalign.Cohesion.Connections` contracts. Carriers are chosen by capability, a reliable and
ordered byte stream, never by protocol identity; single-stream and multiplexed carriers are both
supported. Session, link and messaging semantics are out of scope: they belong to the sibling
[`Assimalign.Cohesion.Amqp`](../assimalign-cohesion-amqp/index.md) library.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Key Types

- `AmqpConnectionListener` (`IAmqpConnectionListener`) — accepts AMQP connections over an
  `IDuplexConnectionListener` or `IMultiplexedConnectionListener`, and returns each one only once
  it is established. It owns the carrier listener, and a connection whose establishment fails or
  times out is dropped without stopping the listener.
- `AmqpConnectionFactory` (`IAmqpConnectionFactory`) — connects AMQP connections over an
  `IDuplexConnectionFactory` or `IMultiplexedConnectionFactory`, one new carrier per
  `ConnectAsync(EndPoint)` call.
- `IAmqpConnection` / `AmqpConnection` — an established AMQP connection. It is an
  `IDuplexConnection` (the carrier's identity, endpoints, capabilities, features, lifetime and
  pipe) with the protocol headers (`LocalProtocolHeader`, `RemoteProtocolHeader`,
  `NegotiateAsync`, `SwitchProtocolAsync`) and frames (`SendAsync`, `ReceiveAsync`).
- `AmqpConnectionListenerOptions` / `AmqpConnectionFactoryOptions` — the initial protocol header,
  whether establishment exchanges it, the maximum frame size, the handshake timeout, and, on the
  listener, how many establishments may run at once.
- `AmqpProtocolHeader`, `AmqpProtocolId`, `AmqpFrame`, `AmqpFrameType` — the wire-level value
  types.
- `AmqpPerformative` family — `AmqpOpenPerformative`, `AmqpBeginPerformative`,
  `AmqpAttachPerformative`, `AmqpFlowPerformative`, `AmqpTransferPerformative`,
  `AmqpDispositionPerformative`, `AmqpDetachPerformative`, `AmqpEndPerformative`,
  `AmqpClosePerformative`, and the SASL performatives.
- `AmqpMessage`, `AmqpSource`, `AmqpTarget`, `AmqpError`, `AmqpSymbol`, `AmqpDescribedValue`, and
  the settle-mode and SASL-code enums — the AMQP 1.0 data model.
- `AmqpProtocolCodec`, `AmqpFrameCodec`, `AmqpMessageCodec` — static encode and decode facades.
- `AmqpProtocolException` — the area exception root.

## Usage

A listener and a factory return connections that are ready to use. With the default options the
protocol headers have already been exchanged, so frames flow at once:

```csharp
using System.Net;

using Assimalign.Cohesion.Amqp.Connections;
using Assimalign.Cohesion.Connections.Tcp;

// Server: the AMQP listener takes ownership of the TCP listener.
await using AmqpConnectionListener listener = AmqpConnectionListener.Create(
    TcpConnectionListener.Create(options => options.EndPoint = new IPEndPoint(IPAddress.Any, 5672)));
await listener.BindAsync(cancellationToken);

await using IAmqpConnection server = await listener.AcceptAsync(cancellationToken);
await foreach (AmqpFrame frame in server.ReceiveAsync(cancellationToken))
{
    // Hand each frame to the connection, session and link layer.
}

// Client: every ConnectAsync call connects a new carrier.
AmqpConnectionFactory factory = AmqpConnectionFactory.Create(new TcpConnectionFactory());
await using IAmqpConnection client = await factory.ConnectAsync(
    new IPEndPoint(IPAddress.Loopback, 5672),
    cancellationToken);
await client.SendAsync(new AmqpFrame(0, new AmqpOpenPerformative { ContainerId = "client" }), cancellationToken);
```

Start in the SASL phase with `InitialProtocolHeader = AmqpProtocolHeader.Sasl10`, exchange the
SASL frames, then move to AMQP with `SwitchProtocolAsync(AmqpProtocolHeader.Amqp10)`. With
`AutoNegotiateProtocolHeader = false`, a returned connection has exchanged no bytes, and its owner
calls `NegotiateAsync` before using frames.

For TLS, compose the TLS layer on the carrier, below AMQP:
`AmqpConnectionListener.Create(tcpListener.UseTls(tlsServerOptions))` and
`AmqpConnectionFactory.Create(tcpFactory.UseTls(tlsClientOptions))`, with `UseTls` from
[`Assimalign.Cohesion.Connections.Security`](../../connections/assimalign-cohesion-connections-security/index.md).
The TLS facts are then on the AMQP connection's features.

Disposing the listener disposes the carrier listener and any connection still being established,
but not the connections it already returned: their owners dispose them. See [Design](design.md)
for establishment, containment, protocol phases and ownership.

## Validated values

Construct an `AmqpSymbol` from non-null seven-bit ASCII text; an empty symbol is allowed and
letter case is significant. Use `AmqpSymbol.TryCreate(text, out var symbol)` when invalid input is
expected. A default symbol has no value and can be compared or used as a dictionary key, but cannot
be encoded as a symbol. Its `ToString()` returns an empty string.

Set the listener, factory and Open maximum frame sizes to at least 512 bytes. The standalone frame
encoder enforces the same minimum before encoding. `AmqpOpenPerformative.MaxFrameSize = null`
omits the advertised limit. Invalid assignments and encoder limits throw
`ArgumentOutOfRangeException`; invalid peer symbols and advertised limits throw
`AmqpProtocolException` during decoding.

The symbol's generated JSON converter writes a JSON string (or null for default), including when
used with source-generated JSON serialization. See
[Symbol and frame-size validation](design.md#symbol-and-frame-size-validation-1484) for the
behavior changes from 10.0.0-preview.1.

## Source Layout

- `src/` — `AmqpConnectionListener`, `AmqpConnectionFactory` and their options, the
  `AmqpConnection` guided base, the performative and message DTOs, and the public codec facades.
- `src/Abstractions` — `IAmqpConnection`, `IAmqpConnectionListener`, `IAmqpConnectionFactory`.
- `src/Exceptions` — `AmqpProtocolException`.
- `src/ValueObjects` — `AmqpProtocolHeader`, `AmqpSymbol`.
- `src/Internal` — the establishment layer (`AmqpConnectionLayer`), the connection implementation
  (`AmqpCarrierConnection`), the multiplexed carrier adapters, the settings read from the options,
  the `AmqpEncoding` wire toolbox, and carrier capability gating (`Internal/Extensions`).
- `src/Properties` — `ErrorMessages.resx`, the package's exception messages.

## Principal public types

| Type | Source file |
|---|---|
| `AmqpAttachPerformative` | `src/AmqpAttachPerformative.cs` |
| `AmqpBeginPerformative` | `src/AmqpBeginPerformative.cs` |
| `AmqpClosePerformative` | `src/AmqpClosePerformative.cs` |
| `AmqpConnection` | `src/AmqpConnection.cs` |
| `AmqpConnectionFactory` | `src/AmqpConnectionFactory.cs` |
| `AmqpConnectionFactoryOptions` | `src/AmqpConnectionFactoryOptions.cs` |
| `AmqpConnectionListener` | `src/AmqpConnectionListener.cs` |
| `AmqpConnectionListenerOptions` | `src/AmqpConnectionListenerOptions.cs` |
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
| `AmqpMessageCodec` | `src/AmqpMessageCodec.cs` |
| `AmqpOpenPerformative` | `src/AmqpOpenPerformative.cs` |
| `AmqpPerformative` | `src/AmqpPerformative.cs` |
| `AmqpProtocolCodec` | `src/AmqpProtocolCodec.cs` |
| `AmqpProtocolException` | `src/Exceptions/AmqpProtocolException.cs` |
| `AmqpProtocolHeader` | `src/ValueObjects/AmqpProtocolHeader.cs` |
| `AmqpProtocolId` | `src/AmqpProtocolId.cs` |
| `AmqpReceiverSettleMode` | `src/AmqpReceiverSettleMode.cs` |
| `AmqpSaslChallengePerformative` | `src/AmqpSaslChallengePerformative.cs` |
| `AmqpSaslCode` | `src/AmqpSaslCode.cs` |
| `AmqpSaslInitPerformative` | `src/AmqpSaslInitPerformative.cs` |
| `AmqpSaslMechanismsPerformative` | `src/AmqpSaslMechanismsPerformative.cs` |
| `AmqpSaslOutcomePerformative` | `src/AmqpSaslOutcomePerformative.cs` |
| `AmqpSaslResponsePerformative` | `src/AmqpSaslResponsePerformative.cs` |
| `AmqpSenderSettleMode` | `src/AmqpSenderSettleMode.cs` |
| `AmqpSource` | `src/AmqpSource.cs` |
| `AmqpSymbol` | `src/ValueObjects/AmqpSymbol.cs` |
| `AmqpTarget` | `src/AmqpTarget.cs` |
| `AmqpTransferPerformative` | `src/AmqpTransferPerformative.cs` |
| `IAmqpConnection` | `src/Abstractions/IAmqpConnection.cs` |
| `IAmqpConnectionFactory` | `src/Abstractions/IAmqpConnectionFactory.cs` |
| `IAmqpConnectionListener` | `src/Abstractions/IAmqpConnectionListener.cs` |

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/Assimalign.Cohesion.Amqp.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/README.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpConnectionListener.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/AmqpConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/Abstractions/IAmqpConnection.cs`.
