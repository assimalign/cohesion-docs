# Assimalign.Cohesion.Connections.InMemory

Provides socketless stream and multiplexed connections for live protocol tests.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Purpose

The in-memory driver for the Cohesion Connections contracts. It produces cross-wired
duplex-pipe connection pairs — a listener plus a dial factory, and a multiplexed variant —
so application protocols can be exercised over a live, socketless byte channel. It is the
shared foundation for deterministic transport testing across the repo, replacing the
divergent private pipe-pair connection doubles the transport test projects previously each
maintained.

## Scope

- `InMemoryConnectionPair.Create(...)` — the primitive: a connected pair of `DuplexConnection`
  ends whose duplex pipes are cross-wired, supporting live multi-round-trip exchange.
- `InMemoryConnectionListener` / `InMemoryConnectionFactory` — the dial-connected
  server/client pair: the factory dials the listener, the listener accepts the server end.
- `InMemoryMultiplexedConnectionPair.Create(...)`,
  `InMemoryMultiplexedConnectionListener` / `InMemoryMultiplexedConnectionFactory` — the
  multiplexed variant for h2/h3-shaped stream tests, where each opened stream is itself an
  in-memory `MultiplexedConnectionStream` with whole-stream `Abort(code, reason)` and `IsInputCompleted`.
  It can also abandon either direction with an application error code
  (`stream.DirectionalAbort`, from `Connections.StreamAbort`); the other end observes the code on a
  `ConnectionResetException`. Both stream ends register that feature; byte-stream pairs have no directional-abort feature.
- `InMemoryEndPoint` — a name-addressed `EndPoint` for the socketless transport.

Closing or aborting either multiplexed parent notifies both ends and completes both accept loops.
Active streams fail with `ConnectionResetException.ApplicationErrorCode`: the selected close code,
or 0 for disposal and no-code abort. The first pair close wins.

Protocol semantics belong to the layers above; this driver only moves bytes.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.StreamAbort`](../assimalign-cohesion-connections-streamabort/index.md) | `CohesionProjectReference` |

- `Assimalign.Cohesion.Connections` — the contracts this driver implements (`DuplexConnection`,
  `DuplexConnectionListener`, `DuplexConnectionFactory`, and the multiplexed counterparts).
- `Assimalign.Cohesion.Core`.
- `System.IO.Pipelines` and `System.Threading.Channels` from the shared framework. No
  operating-system socket, no reflection, no third-party packages.

## Usage

```csharp
// A connected pair, driven directly.
(DuplexConnection client, DuplexConnection server) = InMemoryConnectionPair.Create();
await client.Output.WriteAsync(request);
ReadResult received = await server.Input.ReadAsync();

// Or the dial-connected listener/factory pair (the shape a server accept loop consumes).
await using InMemoryConnectionListener listener = new();
InMemoryConnectionFactory factory = listener.CreateFactory();

ValueTask<IDuplexConnection> accept = listener.AcceptAsync(cancellationToken);
IDuplexConnection dialed = await factory.ConnectAsync(listener.EndPoint, cancellationToken);
IDuplexConnection accepted = await accept;
```

See [Design](design.md) for the pair wiring and the close/dispose/abort semantics.

The driver participates in the common `IConnection` identity, feature, and lifecycle hierarchy.
Listeners expose a stable `Id` and live `Connections`. Entries disappear on closure; disposing
the listener leaves accepted connections open for their owners to dispose. `ConnectionClosed`
fires on abort, observed peer closure, and no later than local disposal.

## Principal public types

| Type | Source file |
|---|---|
| `InMemoryConnectionFactory` | `src/InMemoryConnectionFactory.cs` |
| `InMemoryConnectionListener` | `src/InMemoryConnectionListener.cs` |
| `InMemoryConnectionPair` | `src/InMemoryConnectionPair.cs` |
| `InMemoryEndPoint` | `src/InMemoryEndPoint.cs` |
| `InMemoryMultiplexedConnectionFactory` | `src/InMemoryMultiplexedConnectionFactory.cs` |
| `InMemoryMultiplexedConnectionListener` | `src/InMemoryMultiplexedConnectionListener.cs` |
| `InMemoryMultiplexedConnectionPair` | `src/InMemoryMultiplexedConnectionPair.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryConnectionPair.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryEndPoint.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/InMemoryMultiplexedConnectionPair.cs`.
