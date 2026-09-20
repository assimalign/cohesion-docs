# Assimalign.Cohesion.Http.ProtocolUpgrade design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ProtocolUpgrade`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The interceptor detects eligible requests and coordinates the transition response. Acceptance writes
the switching or tunnel response and transfers the raw duplex stream to the caller. The inner
protocol's lifetime then belongs to the accepting application.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`, `Assimalign.Cohesion.Http.Cookies`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Assimalign.Cohesion.Http.ProtocolUpgrade.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src`.
