# Assimalign.Cohesion.Resilience.Retry design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.Retry`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Retry configuration owns attempt count, delay, jitter, predicates, and callbacks. Builder extensions
support generic and non-generic pipelines. Internal strategies contain execution details so the
public surface remains focused on policy configuration.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/Assimalign.Cohesion.Resilience.Retry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src`.
