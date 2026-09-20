# Assimalign.Cohesion.Resilience.CircuitBreaker design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.CircuitBreaker`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`UseCircuitBreaker` configures a failure threshold, break duration, time provider, and
state-transition callbacks. Handled failures open the circuit; a later probe can recover it. The
current implementation and tests supersede the placeholder description in the older overview.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/Assimalign.Cohesion.Resilience.CircuitBreaker.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/tests`.
