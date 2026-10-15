# VpnGatewayResourceOptions

The `VpnGatewayResourceOptions` type is part of the documented `Assimalign.Cohesion.VpnGateway.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for a VpnGateway resource.

Namespace: `Assimalign.Cohesion.VpnGateway.ApplicationModel`.

## Documented behavior

`VpnGatewayResource` inherits `PlannedResource`; `VpnGatewayResourceOptions` inherits
`ResourceOptions`. `IVpnGatewayResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddVpnGateway` extends `IApplicationBuilder`;
`VpnGatewayResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no command
kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.VpnGateway.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/docs/Assembly/Assimalign.Cohesion.VpnGateway.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/src/VpnGatewayResourceOptions.cs`.
