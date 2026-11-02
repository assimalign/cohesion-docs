# SecretStore

SecretStore provides protected secret persistence and private certificate workflows.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

SecretStore is the L3 operational service platform for protected secret persistence and private certificate workflows. The substantive runtime stores protected files on its persistent `data` volume and issues durable private-CA leaves on first resolution of `certs/<name>`. A SecretStore application becomes a managed resource instance with `Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel`: `builder.Services.AddResource("secrets", resource => resource.UseInProcess())` joins the gateway that runs it through the platform it opted into.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.SecretStore`](assimalign-cohesion-secretstore/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.SecretStore.Client`](assimalign-cohesion-secretstore-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.SecretStore.Hosting`](assimalign-cohesion-secretstore-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel`](assimalign-cohesion-secretstore-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.SecretStore.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference any
same-area library except the area's `Testing` and `Hosting.ApplicationModel` packages, the
framework producers, and harnesses (owner decision 2026-10-09).
`Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel` references the runtime module, the one reference
`COHRES001` sanctions by name, and makes the application a managed resource instance with
`builder.Services.AddResource(name, ...)`. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.SecretStore` delivers the `Assimalign.Cohesion.App.SecretStore` family. Its producers,
`Assimalign.Cohesion.SecretStore.Refs` and `Assimalign.Cohesion.SecretStore.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.SecretStore` |
| `Assimalign.Cohesion.SecretStore` |
| `Assimalign.Cohesion.SecretStore.Hosting` |
| `Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Security.DataProtection` |

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.
## Related documentation

- **Product** — [SecretStore](../../../secret-store/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.SecretStore`](../../sdks/sdk-secret-store/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/SecretStore/README.md`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Refs/src/Assimalign.Cohesion.SecretStore.Refs.csproj` and `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Runtime/Directory.Build.props`.
