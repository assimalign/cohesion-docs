# INotificationHubResourceDescriptor

The `INotificationHubResourceDescriptor` type is part of the documented `Assimalign.Cohesion.NotificationHub.ApplicationModel` API.

> **Status:** Partial.

Describes a NotificationHub resource together with its application-graph dependencies.

Namespace: `Assimalign.Cohesion.NotificationHub.ApplicationModel`.

## Documented behavior

`NotificationHubResource` inherits `PlannedResource`; `NotificationHubResourceOptions` inherits
`ResourceOptions`. `INotificationHubResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddNotificationHub` extends `IApplicationBuilder`;
`NotificationHubResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no
command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.NotificationHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.NotificationHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.ApplicationModel/src/Abstractions/INotificationHubResourceDescriptor.cs`.
