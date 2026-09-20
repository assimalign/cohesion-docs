# NotificationHub

NotificationHub defines the application and hosting seams for a notification resource.

> **Status:** Partial. The host and orchestration seams exist; domain services remain fillers.

NotificationHub is the L3 messaging service platform intended to manage subscriptions and audiences,
templates, channel routing, delivery policy, receipts, retries, and suppression.

The host supports enabled-resource control planes; domain services remain fillers pending the area
program.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.NotificationHub`](assimalign-cohesion-notificationhub/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.NotificationHub.ApplicationModel`](assimalign-cohesion-notificationhub-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.NotificationHub.Client`](assimalign-cohesion-notificationhub-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.NotificationHub.Hosting`](assimalign-cohesion-notificationhub-hosting/index.md) | Hosting | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.NotificationHub.Hosting`. Roots and feature
libraries do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child
libraries. Feature registration composes against the root contracts. The exact runtime may reference
its area root and its hosting-family integrations; integrations may not reference the exact runtime.
The declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.NotificationHub.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.NotificationHub` delivers the `Assimalign.Cohesion.App.NotificationHub`
family. Its reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.NotificationHub` |
| `Assimalign.Cohesion.NotificationHub` |
| `Assimalign.Cohesion.NotificationHub.Hosting` |

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

- **Product** — [NotificationHub](../../../notification-hub/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.NotificationHub`](../../sdks/sdk-notification-hub/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/NotificationHub/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.NotificationHub.Refs/src/Assimalign.Cohesion.App.NotificationHub.Refs.csproj`.
