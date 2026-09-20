# Overview

Cohesion composes executable services through shared libraries, domain SDKs, and a portable application model.

## Code-first services

A resource instance is an executable project with its own `Program.cs`. Its domain builder
captures application declarations; its concrete host owns lifecycle and runtime composition.
An application gateway describes relationships between those executables and realizes the graph
on a selected target. The application model keeps deployment facts separate from service code.

This separation supports local child processes, explicit in-process composites, and container
platforms. A resource's manifest and the selected platform's capabilities still constrain which
topologies are valid.

## SDK and shared-framework model

The base `Assimalign.Cohesion.Sdk` and domain `Assimalign.Cohesion.Sdk.<Domain>` pair with
`Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.<Domain>`. This follows the
`Microsoft.NET.Sdk` plus `Microsoft.NETCore.App` model: selecting a domain SDK supplies the
matching framework's reference and runtime assemblies through NuGet, without a framework installer.

`Assimalign.Cohesion.Sdk.Gateway` is the exception. It consumes ordinary orchestration packages;
there is no `Assimalign.Cohesion.App.Gateway`. Resource `.ApplicationModel` and `.Client` packages
are also NuGet-only, outside resource shared frameworks.

See [SDKs](dotnet-apis/sdks/index.md) and the
[Gateway SDK](dotnet-apis/sdks/sdk-gateway/index.md).

## Three layers

| Layer | Responsibility | Examples |
|---|---|---|
| L1 | Foundation libraries and supporting SDK/tooling infrastructure | Core, protocols, serialization primitives, configuration, logging, and security. |
| L2 | Application runtime and composition | `ApplicationModel`, the gateway family, and the nestable `Hosting` lifecycle. |
| L3 | Executable service platforms | Web, Database, operational stores, identity, messaging, and networking areas. |

L3 resources compose the common runtime and foundations. They do not introduce a second host
lifecycle or move platform orchestration into service libraries. Feature packages extend their
area root's builder contracts; dependency injection, configuration, logging, and transport wiring
belong in the area's hosting module.

## Resource-area boundaries

Every resource area has one exact runtime module, `Assimalign.Cohesion.<Area>.Hosting`.
The root owns `I<Area>ApplicationBuilder` and `I<Area>Application`. The hosting module exposes
`<Area>Application.CreateBuilder(args)`, a concrete builder, and a concrete application supporting
`RunAsync` and asynchronous disposal. `AddService` belongs to the concrete hosting builder.

| Diagnostic | Enforced boundary |
|---|---|
| `COHRES001` | Area libraries cannot reference their exact runtime module without a named assembly exemption. Roots/features cannot reference their hosting-family integrations; integrations cannot reference the exact runtime module. |
| `COHRES002` | The runtime module may reference only its own area root and hosting family within the area. |
| `COHRES003` | Shipped resource projects cannot resolve `Assimalign.Cohesion.ApplicationModel.Gateway*` assemblies; there is no opt-out. |
| `COHRES004` | Area roots/features cannot reference shared `Assimalign.Cohesion.Hosting*` libraries directly or transitively; hosting families, testing, and ApplicationModel are the allowed consumers. |
| `COHAM001` | An opted-in ApplicationModel package has a fixed dependency closure of Core, ApplicationModel, Hosting, Hosting.Health, Hosting.Resources, and permitted base class library assemblies. |

`CohesionApplicationModelGuard=true` enables the last guard. Its allowlist includes the
`System.Security.Cryptography.ProtectedData` facade used by the Windows mount carrier.
It is a migration gate, not a way to add local exceptions. All 18 resource areas have guarded
declarative packages. Generated consumer code joins the declarative and hosting sides through
`ResourceRuntime`; those packages do not reference each other.

## From declarations to running resources

1. A resource SDK builds a `resource.json` manifest describing the executable and its endpoints,
   mounts, commands, and workload facts. `CohesionApplicationModel=enabled` opts a resource in.
2. `IApplicationBuilder` collects descriptors and graph relationships. At `Build()`, area planners
   validate the manifests and produce immutable, platform-neutral `ResourcePlan` values.
3. The selected gateway gathers existing artifacts, starts its observer, and reconciles resources
   in dependency order. Gathering does not build images.
4. Controllers apply desired state; observers publish lifecycle and endpoint observations. The
   gateway waits on each plan's readiness gate before admitting dependents.

Long-running workloads satisfy initial readiness at `Running`. Jobs satisfy it at `Stopped`.
`Degraded` is an observation after startup and does not re-gate already admitted dependents.
Dependency addresses come from observed endpoints, not ports guessed from the desired model.

`LocalGateway` realizes local processes. The InProcess gateway invokes real entry points under
isolated ambient contexts. A Composite is the explicit colocation boundary. Docker and Kubernetes
compilers live in `cohesion-platforms`; see [Platforms](platforms/index.md).

`IApplicationSet`, external declarations, and remote references represent application boundaries.
The gateway control plane supplies authenticated discovery, observed views, and command delivery.
Local discovery metadata is published beneath `.cohesion/<application>/control-plane.json`.
[ApplicationModel reference](dotnet-apis/libraries/application-model/index.md) covers these contracts.

## Runtime inputs and trust

The versioned runtime contract uses `COHESION_*` keys. Out-of-process resources receive environment
variables and file paths; in-process resources receive corresponding ambient context values.
`ResourceEnvironment` supplies the .NET constants and readers; `ResourceContext` supplies the
resource invocation's endpoints, mounts, credentials, and environment.

The gateway owns endpoint binding and resolves Configuration and Secret mount sources before
startup. Resources consume delivered values. An HTTPS endpoint names an ordinary Secret mount,
usually `tls`, holding its certificate, private key, and chain. Trust anchors travel separately.
Bootstrap credentials authorize management; telemetry-scoped credentials cannot authorize sink
query or management.

`Local` denotes a developer machine. `Development` is a deployable environment. Without an
explicit value, environment selection falls back through `DOTNET_ENVIRONMENT` to `Production`.
These distinctions matter for the Local-only certificate fallbacks in the operational hosts.

## Native compilation

Native ahead-of-time compilation (NativeAOT) compatibility is a standing requirement for Cohesion
runtime libraries and resources. Composition favors explicit registration, source-generated
serialization, and static metadata over runtime reflection or assembly scanning.

Platform gateways have a separate policy: `cohesion-platforms` has no repository-wide NativeAOT
mandate. Kubernetes advertises `RequiresJit=true`, where JIT means just-in-time compilation;
the Gateway SDK can therefore disable `PublishAot` in automatic mode. Docker advertises
`RequiresJit=false`. These provider facts do not relax the deployed resource runtime requirement.

## Delivery status

The delivery roadmap orders foundation/runtime work before service build-out. Web, operational
stores, Scheduler, and the shared database engine occupy the core-platform priority; primary data
models and identity follow, then messaging, secondary services, and advanced networking.
Historical scheduling rows are planning context, not proof that a feature is delivered.

The current area sources show working Web hosting; configuration, secret, and identity services;
LogSpace ingestion/query; and Scheduler cron/timer execution. Rezolvr persists record declarations
but does not serve DNS answers. The other non-Database areas have enabled-resource hosts with
domain behavior still deferred. See [Database](database/index.md) for its model-specific status.

Per-area pages take their status from current implementations. Older repository summaries and the
historical service-layer design can lag those implementations; neither establishes a working API
merely by proposing it.

## Versioning and release identity

The repository uses synchronized versions: libraries, resources, SDKs, targeting packs, and runtime
packs from one commit share an identity. `build/Targets/Build.Version.props` is authoritative;
the checked-out policy names `10.0.1-preview.3`. This is source-tree context, not a claim that a
particular feed currently contains every package.

Published identities are immutable. Local installation appends `.local` to the canonical
prerelease and writes packages to `_out/packages`. Tagged releases stage in GitHub Packages;
public preview or release-candidate promotion requires a separate manual release workflow and
review. Alpha/beta remain staging channels; stable promotion is not enabled by that policy.

Every Cohesion SDK pin present in `global.json` must use the same exact version string.
`COHSDK002` enforces agreement and a pinned .NET SDK of at least `10.0.300`. An inner-loop checkout
may consistently select the local package identity instead of the canonical release identity.

## Repository map

| Path in `cohesion` | Contents |
|---|---|
| `analyzers/` | Roslyn analyzers, code fixes, and source generators. |
| `assets/` | Shared schemas and assets. |
| `build/` | Central MSBuild targets, versions, and build tasks. |
| `docs/` | Repository architecture, programs, runtime contract, and release policy. |
| `extensions/` | IDE integrations. |
| `frameworks/` | Shared-framework reference/runtime pack producers and membership manifest. |
| `installer/` | Installer sources, local delivery, and packaging scripts. |
| `libraries/` | Foundation and application-runtime library families. |
| `resources/` | The 18 L3 resource areas. |
| `sdks/` | Base, domain, and Gateway MSBuild SDKs. |
| `tooling/` | Command-line tooling, developer scripts, and project templates. |

The companion `cohesion-platforms` repository owns Containers, Docker, and Kubernetes.
Return to [Cohesion Documentation](index.md).

## Sources

- **Orientation** — `cohesion/README.md` and `cohesion/docs/OVERVIEW.md`.
- **Boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Application architecture** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md` and `cohesion/docs/DEVELOPER_EXPERIENCE_DESIGN.md`.
- **Historical context** — `cohesion/docs/SERVICE_LAYER_DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Delivery** — `cohesion/docs/programs/DELIVERY_ROADMAP.md` and `cohesion/resources/Scheduler/README.md`.
- **Release policy** — `cohesion/docs/VERSIONING_RELEASE_POLICY.md`, `cohesion/docs/versioning/VERSIONING.md`, and `cohesion/build/Targets/Build.Version.props`.
- **Gateway SDK** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Platforms** — `cohesion-platforms/README.md` and `cohesion-platforms/.claude/rules/platform-areas.md`.
