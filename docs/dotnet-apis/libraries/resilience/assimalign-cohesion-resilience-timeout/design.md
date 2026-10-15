# Assimalign.Cohesion.Resilience.Timeout design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.Timeout`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Timeout options own duration selection and builder extensions attach the strategy.
`TimeoutRejectedException` identifies the policy failure boundary. Current behavioral tests exercise
timeout rejection and successful completion.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Assimalign.Cohesion.Resilience.Timeout.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/tests`.
