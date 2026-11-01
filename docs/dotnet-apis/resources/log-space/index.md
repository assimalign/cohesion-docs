# LogSpace

LogSpace provides OTLP log ingestion, durable segments, and bounded log queries.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

LogSpace is the L3 observability service platform intended to provide append-only log ingestion, indexing, retention, archival, query, correlation lookup, and export.

A LogSpace application becomes a managed resource instance with `Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel`: `builder.Services.AddResource("logs", resource => resource.UseInProcess())` joins the gateway that runs it through the platform it opted into. Retention, archival and Database.Embedded consumption are deferred.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.LogSpace`](assimalign-cohesion-logspace/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.LogSpace.Hosting`](assimalign-cohesion-logspace-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel`](assimalign-cohesion-logspace-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |
| [`Assimalign.Cohesion.LogSpace.Telemetry`](assimalign-cohesion-logspace-telemetry/index.md) | Feature library | Separate package; not in this framework |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.LogSpace.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference any
same-area library except the area's `Testing` and `Hosting.ApplicationModel` packages, the
framework producers, and harnesses (owner decision 2026-10-09).
`Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel` references the runtime module, the one reference
`COHRES001` sanctions by name, and makes the application a managed resource instance with
`builder.Services.AddResource(name, ...)`. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.LogSpace` delivers the `Assimalign.Cohesion.App.LogSpace` family. Its producers,
`Assimalign.Cohesion.LogSpace.Refs` and `Assimalign.Cohesion.LogSpace.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.LogSpace` |
| `Assimalign.Cohesion.LogSpace` |
| `Assimalign.Cohesion.LogSpace.Hosting` |
| `Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.Server` |

Platform packages a resource opts into, such as `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`,
are NuGet packages the application references, never framework members.
## Related documentation

- **Product** — [LogSpace](../../../log-space/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.LogSpace`](../../sdks/sdk-log-space/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/LogSpace/README.md`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Refs/src/Assimalign.Cohesion.LogSpace.Refs.csproj` and `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Runtime/Directory.Build.props`.
