# Resilience

Execution pipelines with retry, timeout, circuit breaker, fallback, hedging, and rate limiting.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Resilience` | Builds execution pipelines from ordered resilience strategies. | [Overview](assimalign-cohesion-resilience/index.md) |
| `Assimalign.Cohesion.Resilience.CircuitBreaker` | Opens and recovers a circuit around failing resilience callbacks. | [Overview](assimalign-cohesion-resilience-circuitbreaker/index.md) |
| `Assimalign.Cohesion.Resilience.Fallback` | Runs a fallback action or produces a replacement value after a handled failure. | [Overview](assimalign-cohesion-resilience-fallback/index.md) |
| `Assimalign.Cohesion.Resilience.Hedging` | Schedules additional attempts and selects successful resilience outcomes. | [Overview](assimalign-cohesion-resilience-hedging/index.md) |
| `Assimalign.Cohesion.Resilience.RateLimiting` | Acquires rate-limiter permits before executing resilience callbacks. | [Overview](assimalign-cohesion-resilience-ratelimiting/index.md) |
| `Assimalign.Cohesion.Resilience.Retry` | Adds retry options, callbacks, and strategy composition to resilience pipelines. | [Overview](assimalign-cohesion-resilience-retry/index.md) |
| `Assimalign.Cohesion.Resilience.Timeout` | Adds fixed and dynamically selected timeout strategies. | [Overview](assimalign-cohesion-resilience-timeout/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Resilience` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.ObjectPool` (CohesionProjectReference) |
| `Assimalign.Cohesion.Resilience.CircuitBreaker` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference) |
| `Assimalign.Cohesion.Resilience.Fallback` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference) |
| `Assimalign.Cohesion.Resilience.Hedging` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference) |
| `Assimalign.Cohesion.Resilience.RateLimiting` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference), `System.Threading.RateLimiting` (CohesionPackageReference) |
| `Assimalign.Cohesion.Resilience.Retry` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference) |
| `Assimalign.Cohesion.Resilience.Timeout` | `Assimalign.Cohesion.Resilience` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Assimalign.Cohesion.Resilience.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src/Assimalign.Cohesion.Resilience.CircuitBreaker.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/Assimalign.Cohesion.Resilience.Fallback.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src/Assimalign.Cohesion.Resilience.Hedging.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/Assimalign.Cohesion.Resilience.RateLimiting.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/tests`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src/Assimalign.Cohesion.Resilience.Retry.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Retry/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src/Assimalign.Cohesion.Resilience.Timeout.csproj`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/tests`.
