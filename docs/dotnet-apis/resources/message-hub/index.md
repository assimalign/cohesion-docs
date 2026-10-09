# MessageHub

MessageHub defines the application and hosting seams for a messaging resource.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

MessageHub is the L3 messaging service platform intended to provide queues, topics, producers,
consumers, settlement, retries, dead-lettering, and message correlation.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.MessageHub`](assimalign-cohesion-messagehub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.MessageHub.ApplicationModel`](assimalign-cohesion-messagehub-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.MessageHub.Client`](assimalign-cohesion-messagehub-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.MessageHub.Hosting`](assimalign-cohesion-messagehub-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.MessageHub.Hosting`. Roots and feature libraries
do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.MessageHub.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.MessageHub` delivers the `Assimalign.Cohesion.App.MessageHub` family. Its
producers, `Assimalign.Cohesion.MessageHub.Refs` and `Assimalign.Cohesion.MessageHub.Runtime`,
declare `CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The public
and private assembly inventory is hand-curated in the Runtime producer's `Directory.Build.props`,
which the Refs producer imports. `CohesionFrameworkAssembly` entries appear in the reference and
runtime packs; `CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.MessageHub` |
| `Assimalign.Cohesion.MessageHub` |
| `Assimalign.Cohesion.MessageHub.Hosting` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Cookies` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
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

- **Product** — [MessageHub](../../../message-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.MessageHub`](../../sdks/sdk-message-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/MessageHub/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Refs/src/Assimalign.Cohesion.MessageHub.Refs.csproj` and `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Runtime/Directory.Build.props`.
