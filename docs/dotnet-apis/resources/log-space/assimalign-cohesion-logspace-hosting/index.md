# Assimalign.Cohesion.LogSpace.Hosting

The LogSpace runtime module: `LogSpaceApplication.CreateBuilder(args)`, the concrete host, and its services.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`LogSpaceApplication`](log-space-application.md)** — Documented public type.

The area's resource integration is `Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel`, which
references this module; this module references no application-model library.

POST /v1/logs requires application/json. Query accepts resource, since (ISO-8601), limit (default
100/max 1000), and cursor, returning application/x-ndjson and X-Cohesion-Next-Cursor when more
scanning is possible. Query pages are bounded to four million characters (plus at most one bounded
record).

Query under `/cohesion/v1` accepts only transport peers at `127.0.0.1`, `::1`, or IPv4-mapped
`127.0.0.1`; other callers receive `403 Forbidden`. Forwarded headers do not change this decision.
This tightens standalone behavior from 10.0.0-preview.1.

## Concrete composition (T10 / O34)

`LogSpaceApplication.CreateBuilder(args)` returns the public concrete `LogSpaceApplicationBuilder`;
its `Build()` returns the public `LogSpaceApplication : Host<LogSpaceApplicationContext>`. The
public `LogSpaceApplicationContext` implements `ILogSpaceApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration belongs to the concrete `LogSpaceApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<LogSpaceApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and Database's AddService, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Database's
root-level AddServer keeps the interface context. Factories run once per build against the same
context retained by the application; the hosted-service snapshot is installed after factory
evaluation. Services start in registration order and stop in reverse. No area-owned service
abstraction is introduced.

## Dependency injection composition

`LogSpaceApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order. `Build` closes registration and resolves the services once. Disposing
the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.LogSpace` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.RequestLimits` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Security` | `CohesionPrivateProjectReference` |

The area's resource integration, [`Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel`](../assimalign-cohesion-logspace-hosting-applicationmodel/index.md), references this module and registers the resource service in its `Services`.

[Parent: LogSpace](../index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/src/Assimalign.Cohesion.LogSpace.Hosting.csproj`.
