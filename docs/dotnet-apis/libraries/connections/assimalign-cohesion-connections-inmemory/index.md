# Assimalign.Cohesion.Connections.InMemory

Provides socketless stream and multiplexed connections for live protocol tests.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Paired pipes are cross-wired so both ends can exchange bytes repeatedly. Listener/factory pairs
preserve the production connection contracts while avoiding operating-system sockets. This driver
moves bytes; application protocol behavior belongs to its consumers.

An end of a multiplexed stream that aborts, or whose holder completes either pipe half with an error
(the in-memory `RESET_STREAM` and `STOP_SENDING`), fires the other end's `ConnectionClosed` at once,
as the QUIC driver does for a peer that abandons a stream (#1329). See the
[design](design.md#multiplexed-variant).

Each opened stream is an in-memory `MultiplexedStream` with whole-stream `Abort(code, reason)` and
`IsInputCompleted`. It can also abandon either direction with an application error code
(`stream.DirectionalAbort`, from
[Connections.StreamAbort](../assimalign-cohesion-connections-streamabort/index.md)); the other end
observes the code on a `ConnectionResetException`. Both stream ends register that feature;
byte-stream pairs have no directional-abort feature. See the
[design](design.md#multiplexed-variant).

Closing or aborting either multiplexed parent notifies both ends and completes both accept loops.
Active streams fail with `ConnectionResetException.ApplicationErrorCode`: the selected close code,
or 0 for disposal and no-code abort. The first pair close wins.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.StreamAbort`](../assimalign-cohesion-connections-streamabort/index.md) | `CohesionProjectReference` |

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
