# Assimalign.Cohesion.Amqp.Connections

Negotiates AMQP connections and encodes protocol frames and messages over Cohesion carriers.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Amqp](../index.md)

## Scope

A carrier must provide a reliable ordered byte stream; protocol identity does not select the
transport. Single-stream and multiplexed carriers share the AMQP connection surface. The server
transport owns its listener, while session, link, and messaging policy belong above this package.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

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
