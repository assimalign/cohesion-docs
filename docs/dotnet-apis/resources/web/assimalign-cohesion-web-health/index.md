# Assimalign.Cohesion.Web.Health

`Assimalign.Cohesion.Web.Health` is the public health-check feature for Cohesion Web applications.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

## Purpose

`Assimalign.Cohesion.Web.Health` is the public health-check feature for Cohesion Web applications.
It owns both the health model and its HTTP delivery through `/healthz`, `/readyz`, and `/livez`
pipeline endpoints.

## Scope

- **Model:** `IHealthCheck`, `HealthStatus`, `HealthCheckResult`, `HealthReport`,
  `HealthReportEntry`, and `HealthCheckRegistration`.
- **Composition:** `HealthChecks.CreateBuilder()`, `IHealthChecksBuilder`, inline and typed checks.
  The optional `Web.Hosting.Health` package adds `AddContributor` for host contributors.
- **Filtering:** `HealthTags` and `HealthCheckPredicates` for aggregate, readiness, and liveness
  views.
- **HTTP delivery:** `MapHealthChecks`, `MapReadinessCheck`, `MapLivenessCheck`,
  `HealthEndpointOptions`, `IHealthResponseWriter`, and `IHttpHealthFeature`.

## Dependencies

The package references `Assimalign.Cohesion.Web` for pipeline and HTTP contracts. It references no
hosting library or dependency-injection container. `Web.Hosting.Health` references this package and
`Hosting.Health` to supply the optional contributor bridge.

## Framework delivery

- **`App.Web`** — lists `Assimalign.Cohesion.Web.Health` as a public framework assembly. Web application
  authors see and use its types directly.
- **`App.Database`** — lists the same assembly as a private framework assembly because the `Database`
  runtime uses a private Web implementation closure. `Database` application authors do not gain a
  public Web health surface from that inclusion.

The package can also be referenced directly outside those framework profiles.

## Usage

Compose an immutable service, then explicitly map the endpoints that the application exposes:

See the [source-backed usage examples](examples/index.md).

`AddContributor` uses `IHealthContributor.Name` as the registration name and preserves the
contribution's status, description, and diagnostic data. It forwards request cancellation and
supports the same failure status, tags, and timeout policy as `AddCheck`. When tags are omitted,
both `ready` and `live` are applied; pass an empty collection for aggregate-only participation.

The middleware receives `IHealthCheckService` explicitly. It does not locate services during a
request and does not mutate registrations after the service is built.

Hosting contributor integration is supplied by `Assimalign.Cohesion.Web.Hosting.Health`.
`Web.Health` owns the health model and endpoints and references no hosting library (O34).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/src/Assimalign.Cohesion.Web.Health.csproj`.
