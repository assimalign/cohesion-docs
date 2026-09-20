# LoadBalancer

LoadBalancer defines the application and hosting seams for a load-balancing resource.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

LoadBalancer is the L3 networking service platform intended to manage backend pools, health probes,
routing and affinity policy, draining, failover, and load-balancing decisions.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.LoadBalancer`](assimalign-cohesion-loadbalancer/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.LoadBalancer.ApplicationModel`](assimalign-cohesion-loadbalancer-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.LoadBalancer.Hosting`](assimalign-cohesion-loadbalancer-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.LoadBalancer.Hosting`. Roots and feature
libraries do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child
libraries. Feature registration composes against the root contracts. The exact runtime may reference
its area root and its hosting-family integrations; integrations may not reference the exact runtime.
The declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.LoadBalancer.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.LoadBalancer` delivers the `Assimalign.Cohesion.App.LoadBalancer` family.
Its reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.LoadBalancer` |
| `Assimalign.Cohesion.LoadBalancer` |
| `Assimalign.Cohesion.LoadBalancer.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Web.Hosting` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [LoadBalancer](../../../load-balancer/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.LoadBalancer`](../../sdks/sdk-load-balancer/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/LoadBalancer/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.LoadBalancer.Refs/src/Assimalign.Cohesion.App.LoadBalancer.Refs.csproj`.
