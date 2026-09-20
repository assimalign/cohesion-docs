# Assimalign.Cohesion.ApiManager.ApplicationModel

Compose a build-produced manifest with `builder.AddApiManager(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`ApiManagerResource`](api-manager-resource.md)** — Documented public type.
- **[`ApiManagerResourceControlPlane`](api-manager-resource-control-plane.md)** — Documented public type.
- **[`ApiManagerResourceOptions`](api-manager-resource-options.md)** — Documented public type.
- **[`IApiManagerResourceDescriptor`](i-api-manager-resource-descriptor.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddApiManager(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `ApiManagerResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: ApiManager](../index.md)

## Sources

- **Primary source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.ApplicationModel/src/Assimalign.Cohesion.ApiManager.ApplicationModel.csproj`.
