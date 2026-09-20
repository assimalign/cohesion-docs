# IdentityHub

IdentityHub provides a minimal OpenID Connect issuer and its resource control plane.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

IdentityHub is the L3 identity service platform intended to manage tenants, directories,
applications, principals, credentials, sessions, token services, federation, and provisioning.

The application builder configures a minimal code-first OpenID Connect issuer with persisted ES256
keys, client credentials, a loopback Local device-authorization flow, and a Cohesion resource
control plane. Production HTTPS consumes a gateway-materialized `tls` Secret mount; the self-signed
fallback is Local-only.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.IdentityHub`](assimalign-cohesion-identityhub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.IdentityHub.ApplicationModel`](assimalign-cohesion-identityhub-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.IdentityHub.Client`](assimalign-cohesion-identityhub-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.IdentityHub.Hosting`](assimalign-cohesion-identityhub-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.IdentityHub.Models`](assimalign-cohesion-identityhub-models/index.md) | Feature library | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.IdentityHub.Hosting`. Roots and feature libraries
do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.IdentityHub.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.IdentityHub` delivers the `Assimalign.Cohesion.App.IdentityHub` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.IdentityHub` |
| `Assimalign.Cohesion.IdentityHub` |
| `Assimalign.Cohesion.IdentityHub.Hosting` |
| `Assimalign.Cohesion.IdentityHub.Models` |
| `Assimalign.Cohesion.IdentityModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Forms` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [IdentityHub](../../../identity-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.IdentityHub`](../../sdks/sdk-identity-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.IdentityHub.Refs/src/Assimalign.Cohesion.App.IdentityHub.Refs.csproj`.
