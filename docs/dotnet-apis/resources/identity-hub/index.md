# IdentityHub

IdentityHub provides a minimal OpenID Connect issuer.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

IdentityHub is the L3 identity service platform intended to manage tenants, directories, applications, principals, credentials, sessions, token services, federation, and provisioning.

The application builder configures a minimal code-first OpenID Connect issuer with persisted ES256 keys, client credentials, and a loopback Local device-authorization flow. An IdentityHub application becomes a managed resource instance with `Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel`: `builder.Services.AddResource("identity", resource => resource.UseInProcess())` joins the gateway that runs it through the platform it opted into. The self-signed fallback is Local-only.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.IdentityHub`](assimalign-cohesion-identityhub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.IdentityHub.Hosting`](assimalign-cohesion-identityhub-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel`](assimalign-cohesion-identityhub-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |
| [`Assimalign.Cohesion.IdentityHub.Models`](assimalign-cohesion-identityhub-models/index.md) | Feature library | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.IdentityHub.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference any
same-area library except the area's `Testing` and `Hosting.ApplicationModel` packages, the
framework producers, and harnesses (owner decision 2026-10-09).
`Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel` references the runtime module, the one reference
`COHRES001` sanctions by name, and makes the application a managed resource instance with
`builder.Services.AddResource(name, ...)`. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.IdentityHub` delivers the `Assimalign.Cohesion.App.IdentityHub` family. Its producers,
`Assimalign.Cohesion.IdentityHub.Refs` and `Assimalign.Cohesion.IdentityHub.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.IdentityHub` |
| `Assimalign.Cohesion.IdentityHub` |
| `Assimalign.Cohesion.IdentityHub.Hosting` |
| `Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel` |
| `Assimalign.Cohesion.IdentityHub.Models` |
| `Assimalign.Cohesion.IdentityModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.SecurityHeaders` |
| `Assimalign.Cohesion.Web.Server` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Forms` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Http.Streaming` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.
## Related documentation

- **Product** — [IdentityHub](../../../identity-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.IdentityHub`](../../sdks/sdk-identity-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/README.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Refs/src/Assimalign.Cohesion.IdentityHub.Refs.csproj` and `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Runtime/Directory.Build.props`.
