# Rezolvr

Rezolvr defines a DNS server resource and its application, hosting, and orchestration seams.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

Rezolvr is the L3 networking service platform intended to be a standalone DNS server with
authoritative zones, forwarding and recursive resolution, caching, transfers, and administration.

Rezolvr is a DNS server product and is never the service-discovery subsystem. Discovery uses
observed endpoints, Service DNS, and export documents.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.Rezolvr`](assimalign-cohesion-rezolvr/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.Rezolvr.ApplicationModel`](assimalign-cohesion-rezolvr-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.Rezolvr.Client`](assimalign-cohesion-rezolvr-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.Rezolvr.Hosting`](assimalign-cohesion-rezolvr-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Rezolvr.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.Rezolvr.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.Rezolvr` delivers the `Assimalign.Cohesion.App.Rezolvr` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.Rezolvr` |
| `Assimalign.Cohesion.Rezolvr` |
| `Assimalign.Cohesion.Rezolvr.Hosting` |

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

- **Product** — [Rezolvr](../../../rezolvr/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Rezolvr`](../../sdks/sdk-rezolvr/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.Rezolvr.Refs/src/Assimalign.Cohesion.App.Rezolvr.Refs.csproj`.
