# Assimalign.Cohesion.Resilience.Retry

Adds retry options, callbacks, and strategy composition to resilience pipelines.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

Retry configuration owns attempt count, delay, jitter, predicates, and callbacks. Builder extensions
support generic and non-generic pipelines. Internal strategies contain execution details so the
public surface remains focused on policy configuration.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Resilience`](../../resilience/assimalign-cohesion-resilience/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `DelayBackoffType` | `src/DelayBackoffType.cs` |
| `OnRetryArguments` | `src/OnRetryArguments.cs` |
| `OnRetryArguments<TResult>` | `src/OnRetryArguments.TResult.cs` |
| `RetryDelayGeneratorArguments` | `src/RetryDelayGeneratorArguments.cs` |
| `RetryDelayGeneratorArguments<TResult>` | `src/RetryDelayGeneratorArguments.TResult.cs` |
| `RetryPredicateArguments` | `src/RetryPredicateArguments.cs` |
| `RetryPredicateArguments<TResult>` | `src/RetryPredicateArguments.TResult.cs` |
| `RetryResilienceExtensions` | `src/Extensions/RetryResilienceExtensions.cs` |
| `RetryStrategyOptions` | `src/RetryStrategyOptions.cs` |
| `RetryStrategyOptions<TResult>` | `src/RetryStrategyOptions.TResult.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/Assimalign.Cohesion.Resilience.Retry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/DelayBackoffType.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/OnRetryArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/OnRetryArguments.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryDelayGeneratorArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryDelayGeneratorArguments.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryPredicateArguments.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryPredicateArguments.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/Extensions/RetryResilienceExtensions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryStrategyOptions.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/RetryStrategyOptions.TResult.cs`.
