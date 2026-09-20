# MessageHubResourceControlPlane

The `MessageHubResourceControlPlane` type is part of the documented `Assimalign.Cohesion.MessageHub.ApplicationModel` API.

> **Status:** Partial.

Creates the default control plane for enabled MessageHub resources.

Namespace: `Assimalign.Cohesion.MessageHub.ApplicationModel`.

## Documented behavior

`MessageHubResource` inherits `PlannedResource`; `MessageHubResourceOptions` inherits
`ResourceOptions`. `IMessageHubResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddMessageHub` extends `IApplicationBuilder`;
`MessageHubResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no command
kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.MessageHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.MessageHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.ApplicationModel/src/MessageHubResourceControlPlane.cs`.
