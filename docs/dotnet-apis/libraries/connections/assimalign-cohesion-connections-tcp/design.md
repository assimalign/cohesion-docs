# Assimalign.Cohesion.Connections.Tcp design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Tcp`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

One socket data path serves the supported endpoint forms. Binding is explicit, and Unix socket-file
lifetime belongs to the listener. Shared pipe plumbing is compiled from the connection contracts
project rather than exposed as driver-specific public infrastructure.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/Assimalign.Cohesion.Connections.Tcp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src`.
