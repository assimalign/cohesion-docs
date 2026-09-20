# Assimalign.Cohesion.Connections.NamedPipes

Carries ordered byte streams over named pipes for local inter-process communication.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

The listener binds a pipe name before accepting clients, and the factory dials a
`NamedPipeEndPoint`. Access control is chosen through listener options. The driver exposes the same
connection capabilities used by socket listeners, so protocol layers need no named-pipe-specific
branch.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `NamedPipeConnectionFactory` | `src/NamedPipeConnectionFactory.cs` |
| `NamedPipeConnectionFactoryOptions` | `src/NamedPipeConnectionFactoryOptions.cs` |
| `NamedPipeConnectionListener` | `src/NamedPipeConnectionListener.cs` |
| `NamedPipeConnectionListenerOptions` | `src/NamedPipeConnectionListenerOptions.cs` |
| `NamedPipeEndPoint` | `src/NamedPipeEndPoint.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/Assimalign.Cohesion.Connections.NamedPipes.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/NamedPipeConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/NamedPipeConnectionFactoryOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/NamedPipeConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/NamedPipeConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/NamedPipeEndPoint.cs`.
