# Assimalign.Cohesion.LogSpace.ApplicationModel

Compose a build-produced manifest with `builder.AddLogSpace(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`ILogSpaceResourceDescriptor`](i-log-space-resource-descriptor.md)** — Documented public type.
- **[`LogSpaceResource`](log-space-resource.md)** — Documented public type.
- **[`LogSpaceResourceControlPlane`](log-space-resource-control-plane.md)** — Documented public type.
- **[`LogSpaceResourceOptions`](log-space-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddLogSpace(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral StatefulSet plan.
Generated resource code calls `LogSpaceResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: LogSpace](../index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/src/Assimalign.Cohesion.LogSpace.ApplicationModel.csproj`.
