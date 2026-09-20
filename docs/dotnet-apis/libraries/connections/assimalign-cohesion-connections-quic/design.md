# Assimalign.Cohesion.Connections.Quic design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Quic`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.
