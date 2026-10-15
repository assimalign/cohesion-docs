# Assimalign.Cohesion.Resilience

Builds execution pipelines from ordered resilience strategies.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Resilience](../index.md)

## Scope

The core composes callbacks and outcomes without embedding individual policy behavior. Strategies
wrap execution from the outside in. Concrete policies extend builders from sibling packages, while
the current core remains transitional.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.ObjectPool`](../../object-pool/assimalign-cohesion-objectpool/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IResilienceContext` | `src/Abstractions/IResilienceContext.cs` |
| `Outcome` | `src/ValueTypes/Outcome.cs` |
| `Outcome<TResult>` | `src/ValueTypes/Outcome.TResult.cs` |
| `ResiliencePipelineBuilder` | `src/ResiliencePipelineBuilder.cs` |
| `ResiliencePipelineBuilder<TResult>` | `src/ResiliencePipelineBuilder.TResult.cs` |
| `ExecutionRejectedException` | `src/Exceptions/ExecutionRejectedException.cs` |
| `IResiliencePipeline` | `src/Abstractions/IResiliencePipeline.cs` |
| `IResiliencePipeline<TResult>` | `src/Abstractions/IResiliencePipeline.TResult.cs` |
| `IResiliencePipelineBuilder` | `src/Abstractions/IResiliencePipelineBuilder.cs` |
| `IResiliencePipelineBuilder<TResult>` | `src/Abstractions/IResiliencePipelineBuilder.TResult.cs` |
| `IResilienceStrategy` | `src/Abstractions/IResilienceStrategy.cs` |
| `IResilienceStrategy<TResult>` | `src/Abstractions/IResilienceStrategy.TResult.cs` |
| `ResilienceErrorCode` | `src/Exceptions/ResilienceErrorCode.cs` |
| `ResilienceException` | `src/Exceptions/ResilienceException.cs` |
| `ResilienceExtensions` | `src/Extensions/ResilienceExtensions.Context.cs` |
| `ResiliencePipelineFailureException` | `src/Exceptions/ResiliencePipelineFailureException.cs` |

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Assimalign.Cohesion.Resilience.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResilienceContext.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/ValueTypes/Outcome.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/ValueTypes/Outcome.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/ResiliencePipelineBuilder.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/ResiliencePipelineBuilder.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Exceptions/ExecutionRejectedException.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResiliencePipeline.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResiliencePipeline.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResiliencePipelineBuilder.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResiliencePipelineBuilder.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResilienceStrategy.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Abstractions/IResilienceStrategy.TResult.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Exceptions/ResilienceErrorCode.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Exceptions/ResilienceException.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Extensions/ResilienceExtensions.Context.cs`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Exceptions/ResiliencePipelineFailureException.cs`.
