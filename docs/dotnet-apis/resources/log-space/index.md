# LogSpace

LogSpace provides authenticated log ingestion, durable segments, and bounded log queries.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

LogSpace is the L3 observability service platform intended to provide append-only log ingestion,
indexing, retention, archival, query, correlation lookup, and export.

The enabled host receives authenticated OTLP/HTTP JSON logs over HTTPS, persists append-only
segments under its data mount, and serves bounded authenticated NDJSON queries. Retention, archival
and `Database.Embedded` consumption are deferred.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.LogSpace`](assimalign-cohesion-logspace/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.LogSpace.ApplicationModel`](assimalign-cohesion-logspace-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.LogSpace.Hosting`](assimalign-cohesion-logspace-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.LogSpace.Telemetry`](assimalign-cohesion-logspace-telemetry/index.md) | Feature library | Not listed in this framework |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.LogSpace.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.LogSpace.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.LogSpace` delivers the `Assimalign.Cohesion.App.LogSpace` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.LogSpace` |
| `Assimalign.Cohesion.LogSpace` |
| `Assimalign.Cohesion.LogSpace.Hosting` |

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

- **Product** — [LogSpace](../../../log-space/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.LogSpace`](../../sdks/sdk-log-space/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/LogSpace/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.LogSpace.Refs/src/Assimalign.Cohesion.App.LogSpace.Refs.csproj`.
