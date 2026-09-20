# Assimalign.Cohesion.Scheduler.Hosting

Assimalign.Cohesion.Scheduler.Hosting supplies SchedulerApplication.CreateBuilder.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`SchedulerApplication`](scheduler-application.md)** — Documented public type.

`Assimalign.Cohesion.Scheduler.Hosting` supplies `SchedulerApplication.CreateBuilder`. The concrete
application executes all schedules supplied through registered providers while preserving
caller-added host services.

Enabled resources inherit their environment, content root, http endpoint, bootstrap credential, and
shutdown grace from ResourceRuntime. The runtime observes the endpoint, attaches the default control
plane, exposes health/readiness/liveness and management routes, and drains active occurrences during
stop.

## Concrete composition (T10 / O34)

`SchedulerApplication.CreateBuilder(args)` returns the public concrete `SchedulerApplicationBuilder`
; its `Build()` returns the public `SchedulerApplication : Host<SchedulerApplicationContext>`. The
public `SchedulerApplicationContext` implements `ISchedulerApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration belongs to the concrete `SchedulerApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<SchedulerApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and `Database`'s `AddService`, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Factories run
once per build against the same context retained by the application; the hosted-service snapshot is
installed after factory evaluation. Services start in registration order and stop in reverse. No
area-owned service abstraction is introduced.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Scheduler` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Telemetry` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |

[Parent: Scheduler](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/src/Assimalign.Cohesion.Scheduler.Hosting.csproj`.
