# Assimalign.Cohesion.Connections design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A stream connection is itself an `IDuplexPipe`, with input and output anchored to its holder.
Listeners bind explicitly and factories establish outbound connections. Capabilities describe
delivery and security; connection layers transform connections at establishment without introducing
a parallel transport abstraction.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Assimalign.Cohesion.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src`.
