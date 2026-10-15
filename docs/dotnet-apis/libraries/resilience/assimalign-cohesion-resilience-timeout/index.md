# Assimalign.Cohesion.Resilience.Timeout

Adds fixed and dynamically selected timeout strategies.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

Timeout options own duration selection and builder extensions attach the strategy.
`TimeoutRejectedException` identifies the policy failure boundary. Current behavioral tests exercise
timeout rejection and successful completion.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OnTimeoutArguments` | `src/OnTimeoutArguments.cs` |
| `TimeoutGeneratorArguments` | `src/TimeoutGeneratorArguments.cs` |
| `TimeoutRejectedException` | `src/Exception/TimeoutRejectedException.cs` |
| `TimeoutResilienceExtensions` | `src/Extensions/TimeoutResilienceExtensions.cs` |
| `TimeoutStrategyOptions` | `src/TimeoutStrategyOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Assimalign.Cohesion.Resilience.Timeout.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/OnTimeoutArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/TimeoutGeneratorArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Exception/TimeoutRejectedException.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Extensions/TimeoutResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/TimeoutStrategyOptions.cs`.
