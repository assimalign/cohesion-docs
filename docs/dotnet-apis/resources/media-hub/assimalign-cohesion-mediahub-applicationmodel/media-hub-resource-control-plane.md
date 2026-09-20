# MediaHubResourceControlPlane

The `MediaHubResourceControlPlane` type is part of the documented `Assimalign.Cohesion.MediaHub.ApplicationModel` API.

> **Status:** Partial.

Creates the default control plane for enabled MediaHub resources.

Namespace: `Assimalign.Cohesion.MediaHub.ApplicationModel`.

## Documented behavior

`MediaHubResource` inherits `PlannedResource`; `MediaHubResourceOptions` inherits `ResourceOptions`
. `IMediaHubResourceDescriptor` exposes the typed resource and ordinary dependency edges.
`AddMediaHub` extends `IApplicationBuilder`; `MediaHubResourceControlPlane.Create` returns an
isolated `IResourceControlPlane` with no command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.MediaHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.MediaHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/src/MediaHubResourceControlPlane.cs`.
