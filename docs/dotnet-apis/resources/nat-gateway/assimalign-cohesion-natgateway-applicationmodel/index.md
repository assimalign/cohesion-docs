# Assimalign.Cohesion.NatGateway.ApplicationModel

Compose a build-produced manifest with `builder.AddNatGateway(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`INatGatewayResourceDescriptor`](i-nat-gateway-resource-descriptor.md)** — Documented public type.
- **[`NatGatewayResource`](nat-gateway-resource.md)** — Documented public type.
- **[`NatGatewayResourceControlPlane`](nat-gateway-resource-control-plane.md)** — Documented public type.
- **[`NatGatewayResourceOptions`](nat-gateway-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddNatGateway(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `NatGatewayResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: NatGateway](../index.md)

## Sources

- **Primary source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.ApplicationModel/src/Assimalign.Cohesion.NatGateway.ApplicationModel.csproj`.
