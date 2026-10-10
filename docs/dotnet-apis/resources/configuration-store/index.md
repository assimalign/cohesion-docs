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
any same-area library except the area's `Testing`, `ApplicationModel`, and
`ApplicationModel.Orchestration` packages, the framework producers, and harnesses, and its resolved
closure may carry neither of the two ApplicationModel packages (owner decision 2026-10-09);
integrations may not reference the exact runtime. The declarative application model remains separate
from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.ConfigurationStore.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.ConfigurationStore` delivers the
`Assimalign.Cohesion.App.ConfigurationStore` family. Its producers,
`Assimalign.Cohesion.ConfigurationStore.Refs` and `Assimalign.Cohesion.ConfigurationStore.Runtime`,
declare `CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The public
and private assembly inventory is hand-curated in the Runtime producer's `Directory.Build.props`,
which the Refs producer imports. `CohesionFrameworkAssembly` entries appear in the reference and
runtime packs; `CohesionFrameworkPrivateAssembly` entries appear only at runtime.

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
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Cookies` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
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
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Refs/src/Assimalign.Cohesion.ConfigurationStore.Refs.csproj` and `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Runtime/Directory.Build.props`.
