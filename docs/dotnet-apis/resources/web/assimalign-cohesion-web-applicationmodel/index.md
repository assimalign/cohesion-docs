# Assimalign.Cohesion.Web.ApplicationModel

The Web area's AOT-compatible, dependency-guarded orchestration package supplies a typed manifest-backed resource, a platform-neutral planner, and the default control-plane factory used by enabled Web executables.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The Web area's AOT-compatible, dependency-guarded orchestration package supplies a typed
manifest-backed resource, a platform-neutral planner, and the default control-plane factory used by
enabled Web executables.

## `Key` surface

- **`WebResource`** — snapshots a build-produced `ResourceManifest`.
- **`WebResourceOptions`** — provides the typed `Replicas` override.
- **`AddWeb(manifest, options)`** — returns `IWebResourceDescriptor` with typed dependency chaining. `RemoteReferenceWeb` returns that surface for a manifest-backed external; both retain the canonical graph resource identity.
- **The Web planner emits `cohesion/plan/v1` IR** — a stateless `Deployment`, no
  persistent volumes, one service per endpoint, and one exposure per public endpoint.
- **`WebResourceControlPlane.Create()`** — returns a fresh `IResourceControlPlane`.
- **The plane aggregates health,** — readiness, and liveness contributions.
- **It reports realized endpoints** — and accepts graceful-stop requests.
- **Its accepted command list** — is currently empty.

The Web SDK adds this package only for an executable with `CohesionApplicationModel=enabled`.
Generated code registers the factory by resource assembly; `WebApplication.CreateBuilder(args)`
consumes that registration and the current `Hosting.Resources` `ResourceContext`. A disabled
executable remains a plain Web application.

Public exposure remains a manifest fact. `Plan` v1 has no host or certificate override fields;
platform-specific ingress, load-balancer, and certificate-resolution choices remain gateway
responsibilities.

## Dependencies

- **Detail** — `Assimalign.Cohesion.ApplicationModel`
- **Detail** — `Assimalign.Cohesion.Hosting.Resources`

No Web runtime or platform assembly enters this dependency closure.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/src/Assimalign.Cohesion.Web.ApplicationModel.csproj`.
