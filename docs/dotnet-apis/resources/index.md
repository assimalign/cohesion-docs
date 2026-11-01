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
builder contracts. Features extend those contracts without importing the runtime. Each area's one
application-model integration, `<Area>.Hosting.ApplicationModel`, references the runtime module and
makes the area's application a managed resource instance. Any other `<Area>.Hosting.<Suffix>`
package is reserved for an integration that must not land in every framework that carries the
module.

| `Diagnostic` | Enforced boundary |
|---|---|
| `COHRES001` | Area libraries cannot reference the exact hosting module, except the area's `<Area>.Hosting.ApplicationModel` (sanctioned by its exact name) and a named exemption holder. Roots and features also cannot reference hosting-family integrations. Integrations can reference each other, but cannot reference the exact runtime. |
| `COHRES002` | The exact hosting module may reference any same-area library except `<Area>.Testing`, `<Area>.Hosting.ApplicationModel`, the framework producers, and harnesses (owner decision 2026-10-09). |
| `COHRES003` | Shipped resource assemblies cannot resolve `Assimalign.Cohesion.ApplicationModel.Gateway*` assemblies; there is no exemption. |
| `COHRES004` | Roots and features cannot directly or transitively reference `Assimalign.Cohesion.Hosting` or `Assimalign.Cohesion.Hosting.*`. Only the hosting family, which includes `<Area>.Hosting.ApplicationModel`, and the exact area testing package may. |
| `COHLIB001` | No non-harness project under `libraries/` or `sdks/` may reference a resource project; the same-area SDK is the one exception. |

`COHRES001`, `COHRES003`, and `COHRES004` check both the project-reference graph and resolved
assemblies, including package-delivered references. `COHRES002` checks the module's direct
references against every excluded category. Tests, examples, samples, and fixtures are excluded by
their source paths.

## Composition and packaging

- **`Root` contracts** — The root owns `I<Area>ApplicationBuilder` and
  `I<Area>Application`. Child roots are independently consumable; the parent root
  references them, and they never reference the parent root.
- **Feature registration** — Feature packages own their registration verbs and consume
  root contracts. A verb is a root-builder extension or a component integration that the
  generator projects onto the hosting builder's `Services`, as Web's are
  (`builder.Services.AddAuthentication(...)`). Sibling feature references are permitted.
- **Runtime composition** — `<Area>Application.CreateBuilder(string[] args)` returns
  a concrete builder whose `Services` container is the only composition registry.
  Background-service registration belongs on that concrete builder.
- **Managed resources** — A resource instance is the consumer's ordinary `Sdk.<Area>`
  executable. Being managed is one registration in its `Program.cs`:
  `builder.Services.AddResource(name, resource => resource.UseInProcess())`, from the area's
  `<Area>.Hosting.ApplicationModel`. Without a `COHESION_GATEWAY` platform in the environment it
  registers nothing; managed, it registers one `ApplicationInstanceService` that publishes the
  manifest, restores the gateway's state, and opens the control plane. Nothing is generated at
  build time. See [ApplicationModel](../libraries/application-model/index.md).
- **Testing exemption** — Where present, `<Area>.Testing` is the area’s sole
  explicit `CohesionHostingIsolationExemptions` holder.
- **Framework delivery** — Each area's member list lives in
  `resources/<Area>/Assimalign.Cohesion.<Area>.Runtime/Directory.Build.props`. It distinguishes
  public reference-pack assemblies from private runtime assemblies, and
  `<Area>.Hosting.ApplicationModel` is a public member. Platform packages a resource opts into and
  clients an area does not consume at run time remain NuGet-only.

[.NET APIs](../index.md)

## Sources

- **Primary source** — `cohesion/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/build/Targets/Build.Rules.targets`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
