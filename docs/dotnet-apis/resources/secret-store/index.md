# SecretStore

SecretStore provides protected secret persistence and private certificate workflows.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

SecretStore is the L3 operational service platform for protected secret persistence, application
trust, and private certificate workflows. The substantive runtime stores protected files on its
persistent `data` volume, verifies gateway-issued ES256 bootstrap credentials, and issues durable
private-CA leaves on first resolution of `certs/<name>`.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.SecretStore`](assimalign-cohesion-secretstore/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.SecretStore.ApplicationModel`](assimalign-cohesion-secretstore-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.SecretStore.Client`](assimalign-cohesion-secretstore-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.SecretStore.Hosting`](assimalign-cohesion-secretstore-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.SecretStore.Hosting`. Roots and feature libraries
do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.SecretStore.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.SecretStore` delivers the `Assimalign.Cohesion.App.SecretStore` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.SecretStore` |
| `Assimalign.Cohesion.SecretStore` |
| `Assimalign.Cohesion.SecretStore.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Security.DataProtection` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [SecretStore](../../../secret-store/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.SecretStore`](../../sdks/sdk-secret-store/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/SecretStore/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.SecretStore.Refs/src/Assimalign.Cohesion.App.SecretStore.Refs.csproj`.
