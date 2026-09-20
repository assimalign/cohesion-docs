# Assimalign.Cohesion.EventHub.ApplicationModel

Compose a build-produced manifest with `builder.AddEventHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`EventHubResource`](event-hub-resource.md)** — Documented public type.
- **[`EventHubResourceControlPlane`](event-hub-resource-control-plane.md)** — Documented public type.
- **[`EventHubResourceOptions`](event-hub-resource-options.md)** — Documented public type.
- **[`IEventHubResourceDescriptor`](i-event-hub-resource-descriptor.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddEventHub(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `EventHubResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: EventHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.ApplicationModel/src/Assimalign.Cohesion.EventHub.ApplicationModel.csproj`.
