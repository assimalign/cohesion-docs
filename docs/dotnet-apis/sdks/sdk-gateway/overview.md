# Overview

Gateway combines shared resource generation with provider selection and boundary-aware application source generation.

> **Status:** Partial. Seven typed mappings and a finite restore bootstrap are implemented; selective first-restore dependencies are incomplete.

## Defaults and forced assignments

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; explicitly choose Library for a base-SDK library. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | Compile against the .NET 10 target framework. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | Enable preview C# syntax. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | Permit preview APIs; also affects language-version selection. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | Require explicit using directives. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | Enable nullable reference analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | Enable ahead-of-time compatibility analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |

Gateway sets `IsAotCompatible=true` unconditionally in its props after consumer
`Directory.Build.props`. Its `Sdk.targets` restores `OutputType=Exe`,
`CohesionApplicationModel=enabled`, `CohesionResourceKind=Composite`, and its control-plane
application-model identity after the project body and before base targets compute enabled state.
Ordinary project assignments cannot turn it into a library or disable that resource generation.

Gateway suppresses the implicit `Assimalign.Cohesion.App` reference before importing base props.
There is no `Assimalign.Cohesion.App.Gateway` framework. The normal orchestration path uses NuGet
packages. The active in-process exception currently adds `Assimalign.Cohesion.App`,
`Assimalign.Cohesion.App.Web`, and `Assimalign.Cohesion.App.Database` explicitly.

## Gateway properties

| Name | Default or assignment | Effect | Where defined and condition |
|---|---|---|---|
| `CohesionAutoIncludeAppFramework` | `false` | Suppress implicit framework references only when set to false before inclusion. | `Sdk/Sdk.props`; unconditional |
| `OutputType` | `Exe` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; unconditional |
| `CohesionApplicationModel` | `enabled` | Enable or disable resource generation. | `Sdk/Sdk.targets`; unconditional |
| `CohesionResourceKind` | `Composite` | Resource kind recorded in the manifest. | `Sdk/Sdk.targets`; unconditional |
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` | Application-model assembly/package selected for an enabled resource. | `Sdk/Sdk.targets`; unconditional |
| `CohesionResourceReferencesAreRuntime` | `$(_CohesionGatewayInProcessActive)` | Controls resource project compilation edges for in-process mode. | `Sdk/Sdk.targets`; unconditional |
| `ValidateExecutableReferencesMatchSelfContained` | `false` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(_CohesionGatewayInProcessActive)' == 'true'` |
| `PublishAot` | `true` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(CohesionGatewayAot)' == 'true'` |
| `PublishAot` | `false` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(CohesionGatewayAot)' == 'false'` |
| `PublishAot` | `true` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(CohesionGatewayAot)' == 'auto' and '$(_CohesionGatewayAutoAotKnownSafe)' == 'True'` |
| `IsAotCompatible` | `true` | Assigned during `evaluation`. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionApplicationModel` | `enabled` | Enable or disable resource generation. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceKind` | `Composite` | Resource kind recorded in the manifest. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceComposable` | `true` | Whether the manifest permits in-process composition. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionControlPlaneEndpoint` | `admin` | Endpoint used for the default control plane. | `Targets/Sdk.Gateway.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Gateway.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionGatewayAdminPort` | `8082` | Container port for the admin endpoint; refreshed after the project body. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayAdminPort)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Gateway.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionGateways` | `Local` | Semicolon-delimited provider selection. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGateways)' == ''` |
| `CohesionPlatformsVersion` | `$(CohesionVersion)` | Version of selected external Docker/Kubernetes provider packages. | `Targets/Sdk.Gateway.props`; when `'$(CohesionPlatformsVersion)' == ''` |
| `CohesionGatewayAot` | `auto` | auto, true, or false; controls whole-gateway `PublishAot` selection. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayAot)' == ''` |
| `CohesionGatewayInProcess` | `false` | Explicit opt-in required alongside the InProcess provider. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayInProcess)' == ''` |
| `CohesionSkipResourceApplicationModelReference` | `true` | Suppress base automatic area dependency injection. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionGatewaySourcePath` | `$(IntermediateOutputPath)cohesion/Gateway.g.cs` | Destination for generated Gateway.g.cs. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewaySourcePath)' == ''` |
| `CohesionGatewayRequiresJit` | `false` | Computed from contributed provider RequiresJit metadata. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayRequiresJit)' == ''` |
| `PublishAot` | `true` | Assigned during `evaluation`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'true'` |
| `PublishAot` | `false` | Assigned during `evaluation`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'false'` |
| `CohesionGatewayRequiresJit` | `true` | Computed from contributed provider RequiresJit metadata. | `Targets/Sdk.Gateway.targets`; when `'@(_CohesionGatewayJitProvider)' != ''` |
| `CohesionGatewayRequiresJit` | `false` | Computed from contributed provider RequiresJit metadata. | `Targets/Sdk.Gateway.targets`; when `'@(_CohesionGatewayJitProvider)' == ''` |
| `PublishAot` | `true` | Assigned during `CohesionResolveGatewayAot`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'true'` |
| `PublishAot` | `false` | Assigned during `CohesionResolveGatewayAot`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'false'` |
| `PublishAot` | `false` | Assigned during `CohesionResolveGatewayAot`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'auto' and '$(CohesionGatewayRequiresJit)' == 'true'` |
| `PublishAot` | `true` | Assigned during `CohesionResolveGatewayAot`. | `Targets/Sdk.Gateway.targets`; when `'$(CohesionGatewayAot)' == 'auto' and '$(CohesionGatewayRequiresJit)' != 'true'` |

## Consumer and provider inputs

| Property or item | Default/source | Effect or metadata |
|---|---|---|
| `CohesionApplicationName` | Required non-empty | Application identity used by generated code and image gathering. |
| `CohesionIsApplicationSet` | Unset | true contributes composite.applicationSet=true to the resource manifest. |
| `CohesionGatewayProvider` | Contributed by packages | Name, GatewayType, OptionsType, RequiresJit, optional CommandLineApplyMethod. |
| `CohesionGatewayResourceKind` | Seven typed mappings | ApplicationModel, OptionsType, AddMethod, optional DescriptorType; maps manifest kind to generated verb. |
| `CohesionGatewayClientKind` | Five mappings | PackageId for ConfigurationStore, Database, IdentityHub, Rezolvr, and SecretStore client requirements. |
| `CohesionGatewayPackage` | Local, InProcess, Docker, Kubernetes | PackageId and Version for recognized provider selection. |
| `CohesionResourceReference` | Consumer-declared | Project or manifest-package references; metadata follows the base resource contract. |

All inherited settings, resource items, image options, packing controls, and pin diagnostics are in
the [base overview](../sdk/overview.md). The
[MSBuild inventory](msbuild/index.md#property-and-item-inventory) includes standard consumed inputs,
generated items, and private state. Exact default item declarations are in [props](msbuild/props.md)
.

## Provider and in-process behavior

`CohesionGateways` selects packages at evaluation. Provider `buildTransitive` props contribute
concrete `GatewayType` and `OptionsType` metadata; generated dispatch does not scan assemblies.
Optional `CommandLineApplyMethod` names a public static `void Apply(OptionsType, string[])` hook
invoked only for the selected provider with original arguments.

`CohesionGatewayAot=auto` sets `PublishAot=false` if any contributed provider requires JIT. Explicit
`true` or `false` is honored and logged. Early evaluation enables AOT automatically only for known
Local/InProcess-only selections before base targets latch their state.

`InProcess` selection and `CohesionGatewayInProcess=true` are both required to activate nested
project resources. The generated bindings use
`AppContext.BaseDirectory/cohesion/resources/<resource-name>`. Only enabled, composable,
same-application projects enter that selected closure; manifest packages lack local executable
bindings. Child content is remapped into isolated roots, and selected runtime files are carried into
build and publish output. Child-only package identities and transitive build behavior remain outside
the gateway lock-file solve.

## `Outputs`

`$(IntermediateOutputPath)cohesion/Gateway.g.cs` enters `Compile` and `FileWrites`. It exposes
application identity, manifest constants, same-application `Add*` verbs, `AddAllResources`,
boundary-crossing `Externals`, referenced `Applications`, and provider-driven `UseGateway`. Base
targets independently generate the Composite manifest, accessors, and control-plane source.

`CohesionPublishImages` writes `$(PublishDir)application.images.json` with `cohesion/images/v1`
schema. It publishes source projects and consumes pinned package images. Active InProcess-only
selection gathers one composite image; mixed active selections retain member images and append the
composite. Archives are verified and relocated under `images/<ordinal>/`.

## Diagnostics and gaps

| Code or condition | Behavior |
|---|---|
| `COHGW001` | Resolved area Hosting assembly in an out-of-process gateway; guard condition checks CohesionGatewayInProcess. |
| `COHGW002` | InProcess selected for project resources without explicit in-process opt-in. |
| `COHSDK001` | Referenced resource project has application model disabled. |
| `COHSDK002` | Inherited pin agreement and .NET SDK validation. |
| Ordinary errors | Missing identity, invalid switches, malformed manifests/providers, duplicate generated identities, and invalid image sets. |
| Ordinary warning | No resource manifest belongs to this application; runtime Build rejects an empty resource set. |

The shipped dependency bootstrap includes the seven Web, Database, ConfigurationStore, SecretStore,
IdentityHub, Rezolvr, and LogSpace ApplicationModel packages and the SecretStore, Database, and
ConfigurationStore clients. Manifest-derived requirements are discovered after restore and cannot
rewrite `project.assets.json`. A restore-visible producer dependency descriptor remains future
work. Other kinds use generic `ResourceOptions` /`AddResource` mapping. Docker/Kubernetes providers
belong to external platform packages; the SDK contains their package selection but does not supply
their implementations.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionCreateResourceVerbs.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionGatherImageIndexes.cs`
