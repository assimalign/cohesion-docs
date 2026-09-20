# Assimalign.Cohesion.IoTHub.ApplicationModel

Compose a build-produced manifest with `builder.AddIoTHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IIoTHubResourceDescriptor`](i-io-t-hub-resource-descriptor.md)** — Documented public type.
- **[`IoTHubResource`](io-t-hub-resource.md)** — Documented public type.
- **[`IoTHubResourceControlPlane`](io-t-hub-resource-control-plane.md)** — Documented public type.
- **[`IoTHubResourceOptions`](io-t-hub-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddIoTHub(manifest, options)`. The typed descriptor
supports dependency edges; the resource produces a platform-neutral Deployment plan. Generated
resource code calls `IoTHubResourceControlPlane.Create()`. All command kinds remain deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: IoTHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/src/Assimalign.Cohesion.IoTHub.ApplicationModel.csproj`.
