# LoadBalancer

LoadBalancer defines the application and hosting seams for a load-balancing resource.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

LoadBalancer is the L3 networking service platform intended to manage backend pools, health probes, routing and affinity policy, draining, failover, and load-balancing decisions.

Domain services remain fillers pending the area program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.LoadBalancer`](assimalign-cohesion-loadbalancer/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.LoadBalancer.Hosting`](assimalign-cohesion-loadbalancer-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel`](assimalign-cohesion-loadbalancer-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.LoadBalancer.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries. The
exact runtime may reference any same-area library except the area's `Testing` and
`Hosting.ApplicationModel` packages, the framework producers, and harnesses (owner decision
2026-10-09). `Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel` references the runtime module, the
one reference `COHRES001` sanctions by name, and makes the application a managed resource
instance. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.LoadBalancer` delivers the `Assimalign.Cohesion.App.LoadBalancer` family. Its producers,
`Assimalign.Cohesion.LoadBalancer.Refs` and `Assimalign.Cohesion.LoadBalancer.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.LoadBalancer` |
| `Assimalign.Cohesion.LoadBalancer` |
| `Assimalign.Cohesion.LoadBalancer.Hosting` |
| `Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.

## Related documentation

- **Product** — [LoadBalancer](../../../load-balancer/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.LoadBalancer`](../../sdks/sdk-load-balancer/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/LoadBalancer/README.md`.
- **Source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Runtime/Directory.Build.props`.
