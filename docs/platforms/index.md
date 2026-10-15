# Platforms

Platform gateways translate Cohesion resource plans into deployment objects and observe their runtime state.

> **Status:** Partial. Plan compilers, controllers, observers, and provider contributions exist; platform-specific operational gaps remain.

## Deployment and hosting plane

A Cohesion application declares a desired-state resource graph. At application `Build()`, each
resource planner produces a validated `ResourcePlan` using the `cohesion/plan/v1` schema. Exactly
one compiler for the selected platform translates those plans into target objects. Compilers do
not dispatch on resource kind, area, or Common Language Runtime (CLR) type.

The `cohesion` repository owns portable contracts, `ApplicationGateway`, and `LocalGateway`.
The `cohesion-platforms` repository owns the container substrate and platform implementations.

- **[Containers](containers/index.md)** — Image indexes, digest verification, artifact storage, and registry primitives.
- **[Docker](docker/index.md)** — Containers, networks, volumes, and supervision through a compatible engine.
- **[Kubernetes](kubernetes/index.md)** — Workloads, Services, configuration, secrets, and cluster observation.

## Ownership through the lifecycle

| Component | Responsibility |
|---|---|
| Area planner | Validate resource intent and produce a platform-neutral plan. |
| Platform compiler | Pure translation of plans plus supplied runtime inputs into target objects. |
| Controller | Idempotently apply desired state; do not wait for readiness. |
| Observer | Sole writer of platform-realized lifecycle and endpoint observations. |
| Gateway base | Order dependencies, gather artifacts, reconcile, and wait on plan-derived readiness. |

Gathering locates and validates existing artifacts; it never builds an image. Container identities
are digest-pinned. Dependents receive observed endpoint addresses. Long-running workloads satisfy
readiness at `Running`; Jobs at `Stopped`. A later `Degraded` observation does not re-gate them.

`StopAsync` releases runtime supervision and observation while preserving persistent platform
state. `UninstallAsync`, selected by `--mode teardown`, performs explicit reverse deletion.
Each platform documents what is retained when ownership or custom-controller outcomes are unknown.

## Selecting a provider

Use `Assimalign.Cohesion.Sdk.Gateway` for the application's gateway executable. Its
`CohesionGateways` property is a semicolon-delimited allowlist. This project fragment permits
local, Docker, and Kubernetes selection:

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Gateway">
  <PropertyGroup>
    <CohesionApplicationName>example</CohesionApplicationName>
    <CohesionGateways>Local;Docker;Kubernetes</CohesionGateways>
  </PropertyGroup>
</Project>
```

Pin the base and Gateway SDK identities consistently in `global.json`. External platform packages
resolve at `CohesionPlatformsVersion`; choose an available compatible platform release for the
configured feed. Resource references and declarations make the gateway's application graph.

Provider packages contribute `CohesionGatewayProvider` items through `buildTransitive` props.
Metadata includes `Name`, `GatewayType`, `OptionsType`, and `RequiresJit`, plus an optional
`CommandLineApplyMethod`. Generated `UseGateway(args)` selects among those contributions and
applies the selected provider's argument hook after common options. Platform types are not
hard-coded in the SDK's generated dispatch logic.

`Local` and `InProcess` come from Cohesion's gateway libraries. Docker and Kubernetes are external
provider packages. Containers is shared machinery, not another selectable provider.
The Gateway SDK has no matching `Assimalign.Cohesion.App.Gateway` shared framework.

See the [Gateway SDK reference](../dotnet-apis/sdks/sdk-gateway/index.md) and
[ApplicationModel reference](../dotnet-apis/libraries/application-model/index.md).

## Boundaries and delivery

`COHPLT001` enforces platform dependency isolation: shipped platform projects cannot depend on
resource-area ApplicationModel packages, resource hosting runtimes, or `Microsoft.Extensions.*`.
They consume the generic model/gateway contracts, shared Containers machinery, and permitted thin
clients. Docker and Kubernetes do not reference each other.

The platform repository has no Native ahead-of-time compilation (NativeAOT) mandate. Kubernetes
advertises `RequiresJit=true`; Docker advertises `RequiresJit=false`. Cohesion resource runtimes
retain their separate NativeAOT requirement.

| Delivered surface | Remaining boundary |
|---|---|
| Shared image indexes, verified store, pull-only registry | General Kubernetes node-to-registry reachability remains separate work. |
| Docker compiler/controller/observer and offline renderer | Real-daemon coverage and atomic sensitive-input staging remain incomplete. |
| Kubernetes compiler/controller/observer, render/bootstrap, and discovery | Managed port forwarding and live multi-model control-plane addressing remain incomplete. |

Return to [Cohesion Documentation](../index.md).

## Sources

- **Architecture** — `cohesion-platforms/README.md` and `cohesion-platforms/.claude/rules/platform-areas.md`.
- **Delivery** — `cohesion-platforms/docs/PLATFORMS_PROGRAM_PLAN.md`.
- **Application model** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
- **Provider channel** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md` and `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
- **Operational boundaries** — `cohesion-platforms/platforms/Docker/README.md` and `cohesion-platforms/platforms/Kubernetes/README.md`.
