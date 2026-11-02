# MediaHub

MediaHub defines the application and hosting seams for a media service resource.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

MediaHub is the L3 media service platform intended to coordinate asset ingest and cataloging, processing jobs, packaging and manifests, and delivery policy.

Domain services remain fillers pending the area program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.MediaHub`](assimalign-cohesion-mediahub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.MediaHub.Hosting`](assimalign-cohesion-mediahub-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel`](assimalign-cohesion-mediahub-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.MediaHub.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries. The
exact runtime may reference any same-area library except the area's `Testing` and
`Hosting.ApplicationModel` packages, the framework producers, and harnesses (owner decision
2026-10-09). `Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel` references the runtime module, the
one reference `COHRES001` sanctions by name, and makes the application a managed resource
instance. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.MediaHub` delivers the `Assimalign.Cohesion.App.MediaHub` family. Its producers,
`Assimalign.Cohesion.MediaHub.Refs` and `Assimalign.Cohesion.MediaHub.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.MediaHub` |
| `Assimalign.Cohesion.MediaHub` |
| `Assimalign.Cohesion.MediaHub.Hosting` |
| `Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.

## Related documentation

- **Product** — [MediaHub](../../../media-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.MediaHub`](../../sdks/sdk-media-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/MediaHub/README.md`.
- **Source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Runtime/Directory.Build.props`.
