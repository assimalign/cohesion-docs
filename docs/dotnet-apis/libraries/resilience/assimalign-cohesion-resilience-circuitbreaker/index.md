# Assimalign.Cohesion.Resilience.CircuitBreaker

Opens and recovers a circuit around failing resilience callbacks.

> **Status:** Implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

`UseCircuitBreaker` configures a failure threshold, break duration, time provider, and
state-transition callbacks. Handled failures open the circuit; a later probe can recover it. The
current implementation and tests supersede the placeholder description in the older overview.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `BrokenCircuitException` | `src/Exceptions/BrokenCircuitException.cs` |
| `CircuitBreakerPredicateArguments` | `src/CircuitBreakerPredicateArguments.cs` |
| `CircuitBreakerResilienceExtensions` | `src/Extensions/CircuitBreakerResilienceExtensions.cs` |
| `CircuitBreakerState` | `src/CircuitBreakerState.cs` |
| `CircuitBreakerStrategyOptions` | `src/CircuitBreakerStrategyOptions.cs` |
| `OnCircuitClosedArguments` | `src/OnCircuitClosedArguments.cs` |
| `OnCircuitHalfOpenedArguments` | `src/OnCircuitHalfOpenedArguments.cs` |
| `OnCircuitOpenedArguments` | `src/OnCircuitOpenedArguments.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/Assimalign.Cohesion.Resilience.CircuitBreaker.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/Exceptions/BrokenCircuitException.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/CircuitBreakerPredicateArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/Extensions/CircuitBreakerResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/CircuitBreakerState.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/CircuitBreakerStrategyOptions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/OnCircuitClosedArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/OnCircuitHalfOpenedArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/OnCircuitOpenedArguments.cs`.
