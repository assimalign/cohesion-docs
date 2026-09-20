# Scheduler

Scheduler composes job declarations with cron and timer schedule providers.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

Scheduler is the L3 Cohesion resource for declaring jobs and binding them to trigger providers. The
executable host runs every declared schedule, reports health through the resource control plane, and
drains an active occurrence during graceful shutdown.

Jobs are declarations, not one-shot work: `AddJob` creates a dormant job. `AddCronSchedule` and
`AddTimerSchedule` bind that job to a provider. An unbound job never executes.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.Scheduler`](assimalign-cohesion-scheduler/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.Scheduler.ApplicationModel`](assimalign-cohesion-scheduler-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.Scheduler.Cron`](assimalign-cohesion-scheduler-cron/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Scheduler.Hosting`](assimalign-cohesion-scheduler-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.Scheduler.Timer`](assimalign-cohesion-scheduler-timer/index.md) | Feature library | Public reference and runtime |

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Scheduler.Hosting`. Roots and feature libraries
do not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.Scheduler.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.Scheduler` delivers the `Assimalign.Cohesion.App.Scheduler` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.Scheduler` |
| `Assimalign.Cohesion.Scheduler` |
| `Assimalign.Cohesion.Scheduler.Cron` |
| `Assimalign.Cohesion.Scheduler.Hosting` |
| `Assimalign.Cohesion.Scheduler.Timer` |

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

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [Scheduler](../../../scheduler/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Scheduler`](../../sdks/sdk-scheduler/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Scheduler/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.Scheduler.Refs/src/Assimalign.Cohesion.App.Scheduler.Refs.csproj`.
