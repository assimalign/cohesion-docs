# Assimalign.Cohesion.Hosting.Health design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting.Health`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Health contracts depend only on Core, not on plain Hosting or Hosting.Resources. A contributor
registry and resource-process supervision are outside this package. Shared-framework delivery makes
the contracts available without activating any runtime behavior.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/Assimalign.Cohesion.Hosting.Health.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src`.
