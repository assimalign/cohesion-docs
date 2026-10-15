# EventHubResourceOptions

The `EventHubResourceOptions` type is part of the documented `Assimalign.Cohesion.EventHub.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for a EventHub resource.

Namespace: `Assimalign.Cohesion.EventHub.ApplicationModel`.

## Documented behavior

`EventHubResource` inherits `PlannedResource`; `EventHubResourceOptions` inherits `ResourceOptions`
. `IEventHubResourceDescriptor` exposes the typed resource and ordinary dependency edges.
`AddEventHub` extends `IApplicationBuilder`; `EventHubResourceControlPlane.Create` returns an
isolated `IResourceControlPlane` with no command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.EventHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.EventHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.ApplicationModel/src/EventHubResourceOptions.cs`.
