# Rezolvr

Rezolvr defines a DNS server resource and its application and hosting seams.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

Rezolvr is the L3 networking service platform intended to be a standalone DNS server with authoritative zones, forwarding and recursive resolution, caching, transfers, and administration.
It is a DNS server product, never the service-discovery subsystem.

Record declarations, a record store, and DNS answer serving remain deferred.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.Rezolvr`](assimalign-cohesion-rezolvr/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.Rezolvr.Hosting`](assimalign-cohesion-rezolvr-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel`](assimalign-cohesion-rezolvr-hosting-applicationmodel/index.md) | Resource integration | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Rezolvr.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries. The
exact runtime may reference any same-area library except the area's `Testing` and
`Hosting.ApplicationModel` packages, the framework producers, and harnesses (owner decision
2026-10-09). `Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel` references the runtime module, the
one reference `COHRES001` sanctions by name, and makes the application a managed resource
instance. See the [resource dependency rules](../index.md#dependency-rules).

## Framework and SDK

`Assimalign.Cohesion.Sdk.Rezolvr` delivers the `Assimalign.Cohesion.App.Rezolvr` family. Its producers,
`Assimalign.Cohesion.Rezolvr.Refs` and `Assimalign.Cohesion.Rezolvr.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The assembly
inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which the Refs
producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

The SDK also references the [App hosting kernel](../../libraries/app/index.md), which supplies
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting`, the connection
capability packages, and the configuration and logging providers alongside the area inventory below.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.Rezolvr` |
| `Assimalign.Cohesion.Rezolvr` |
| `Assimalign.Cohesion.Rezolvr.Hosting` |
| `Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel` |

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

- **Product** — [Rezolvr](../../../rezolvr/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Rezolvr`](../../sdks/sdk-rezolvr/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/README.md`.
- **Source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Runtime/Directory.Build.props`.
