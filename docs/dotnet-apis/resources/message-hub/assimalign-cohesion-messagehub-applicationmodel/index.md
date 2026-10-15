# Assimalign.Cohesion.MessageHub.ApplicationModel

Compose a build-produced manifest with `builder.AddMessageHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IMessageHubResourceDescriptor`](i-message-hub-resource-descriptor.md)** — Documented public type.
- **[`MessageHubResource`](message-hub-resource.md)** — Documented public type.
- **[`MessageHubResourceControlPlane`](message-hub-resource-control-plane.md)** — Documented public type.
- **[`MessageHubResourceOptions`](message-hub-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddMessageHub(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `MessageHubResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: MessageHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.ApplicationModel/src/Assimalign.Cohesion.MessageHub.ApplicationModel.csproj`.
