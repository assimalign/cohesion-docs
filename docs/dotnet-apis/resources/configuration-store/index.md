# ConfigurationStore

ConfigurationStore provides durable configuration namespaces through a resource control plane.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

ConfigurationStore is the L3 resource area for durable, named configuration namespaces. Its default
host serves namespace snapshots and declarative mutations over the resource control-plane endpoint;
values are configuration rather than secrets and are stored as plain JSON beneath the `data` volume.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.ConfigurationStore`](assimalign-cohesion-configurationstore/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.ConfigurationStore.ApplicationModel`](assimalign-cohesion-configurationstore-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.ConfigurationStore.Client`](assimalign-cohesion-configurationstore-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting`](assimalign-cohesion-configurationstore-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.ConfigurationStore.Hosting`. Roots and feature
libraries do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child
libraries. Feature registration composes against the root contracts. The exact runtime may reference
its area root and its hosting-family integrations; integrations may not reference the exact runtime.
The declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.ConfigurationStore.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.ConfigurationStore` delivers the
`Assimalign.Cohesion.App.ConfigurationStore` family. Its reference-pack project declares
`CohesionFrameworkName` and imports `frameworks/Assimalign.Cohesion.App.props`, the public and
private assembly inventory. `CohesionFrameworkAssembly` entries appear in the reference and runtime
packs; `CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.ConfigurationStore` |
| `Assimalign.Cohesion.ConfigurationStore` |
| `Assimalign.Cohesion.ConfigurationStore.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [ConfigurationStore](../../../configuration-store/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.ConfigurationStore`](../../sdks/sdk-configuration-store/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.ConfigurationStore.Refs/src/Assimalign.Cohesion.App.ConfigurationStore.Refs.csproj`.
