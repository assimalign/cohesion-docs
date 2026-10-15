# NatGatewayResource

The `NatGatewayResource` type is part of the documented `Assimalign.Cohesion.NatGateway.ApplicationModel` API.

> **Status:** Partial.

Represents a manifest-backed NatGateway workload in an application graph.

Namespace: `Assimalign.Cohesion.NatGateway.ApplicationModel`.

## Documented behavior

`NatGatewayResource` inherits `PlannedResource`; `NatGatewayResourceOptions` inherits
`ResourceOptions`. `INatGatewayResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddNatGateway` extends `IApplicationBuilder`;
`NatGatewayResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no command
kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.NatGateway.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.ApplicationModel/docs/Assembly/Assimalign.Cohesion.NatGateway.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.ApplicationModel/src/NatGatewayResource.cs`.
