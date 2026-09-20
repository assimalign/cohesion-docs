# Assimalign.Cohesion.Connections.InMemory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.InMemory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Paired pipes are cross-wired so both ends can exchange bytes repeatedly. Listener/factory pairs
preserve the production connection contracts while avoiding operating-system sockets. This driver
moves bytes; application protocol behavior belongs to its consumers.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.
