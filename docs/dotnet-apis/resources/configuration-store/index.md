# ConfigurationStore

ConfigurationStore provides durable configuration namespaces and their snapshot reads.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

ConfigurationStore is the L3 resource area for durable, named configuration namespaces. Its
default host serves namespace snapshots over its `/cohesion/v1` endpoint; values are configuration
rather than secrets and are stored as plain JSON beneath the `data` volume. A ConfigurationStore application becomes a managed resource instance with `Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel`: `builder.Services.AddResource("settings", resource => resource.UseInProcess())` joins the gateway that runs it through the platform it opted into.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.ConfigurationStore`](assimalign-cohesion-configurationstore/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.ConfigurationStore.Client`](assimalign-cohesion-configurationstore-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting`](assimalign-cohesion-configurationstore-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel`](assimalign-cohesion-configurationstore-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.ConfigurationStore.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference any
same-area library except the area's `Testing` and `Hosting.ApplicationModel` packages, the
framework producers, and harnesses (owner decision 2026-10-09).
`Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel` references the runtime module, the one reference
`COHRES001` sanctions by name, and makes the application a managed resource instance with
`builder.Services.AddResource(name, ...)`. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.ConfigurationStore` delivers the `Assimalign.Cohesion.App.ConfigurationStore` family. Its producers,
`Assimalign.Cohesion.ConfigurationStore.Refs` and `Assimalign.Cohesion.ConfigurationStore.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.ConfigurationStore` |
| `Assimalign.Cohesion.ConfigurationStore` |
| `Assimalign.Cohesion.ConfigurationStore.Hosting` |
| `Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
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

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.
## Related documentation

- **Product** — [ConfigurationStore](../../../configuration-store/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.ConfigurationStore`](../../sdks/sdk-configuration-store/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/README.md`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Refs/src/Assimalign.Cohesion.ConfigurationStore.Refs.csproj` and `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Runtime/Directory.Build.props`.
