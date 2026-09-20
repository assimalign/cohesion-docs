# Resources

Resource references describe the Cohesion service-platform areas and their assembly boundaries.

Resource areas are level-three (L3) service platforms. Each has an `Assimalign.Cohesion.Sdk.<Area>`
build SDK and an `Assimalign.Cohesion.App.<Area>` shared-framework family.

The repository uses a two-layer folder approach: Layer 1 [Resource] → Layer 2 [Library]. The first
folder identifies the resource domain; its child project folders identify the libraries that
implement that domain. These folder layers do not rename the L1/L2/L3 architectural levels.

## Resource areas

| Area | SDK family | Shared framework |
|---|---|---|
| [ApiManager](api-manager/index.md) | `Assimalign.Cohesion.Sdk.ApiManager` | `Assimalign.Cohesion.App.ApiManager` |
| [ConfigurationStore](configuration-store/index.md) | `Assimalign.Cohesion.Sdk.ConfigurationStore` | `Assimalign.Cohesion.App.ConfigurationStore` |
| [Database](database/index.md) | `Assimalign.Cohesion.Sdk.Database` | `Assimalign.Cohesion.App.Database` |
| [EmailHub](email-hub/index.md) | `Assimalign.Cohesion.Sdk.EmailHub` | `Assimalign.Cohesion.App.EmailHub` |
| [EventHub](event-hub/index.md) | `Assimalign.Cohesion.Sdk.EventHub` | `Assimalign.Cohesion.App.EventHub` |
| [IdentityHub](identity-hub/index.md) | `Assimalign.Cohesion.Sdk.IdentityHub` | `Assimalign.Cohesion.App.IdentityHub` |
| [IoTHub](iot-hub/index.md) | `Assimalign.Cohesion.Sdk.IoTHub` | `Assimalign.Cohesion.App.IoTHub` |
| [LoadBalancer](load-balancer/index.md) | `Assimalign.Cohesion.Sdk.LoadBalancer` | `Assimalign.Cohesion.App.LoadBalancer` |
| [LogSpace](log-space/index.md) | `Assimalign.Cohesion.Sdk.LogSpace` | `Assimalign.Cohesion.App.LogSpace` |
| [MediaHub](media-hub/index.md) | `Assimalign.Cohesion.Sdk.MediaHub` | `Assimalign.Cohesion.App.MediaHub` |
| [MessageHub](message-hub/index.md) | `Assimalign.Cohesion.Sdk.MessageHub` | `Assimalign.Cohesion.App.MessageHub` |
| [NatGateway](nat-gateway/index.md) | `Assimalign.Cohesion.Sdk.NatGateway` | `Assimalign.Cohesion.App.NatGateway` |
| [NotificationHub](notification-hub/index.md) | `Assimalign.Cohesion.Sdk.NotificationHub` | `Assimalign.Cohesion.App.NotificationHub` |
| [Rezolvr](rezolvr/index.md) | `Assimalign.Cohesion.Sdk.Rezolvr` | `Assimalign.Cohesion.App.Rezolvr` |
| [Scheduler](scheduler/index.md) | `Assimalign.Cohesion.Sdk.Scheduler` | `Assimalign.Cohesion.App.Scheduler` |
| [SecretStore](secret-store/index.md) | `Assimalign.Cohesion.Sdk.SecretStore` | `Assimalign.Cohesion.App.SecretStore` |
| [VpnGateway](vpn-gateway/index.md) | `Assimalign.Cohesion.Sdk.VpnGateway` | `Assimalign.Cohesion.App.VpnGateway` |
| [Web](web/index.md) | `Assimalign.Cohesion.Sdk.Web` | `Assimalign.Cohesion.App.Web` |

## Dependency rules

Every area has one exact runtime module, `Assimalign.Cohesion.<Area>.Hosting`. It composes
dependency injection, configuration, logging, and transports. The area root owns the application and
builder contracts. Features extend those contracts without importing the runtime. Hosting
integrations use the `<Area>.Hosting.<Suffix>` naming family.

| `Diagnostic` | Enforced boundary |
|---|---|
| `COHRES001` | Area libraries cannot reference the exact hosting module without a named exemption. Roots and features also cannot reference hosting-family integrations. Integrations can reference each other, but cannot reference the exact runtime. |
| `COHRES002` | The exact hosting module’s direct same-area references are limited to the area root and its hosting integrations. |
| `COHRES003` | Shipped resource assemblies cannot resolve `Assimalign.Cohesion.ApplicationModel.Gateway*` assemblies; there is no exemption. |
| `COHRES004` | Roots and features cannot directly or transitively reference `Assimalign.Cohesion.Hosting` or `Assimalign.Cohesion.Hosting.*`. The hosting family, exact area testing package, and application-model assemblies are the permitted categories. |
| `COHAM001` | An application-model assembly opting into `CohesionApplicationModelGuard` is restricted to the fixed dependency closure below. All 18 resource application-model assemblies enable the guard. |

The `COHAM001` closure permits `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.ApplicationModel`,
`Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.Hosting.Health`,
`Assimalign.Cohesion.Hosting.Resources`, the base class library (BCL) supplied by
`Microsoft.NETCore.App`, and the `System.Security.Cryptography.ProtectedData` facade used by the
Windows mount carrier. The guard is an opt-in migration gate, not permission to extend that
allowlist.

`COHRES001`, `COHRES003`, `COHRES004`, and `COHAM001` check both the project-reference graph and
resolved assemblies, including package-delivered references. `COHRES002` constrains direct
references: child roots may arrive transitively through the area root. Tests, examples, samples, and
fixtures are excluded by their source paths.

## Composition and packaging

- **`Root` contracts** — The root owns `I<Area>ApplicationBuilder` and
  `I<Area>Application`. Child roots are independently consumable; the parent root
  references them, and they never reference the parent root.
- **Feature registration** — Feature packages own their registration verbs and consume
  root contracts. Sibling feature references are permitted.
- **Runtime composition** — `<Area>Application.CreateBuilder(string[] args)` returns
  a concrete builder. Background-service registration belongs on that concrete builder.
- **Orchestration** — `<Area>.ApplicationModel` owns manifest-backed resources,
  planners, graph verbs, and the default control-plane contract. It never references its
  exact runtime. Generated code in an enabled executable joins the two through
  `Assimalign.Cohesion.Hosting.Resources.ResourceRuntime`.
- **Executable ownership** — Consumers own their `Program.cs`. With
  `CohesionApplicationModel=enabled`, build output includes `resource.json`,
  typed resource accessors, and control-plane registration. The default disabled setting
  produces a plain application without those orchestration outputs.
- **Testing exemption** — Where present, `<Area>.Testing` is the area’s sole
  explicit `CohesionHostingIsolationExemptions` holder and invokes the real entry
  point in a test-scoped resource context.
- **Framework delivery** — `frameworks/Assimalign.Cohesion.App.props` distinguishes
  public reference-pack assemblies from private runtime assemblies. `Application`-model and
  client packages remain NuGet-only.

[.NET APIs](../index.md)

## Sources

- **Primary source** — `cohesion/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/build/Targets/Build.Rules.targets`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
