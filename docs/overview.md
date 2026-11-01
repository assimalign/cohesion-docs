# Overview

Cohesion composes executable services through shared libraries, domain SDKs, and a code-first application model.

## Code-first services

A resource instance is an ordinary executable project with its own `Program.cs`. Its domain builder
captures application declarations; its concrete host owns lifecycle and runtime composition. Being
managed is one registration in that program, never a build behavior:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddResource("orders-api", resource => resource.UseInProcess());

WebApplication app = builder.Build();
await app.RunAsync();
```

An application gateway is a separate executable that declares an application model, the resources
it deploys, their dependencies and settings, and runs it on a platform. The logical model (what a
gateway runs) and the physical side (what each instance runs) never load each other's code; they
share only resource identity and the gateway-to-instance protocol.

## SDK and shared-framework model

The base `Assimalign.Cohesion.Sdk` and domain `Assimalign.Cohesion.Sdk.<Domain>` pair with
`Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.<Domain>`. This follows the
`Microsoft.NET.Sdk` plus `Microsoft.NETCore.App` model: selecting a domain SDK supplies the
matching framework's reference and runtime assemblies through NuGet, without a framework installer.
Each area framework carries its `<Area>.Hosting.ApplicationModel` integration, and the App kernel
carries `ApplicationModel.Hosting`, so a resource calls `AddResource` with no package reference.

`Assimalign.Cohesion.Sdk.Gateway` is the exception. It references the
`Assimalign.Cohesion.ApplicationModel.Gateway` package; there is no `Assimalign.Cohesion.App.Gateway`.
Platform packages, such as `ApplicationModel.Hosting.InProcess` and the `cohesion-platforms` pairs,
and area `.Client` packages a resource does not consume at run time are NuGet-only.

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
`RunAsync` and asynchronous disposal. `AddService` belongs to the concrete hosting builder. Each
area's one application-model integration is `<Area>.Hosting.ApplicationModel`.

| Diagnostic | Enforced boundary |
|---|---|
| `COHRES001` | Area libraries cannot reference their exact runtime module, except the area's `<Area>.Hosting.ApplicationModel` (by its exact name) and a named exemption holder. Roots/features cannot reference hosting-family integrations; integrations cannot reference the exact runtime module. |
| `COHRES002` | The runtime module may reference any same-area library except `<Area>.Testing`, `<Area>.Hosting.ApplicationModel`, the framework producers, and harnesses. |
| `COHRES003` | Shipped resource projects cannot resolve `Assimalign.Cohesion.ApplicationModel.Gateway*` assemblies; there is no opt-out. |
| `COHRES004` | Area roots/features cannot reference shared `Assimalign.Cohesion.Hosting*` libraries directly or transitively; the hosting family and the area's testing package are the allowed consumers. |
| `COHLIB001` | Libraries and SDKs never reference a resource project, except the same-area SDK. |

## From declarations to running resources

1. A gateway program declares its model with `ApplicationGateway.CreateBuilder(name)`: one
   `AddResource(provider, name)` per resource, with `DependsOn`, `WithSetting`, and platform
   extensions. It registers the platform it deploys through, such as `UseInProcess(infrastructure)`.
2. `StartAsync` takes the gateway's record over with a new epoch and deploys the resources in
   dependency order, independent ones at once, through the platform's infrastructure provider.
3. Each instance's resource service publishes its manifest through the platform's client before
   any application service or server starts, and restores the state the gateway answers with.
4. A resource is ready once an instance of its deployment has published and its outputs are read;
   dependents then deploy with their settings filled in from those outputs.
5. The run continues until `StopAsync` or a shutdown signal; an operator's action runs on one
   instance's control plane, and a write is restored on every instance.

A platform is a pair of packages that owns every transport between the two sides. The in-process
pair ships in `cohesion`: `Gateway.InProcess` runs each resource's program on a thread of the
gateway's process. Kubernetes and Docker pairs live in `cohesion-platforms`; see
[Platforms](platforms/index.md). A child gateway runs its own model and joins its parent.
[ApplicationModel reference](dotnet-apis/libraries/application-model/index.md) covers these
contracts.

## Runtime inputs and trust

The versioned runtime contract uses `COHESION_*` keys. Out-of-process resources receive environment
variables; in-process resources receive the same keys through the `AppEnvironmentScope` the
in-process gateway enters for each program, read through Core's `AppEnvironment`.
`COHESION_GATEWAY` names the platform and selects the instance's client; `COHESION_MANIFEST_ENDPOINT`
says where it reaches its gateway; `COHESION_DEPLOYMENT` names the deployment the instance repeats in
its manifest; `COHESION_CONFIG__<Section>__<Key>` carries each setting. Without `COHESION_GATEWAY`, a
program runs unmanaged.

Authentication of publications and the gateway's calls belongs to each platform; the in-process
platform needs none. Standalone operational stores such as SecretStore and ConfigurationStore accept
`/cohesion/v1` requests only from loopback transport peers.

`Local` denotes a developer machine. `Development` is a deployable environment. Without an
explicit value, environment selection falls back through `DOTNET_ENVIRONMENT` to `Production`.
These distinctions matter for the Local-only certificate fallbacks in the operational hosts.

## Native compilation

Native ahead-of-time compilation (NativeAOT) compatibility is a standing requirement for Cohesion
runtime libraries and resources. Composition favors explicit registration, source-generated
serialization, and static metadata over runtime reflection or assembly scanning.

Platform packages have a separate policy: `cohesion-platforms` has no repository-wide NativeAOT
mandate, because their gateway halves are deploy-time control planes. That does not relax the
deployed resource runtime requirement.

## Delivery status

The delivery roadmap orders foundation/runtime work before service build-out. Web, operational
stores, Scheduler, and the shared database engine occupy the core-platform priority; primary data
models and identity follow, then messaging, secondary services, and advanced networking.
Historical scheduling rows are planning context, not proof that a feature is delivered.

The current area sources show working Web hosting; configuration, secret, and identity services;
and Scheduler cron/timer execution. LogSpace's receiver is present but not registered by its host.
Rezolvr does not serve DNS answers. The other non-Database
areas have hosts and resource integrations with domain behavior still deferred. See
[Database](database/index.md) for its model-specific status.

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

The companion `cohesion-platforms` repository owns the Docker and Kubernetes platform pairs.
Return to [Cohesion Documentation](index.md).

## Sources

- **Orientation** — `cohesion/README.md` and `cohesion/docs/OVERVIEW.md`.
- **Boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Application architecture** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md` and `cohesion/docs/DEVELOPER_EXPERIENCE_DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Delivery** — `cohesion/docs/programs/DELIVERY_ROADMAP.md` and `cohesion/resources/Scheduler/README.md`.
- **Release policy** — `cohesion/docs/VERSIONING_RELEASE_POLICY.md`, `cohesion/docs/versioning/VERSIONING.md`, and `cohesion/build/Targets/Build.Version.props`.
- **Gateway SDK** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Platforms** — `cohesion-platforms/README.md`.
