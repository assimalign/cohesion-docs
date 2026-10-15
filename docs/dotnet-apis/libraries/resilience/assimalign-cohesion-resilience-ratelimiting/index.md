# Assimalign.Cohesion.Resilience.RateLimiting

Acquires rate-limiter permits before executing resilience callbacks.

> **Status:** Implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

`UseRateLimiter` adapts an explicitly supplied `RateLimiter`, permit count, and rejection callback.
Tests verify that unavailable permits reject execution and available permits allow results. The
current strategy supersedes the exploratory description in the older design.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |
| `System.Threading.RateLimiting` | `CohesionPackageReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OnRateLimiterRejectedArguments` | `src/OnRateLimiterRejectedArguments.cs` |
| `RateLimiterRejectedException` | `src/Exceptions/RateLimiterRejectedException.cs` |
| `RateLimiterResilienceExtensions` | `src/Extensions/RateLimiterResilienceExtensions.cs` |
| `RateLimiterStrategyOptions` | `src/RateLimiterStrategyOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/Assimalign.Cohesion.Resilience.RateLimiting.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/OnRateLimiterRejectedArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/Exceptions/RateLimiterRejectedException.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/Extensions/RateLimiterResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/RateLimiterStrategyOptions.cs`.
