# Assimalign.Cohesion.Connections.NamedPipes design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.NamedPipes`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The listener binds a pipe name before accepting clients, and the factory dials a
`NamedPipeEndPoint`. Access control is chosen through listener options. The driver exposes the same
connection capabilities used by socket listeners, so protocol layers need no named-pipe-specific
branch.

## Diagnostics

The diagnostics tests link the canonical test-only `build/TestSupport/EventSourceRecorder.cs`
with MSBuild `Compile`/`Link` (see `.claude/rules/testing.md`); it records
events and counter publication without adding a shipped dependency.

The driver reports through its own internal event source, named for the assembly:
`Assimalign.Cohesion.Connections.NamedPipes` (`Internal/EventSource/NamedPipeConnectionEventSource.cs`),
per the repository EventSource convention (`cohesion/.claude/rules/event-source.md`). Tools enable it by name;
an application forwards it into its logging with `Assimalign.Cohesion.Logging.EventSource`, where the
source name becomes the log category.

| Id | Event | Level | Payload |
|---|---|---|---|
| 1 | `ListenerBound` | Informational | `listenerId`, `endPoint` |
| 2 | `ListenerClosed` | Informational | `listenerId` |
| 3 | `ConnectionOpened` | Informational | `connectionId`, `listenerId` (empty when dialed), `localEndPoint`, `remoteEndPoint` |
| 4 | `ConnectionClosed` | Informational | `connectionId` |

Counters: `current-connections`, `total-connections`, and `connections-per-second`.

The connection, not the listener, reports its own lifecycle, so dialed connections are counted too
(the listener used to report only accepted ones, and nothing reported a close). `Abort` and
`DisposeAsync` both end a connection; whichever runs first reports the close, once.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/Assimalign.Cohesion.Connections.NamedPipes.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src`.
