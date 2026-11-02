# Assimalign.Cohesion.EventHub.Hosting

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`.

> **Status:** Partial. Domain service behavior remains deferred.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`EventHubApplication`](event-hub-application.md)** — Documented public type.

The module is the area's runtime: explicit services preserve registration and start order and
reverse stop order, and the plain host opens no listener without a registration. It references no
application-model library. The area's resource integration,
[`Assimalign.Cohesion.EventHub.Hosting.ApplicationModel`](../assimalign-cohesion-eventhub-hosting-applicationmodel/index.md),
references this module and registers the resource service in its `Services`. The private Web
implementation stays out of consumer reference packs.

## Concrete composition (T10 / O34)

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`;
its `Build()` returns the public `EventHubApplication : Host<EventHubApplicationContext>`. The public
`EventHubApplicationContext` implements `IEventHubApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

Background-work registration belongs to the concrete `EventHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<EventHubApplicationContext, IHostService>)`. Factories
run once per build against the same context retained by the application. Services start in
registration order and stop in reverse. No area-owned service abstraction is introduced.
## Dependency injection composition

`EventHubApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order; that is how `AddResource` joins it. `Build` closes registration and
resolves the services once. Disposing the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.EventHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |

[Parent: EventHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/src/Assimalign.Cohesion.EventHub.Hosting.csproj`.
