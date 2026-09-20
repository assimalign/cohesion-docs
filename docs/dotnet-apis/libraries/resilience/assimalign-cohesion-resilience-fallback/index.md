# Assimalign.Cohesion.Resilience.Fallback

Runs a fallback action or produces a replacement value after a handled failure.

> **Status:** Implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

`UseFallback` supports both result and non-result pipelines. The predicate selects failed outcomes,
and the configured action handles recovery; cancellation is excluded by the default predicate. The
implementation and tests supersede the older placeholder overview.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `FallbackActionArguments` | `src/FallbackActionArguments.cs` |
| `FallbackActionArguments<TResult>` | `src/FallbackActionArguments.TResult.cs` |
| `FallbackPredicateArguments` | `src/FallbackPredicateArguments.cs` |
| `FallbackPredicateArguments<TResult>` | `src/FallbackPredicateArguments.TResult.cs` |
| `FallbackResilienceExtensions` | `src/Extensions/FallbackResilienceExtensions.cs` |
| `FallbackStrategyOptions` | `src/FallbackStrategyOptions.cs` |
| `FallbackStrategyOptions<TResult>` | `src/FallbackStrategyOptions.TResult.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/Assimalign.Cohesion.Resilience.Fallback.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackActionArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackActionArguments.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackPredicateArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackPredicateArguments.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/Extensions/FallbackResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackStrategyOptions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackStrategyOptions.TResult.cs`.
