# Assimalign.Cohesion.LoadBalancer.ApplicationModel

Compose a build-produced manifest with `builder.AddLoadBalancer(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`ILoadBalancerResourceDescriptor`](i-load-balancer-resource-descriptor.md)** — Documented public type.
- **[`LoadBalancerResource`](load-balancer-resource.md)** — Documented public type.
- **[`LoadBalancerResourceControlPlane`](load-balancer-resource-control-plane.md)** — Documented public type.
- **[`LoadBalancerResourceOptions`](load-balancer-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddLoadBalancer(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `LoadBalancerResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: LoadBalancer](../index.md)

## Sources

- **Primary source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/src/Assimalign.Cohesion.LoadBalancer.ApplicationModel.csproj`.
