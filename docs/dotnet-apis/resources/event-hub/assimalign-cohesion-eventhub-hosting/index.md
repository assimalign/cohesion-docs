# Assimalign.Cohesion.EventHub.Hosting

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`EventHubApplication`](event-hub-application.md)** — Documented public type.

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`.
Explicit services preserve registration/start order and reverse stop order. Enabled resources
discover their area control plane and serve health, readiness, liveness, endpoint discovery, stop,
and command envelopes on the ambient `http` endpoint (http). The plain host opens no listener
without registration.

Managed namespaced routes use ES256 bootstrap verification. The private Web implementation stays out
of consumer reference packs. Domain service behavior and command kinds remain deferred.

See [DESIGN.md](design.md) .

## Concrete composition (T10 / O34)

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`;
its `Build()` returns the public `EventHubApplication : Host<EventHubApplicationContext>`. The
public `EventHubApplicationContext` implements `IEventHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration belongs to the concrete `EventHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<EventHubApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and `Database`'s `AddService`, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Factories run
once per build against the same context retained by the application; the hosted-service snapshot is
installed after factory evaluation. Services start in registration order and stop in reverse. No
area-owned service abstraction is introduced.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.EventHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Telemetry` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting.Resources` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |

[Parent: EventHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/src/Assimalign.Cohesion.EventHub.Hosting.csproj`.
