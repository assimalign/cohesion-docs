# Assimalign.Cohesion.Scheduler.Hosting

The Scheduler runtime module: `SchedulerApplication.CreateBuilder(args)`, the concrete host, and its services.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`SchedulerApplication`](scheduler-application.md)** — Documented public type.

Assimalign.Cohesion.Scheduler.Hosting supplies SchedulerApplication.CreateBuilder. The concrete
application executes all schedules supplied through registered providers while preserving
caller-added host services.

The area's resource integration is `Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel`, which
references this module; this module references no application-model library. The runtime drains
active occurrences during stop.

## Concrete composition (T10 / O34)

`SchedulerApplication.CreateBuilder(args)` returns the public concrete
`SchedulerApplicationBuilder`; its `Build()` returns the public `SchedulerApplication :
Host<SchedulerApplicationContext>`. The public `SchedulerApplicationContext` implements
`ISchedulerApplicationContext`, reading `ContentRootPath` from the host environment. The application
explicitly forwards the root lifecycle contract to `IHost`, and consumers use the concrete
application for `RunAsync` and `await using`. Runtime options and supporting services remain
internal.

Background-work registration belongs to the concrete `SchedulerApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<SchedulerApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and Database's AddService, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Database's
root-level AddServer keeps the interface context. Factories run once per build against the same
context retained by the application; the hosted-service snapshot is installed after factory
evaluation. Services start in registration order and stop in reverse. No area-owned service
abstraction is introduced.

## Dependency injection composition

`SchedulerApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order. `Build` closes registration and resolves the services once. Disposing
the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Scheduler` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |

The area's resource integration, [`Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel`](../assimalign-cohesion-scheduler-hosting-applicationmodel/index.md), references this module and registers the resource service in its `Services`.

[Parent: Scheduler](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/src/Assimalign.Cohesion.Scheduler.Hosting.csproj`.
