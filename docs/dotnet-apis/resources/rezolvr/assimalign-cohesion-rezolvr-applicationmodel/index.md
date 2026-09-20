# Assimalign.Cohesion.Rezolvr.ApplicationModel

Compose a build-produced manifest with `builder.AddRezolvr(manifest, options)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IRezolvrResourceDescriptor`](i-rezolvr-resource-descriptor.md)** — Documented public type.
- **[`RezolvrResource`](rezolvr-resource.md)** — Documented public type.
- **[`RezolvrResourceCommandExtensions`](rezolvr-resource-command-extensions.md)** — Documented public type.
- **[`RezolvrResourceControlPlane`](rezolvr-resource-control-plane.md)** — Documented public type.
- **[`RezolvrResourceOptions`](rezolvr-resource-options.md)** — Documented public type.

Compose a build-produced manifest with `builder.AddRezolvr(manifest, options)`. The typed
descriptor supports dependency edges and `AddARecord` /`AddCnameRecord` commands; the resource
produces a platform-neutral Deployment plan. Generated resource code calls
`RezolvrResourceControlPlane.Create()` to advertise both record kinds.

See [DESIGN.md](design.md) for defaults and package boundaries.

## Commands

| Wire kind | Descriptor verb | Ownership key |
|---|---|---|
| `rezolvr.add-a-record` | `AddARecord` | record name |
| `rezolvr.add-cname-record` | `AddCnameRecord` | record name |

A-record declarations use a BCL IPv4 IPAddress serialized as a string. CNAME declarations carry a
DNS target string; TTL is a positive integer in seconds, defaulting to 300. `COHAM001` keeps Dns
assemblies outside the ApplicationModel dependency closure.

Hosting stores records atomically in `records.json` under
`ResourceContext.GetMount("data", Path.GetFullPath(Path.Combine(ContentRootPath, "data")))`.
Without a data mount, storage therefore lives in the content-root-derived data directory. No
CohesionMount or CohesionWorkloadKind change is made: GenericPlanner requires StatefulSet for a
Volume while RezolvrPlanner requires Deployment. The command registry survives restart and restores
ownership before the listener starts.

Records are stored, not served as DNS answers. ResolverEndpointService remains parked. DNS serving
and reconciling a durable Volume with the Deployment contract are deferred area work.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: Rezolvr](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/src/Assimalign.Cohesion.Rezolvr.ApplicationModel.csproj`.
