# Assimalign.Cohesion.Resilience.Hedging

Schedules additional attempts and selects successful resilience outcomes.

> **Status:** Implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

`UseHedging` configures delay, attempt count, predicates, and notifications. `MaxHedgedAttempts`
includes the primary attempt. Tests exercise both asynchronous winner selection and synchronous
recovery; the older placeholder evaluation no longer describes the code.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HedgingPredicateArguments` | `src/HedgingPredicateArguments.cs` |
| `HedgingResilienceExtensions` | `src/Extensions/HedgingResilienceExtensions.cs` |
| `HedgingStrategyOptions` | `src/HedgingStrategyOptions.cs` |
| `OnHedgingArguments` | `src/OnHedgingArguments.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/Assimalign.Cohesion.Resilience.Hedging.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/HedgingPredicateArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/Extensions/HedgingResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/HedgingStrategyOptions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/OnHedgingArguments.cs`.
