# MediaHub

MediaHub defines the application and hosting seams for a media service resource.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

MediaHub is the L3 media service platform intended to coordinate asset ingest and cataloging,
processing jobs, packaging and manifests, and delivery policy.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.MediaHub`](assimalign-cohesion-mediahub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.MediaHub.ApplicationModel`](assimalign-cohesion-mediahub-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.MediaHub.Hosting`](assimalign-cohesion-mediahub-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.MediaHub.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.MediaHub.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.MediaHub` delivers the `Assimalign.Cohesion.App.MediaHub` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.MediaHub` |
| `Assimalign.Cohesion.MediaHub` |
| `Assimalign.Cohesion.MediaHub.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Web.Hosting` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [MediaHub](../../../media-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.MediaHub`](../../sdks/sdk-media-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/MediaHub/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.MediaHub.Refs/src/Assimalign.Cohesion.App.MediaHub.Refs.csproj`.
