# Assimalign.Cohesion.MediaHub.ApplicationModel

Compose a build-produced manifest with `builder.AddMediaHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IMediaHubResourceDescriptor`](i-media-hub-resource-descriptor.md)** — Documented public type.
- **[`MediaHubResource`](media-hub-resource.md)** — Documented public type.
- **[`MediaHubResourceControlPlane`](media-hub-resource-control-plane.md)** — Documented public type.
- **[`MediaHubResourceOptions`](media-hub-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddMediaHub(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `MediaHubResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: MediaHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/src/Assimalign.Cohesion.MediaHub.ApplicationModel.csproj`.
