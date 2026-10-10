# ApiManager

ApiManager defines application contracts, hosting, and orchestration for an API-management resource.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

ApiManager is the L3 service platform intended to manage API backends, routes, products,
subscriptions, contracts, and gateway policy execution.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.ApiManager`](assimalign-cohesion-apimanager/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.ApiManager.ApplicationModel`](assimalign-cohesion-apimanager-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.ApiManager.Hosting`](assimalign-cohesion-apimanager-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.ApiManager.Hosting`. Roots and feature libraries
do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference any
same-area library except the area's `Testing`, `ApplicationModel`, and
`ApplicationModel.Orchestration` packages, the framework producers, and harnesses, and its resolved
closure may carry neither of the two ApplicationModel packages (owner decision 2026-10-09);
integrations may not reference the exact runtime. The declarative application model remains separate
from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.ApiManager.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.ApiManager` delivers the `Assimalign.Cohesion.App.ApiManager` family. Its
producers, `Assimalign.Cohesion.ApiManager.Refs` and `Assimalign.Cohesion.ApiManager.Runtime`,
declare `CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The public
and private assembly inventory is hand-curated in the Runtime producer's `Directory.Build.props`,
which the Refs producer imports. `CohesionFrameworkAssembly` entries appear in the reference and
runtime packs; `CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.ApiManager` |
| `Assimalign.Cohesion.ApiManager` |
| `Assimalign.Cohesion.ApiManager.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Cookies` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [ApiManager](../../../api-manager/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.ApiManager`](../../sdks/sdk-api-manager/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/ApiManager/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Refs/src/Assimalign.Cohesion.ApiManager.Refs.csproj` and `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Runtime/Directory.Build.props`.
