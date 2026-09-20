# Assimalign.Cohesion.Connections.NamedPipes design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.NamedPipes`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The listener binds a pipe name before accepting clients, and the factory dials a
`NamedPipeEndPoint`. Access control is chosen through listener options. The driver exposes the same
connection capabilities used by socket listeners, so protocol layers need no named-pipe-specific
branch.

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
