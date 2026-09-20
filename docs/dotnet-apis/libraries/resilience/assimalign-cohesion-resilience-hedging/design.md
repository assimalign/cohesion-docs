# Assimalign.Cohesion.Resilience.Hedging design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.Hedging`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`UseHedging` configures delay, attempt count, predicates, and notifications. `MaxHedgedAttempts`
includes the primary attempt. Tests exercise both asynchronous winner selection and synchronous
recovery; the older placeholder evaluation no longer describes the code.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/Assimalign.Cohesion.Resilience.Hedging.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/tests`.
