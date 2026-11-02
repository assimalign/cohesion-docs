# Assimalign.Cohesion.MessageHub.Hosting

`MessageHubApplication.CreateBuilder(args)` returns the public concrete `MessageHubApplicationBuilder`.

> **Status:** Partial. Domain service behavior remains deferred.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`MessageHubApplication`](message-hub-application.md)** — Documented public type.

The module is the area's runtime: explicit services preserve registration and start order and
reverse stop order, and the plain host opens no listener without a registration. It references no
application-model library. The area's resource integration,
[`Assimalign.Cohesion.MessageHub.Hosting.ApplicationModel`](../assimalign-cohesion-messagehub-hosting-applicationmodel/index.md),
references this module and registers the resource service in its `Services`. The private Web
implementation stays out of consumer reference packs.

## Concrete composition (T10 / O34)

`MessageHubApplication.CreateBuilder(args)` returns the public concrete `MessageHubApplicationBuilder`;
its `Build()` returns the public `MessageHubApplication : Host<MessageHubApplicationContext>`. The public
`MessageHubApplicationContext` implements `IMessageHubApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

Background-work registration belongs to the concrete `MessageHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<MessageHubApplicationContext, IHostService>)`. Factories
run once per build against the same context retained by the application. Services start in
registration order and stop in reverse. No area-owned service abstraction is introduced.
## Dependency injection composition

`MessageHubApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order; that is how `AddResource` joins it. `Build` closes registration and
resolves the services once. Disposing the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.MessageHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |

[Parent: MessageHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Hosting/src/Assimalign.Cohesion.MessageHub.Hosting.csproj`.
