# Assimalign.Cohesion.NotificationHub.ApplicationModel

Compose a build-produced manifest with `builder.AddNotificationHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`INotificationHubResourceDescriptor`](i-notification-hub-resource-descriptor.md)** — Documented public type.
- **[`NotificationHubResource`](notification-hub-resource.md)** — Documented public type.
- **[`NotificationHubResourceControlPlane`](notification-hub-resource-control-plane.md)** — Documented public type.
- **[`NotificationHubResourceOptions`](notification-hub-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddNotificationHub(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `NotificationHubResourceControlPlane.Create()`. All command kinds
remain deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: NotificationHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.ApplicationModel/src/Assimalign.Cohesion.NotificationHub.ApplicationModel.csproj`.
