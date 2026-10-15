# Assimalign.Cohesion.EmailHub.ApplicationModel

Compose a build-produced manifest with `builder.AddEmailHub(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`EmailHubResource`](email-hub-resource.md)** — Documented public type.
- **[`EmailHubResourceControlPlane`](email-hub-resource-control-plane.md)** — Documented public type.
- **[`EmailHubResourceOptions`](email-hub-resource-options.md)** — Documented public type.
- **[`IEmailHubResourceDescriptor`](i-email-hub-resource-descriptor.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddEmailHub(manifest, options)`. The typed
descriptor supports dependency edges; the resource produces a platform-neutral Deployment plan.
Generated resource code calls `EmailHubResourceControlPlane.Create()`. All command kinds remain
deferred.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: EmailHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.ApplicationModel/src/Assimalign.Cohesion.EmailHub.ApplicationModel.csproj`.
