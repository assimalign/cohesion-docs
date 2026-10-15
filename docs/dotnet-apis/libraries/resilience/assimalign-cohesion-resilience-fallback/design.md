# Assimalign.Cohesion.Resilience.Fallback design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.Fallback`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`UseFallback` supports both result and non-result pipelines. The predicate selects failed outcomes,
and the configured action handles recovery; cancellation is excluded by the default predicate. The
implementation and tests supersede the older placeholder overview.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/Assimalign.Cohesion.Resilience.Fallback.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests`.
