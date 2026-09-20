# Assimalign.Cohesion.VpnGateway.ApplicationModel

Compose a build-produced manifest with `builder.AddVpnGateway(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IVpnGatewayResourceDescriptor`](i-vpn-gateway-resource-descriptor.md)** — Documented public type.
- **[`VpnGatewayResource`](vpn-gateway-resource.md)** — Documented public type.
- **[`VpnGatewayResourceControlPlane`](vpn-gateway-resource-control-plane.md)** — Documented public type.
- **[`VpnGatewayResourceOptions`](vpn-gateway-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddVpnGateway(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `VpnGatewayResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: VpnGateway](../index.md)

## Sources

- **Primary source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/src/Assimalign.Cohesion.VpnGateway.ApplicationModel.csproj`.
