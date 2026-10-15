# ILoadBalancerResourceDescriptor

The `ILoadBalancerResourceDescriptor` type is part of the documented `Assimalign.Cohesion.LoadBalancer.ApplicationModel` API.

> **Status:** Partial.

Describes a LoadBalancer resource together with its application-graph dependencies.

Namespace: `Assimalign.Cohesion.LoadBalancer.ApplicationModel`.

## Documented behavior

`LoadBalancerResource` inherits `PlannedResource`; `LoadBalancerResourceOptions` inherits
`ResourceOptions`. `ILoadBalancerResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddLoadBalancer` extends `IApplicationBuilder`;
`LoadBalancerResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no
command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.LoadBalancer.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/docs/Assembly/Assimalign.Cohesion.LoadBalancer.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/src/Abstractions/ILoadBalancerResourceDescriptor.cs`.
