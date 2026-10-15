# IoTHubResourceOptions

The `IoTHubResourceOptions` type is part of the documented `Assimalign.Cohesion.IoTHub.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for a IoTHub resource.

Namespace: `Assimalign.Cohesion.IoTHub.ApplicationModel`.

## Documented behavior

`IoTHubResource` inherits `PlannedResource`; `IoTHubResourceOptions` inherits `ResourceOptions`.
`IIoTHubResourceDescriptor` exposes the typed resource and ordinary dependency edges. `AddIoTHub`
extends `IApplicationBuilder`; `IoTHubResourceControlPlane.Create` returns an isolated
`IResourceControlPlane` with no command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.IoTHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.IoTHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/src/IoTHubResourceOptions.cs`.
