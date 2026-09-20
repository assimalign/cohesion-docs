# Assimalign.Cohesion.Http.Connections design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Connections`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The package consumes `IConnection` and `IMultiplexedConnection` rather than owning a second
transport stack. Protocol handling uses the core feature and interceptor seams, allowing optional
concerns to attach without reverse references from the transport.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Connections`, `Assimalign.Cohesion.Http`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Assimalign.Cohesion.Http.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src`.
