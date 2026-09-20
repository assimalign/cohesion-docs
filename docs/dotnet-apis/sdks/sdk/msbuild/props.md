# Props

The base props define consumer defaults, register tasks, and map framework references to NuGet packs.

## `Sdk/Sdk.props`

Entry point: imports defaults, Microsoft props, frozen version, common markers, resource
definitions, settings registration, and framework registrations in that order.

## `Targets/Assimalign.Cohesion.Sdk.Common.props`

Marks the project as a Cohesion SDK consumer and computes the shared intermediate path. Both
properties are private implementation state.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `_CohesionSdk` | `true` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Common.props`; unconditional |
| `_CohesionIntermediateOutputPath` | `$(IntermediateOutputPath)\Cohesion` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Common.props`; unconditional |

## `Targets/Assimalign.Cohesion.Sdk.Defaults.props`

Sets executable/compiler defaults before `Microsoft.NET.Sdk` can default `OutputType` to Library.
Every
assignment is conditional on an empty value.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `OutputType` | `Exe` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(OutputType)' == ''` |
| `TargetFramework` | `net10.0` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(TargetFramework)' == ''` |
| `LangVersion` | `Preview` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(LangVersion)' == ''` |
| `EnablePreviewFeatures` | `true` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(EnablePreviewFeatures)' == ''` |
| `ImplicitUsings` | `disable` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(ImplicitUsings)' == ''` |
| `Nullable` | `enable` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(Nullable)' == ''` |
| `IsAotCompatible` | `true` | Assigned during `evaluation`. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; when `'$(IsAotCompatible)' == ''` |

## `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`

Registers nineteen shared frameworks for net10.0. The base adds only Assimalign.Cohesion.App; each
resource SDK adds its own area framework.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `CohesionAppFrameworkVersion` | `$(CohesionVersion)` | Version used by all registered framework targeting and runtime packs. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`; when `'$(CohesionAppFrameworkVersion)' == ''` |

| Item | Declaration | Metadata | Condition and source |
|---|---|---|---|
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.Web`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.Web`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.Web.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.Web.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.Database`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.Database`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.Database.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.Database.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.ApiManager`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.ApiManager`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.ApiManager.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.ApiManager.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.ConfigurationStore`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.ConfigurationStore`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.ConfigurationStore.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.ConfigurationStore.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.EmailHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.EmailHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.EmailHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.EmailHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.EventHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.EventHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.EventHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.EventHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.IdentityHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.IdentityHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.IdentityHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.IdentityHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.IoTHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.IoTHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.IoTHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.IoTHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.LoadBalancer`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.LoadBalancer`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.LoadBalancer.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.LoadBalancer.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.LogSpace`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.LogSpace`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.LogSpace.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.LogSpace.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.MediaHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.MediaHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.MediaHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.MediaHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.MessageHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.MessageHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.MessageHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.MessageHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.NatGateway`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.NatGateway`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.NatGateway.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.NatGateway.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.NotificationHub`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.NotificationHub`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.NotificationHub.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.NotificationHub.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.Rezolvr`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.Rezolvr`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.Rezolvr.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.Rezolvr.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.Scheduler`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.Scheduler`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.Scheduler.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.Scheduler.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.SecretStore`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.SecretStore`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.SecretStore.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.SecretStore.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `KnownFrameworkReference` | `Include=Assimalign.Cohesion.App.VpnGateway`; `TargetFramework=net10.0`; `RuntimeFrameworkName=Assimalign.Cohesion.App.VpnGateway`; `DefaultRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `LatestRuntimeFrameworkVersion=$(CohesionAppFrameworkVersion)`; `TargetingPackName=Assimalign.Cohesion.App.VpnGateway.Ref`; `TargetingPackVersion=$(CohesionAppFrameworkVersion)`; `RuntimePackNamePatterns=Assimalign.Cohesion.App.VpnGateway.Runtime.**RID**`; `RuntimePackRuntimeIdentifiers=win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`; `IsTrimmable=true` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `FrameworkReference` | `Include=Assimalign.Cohesion.App` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`; when `'$(CohesionAutoIncludeAppFramework)' != 'false'` |

## `Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`

Resource-package contribution template: includes ../cohesion/resource.json using absolute paths,
identifies the package from MSBuildThisFileName, and marks IsSelf=false.

| Item | Declaration | Metadata | Condition and source |
|---|---|---|---|
| `CohesionResourceManifest` | `Include=$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)../cohesion/resource.json'))`; `ManifestPath=$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)../cohesion/resource.json'))`; `PackageId=$(MSBuildThisFileName)`; `ReferenceIdentity=$(MSBuildThisFileName)`; `IsSelf=false` | No additional child metadata | `Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props` |

## `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`

Registers `CreateStronglyTypedSettingsTask` from the packaged task assembly with Runtime=NET.
Generation is wired later, and only if a class name is supplied.

| Task | Assembly | Runtime |
|---|---|---|
| `Assimalign.Cohesion.Sdk.Tasks.CreateStronglyTypedSettingsTask` | `..\Tasks\Assimalign.Cohesion.Sdk.Tasks.dll` | `NET` |

## `Targets/Sdk.Resource.props`

Registers the manifest, portable-package, and image-validation tasks. Defines orchestration opt-in
defaults and item metadata. It does not activate the application model.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `CohesionApplicationModel` | `disabled` | Enable or disable resource generation. | `Targets/Sdk.Resource.props`; when `'$(CohesionApplicationModel)' == ''` |
| `CohesionApplication` | `$(CohesionApplicationName)` | Application identity; aliases CohesionApplicationName. | `Targets/Sdk.Resource.props`; when `'$(CohesionApplication)' == '' and '$(CohesionApplicationName)' != ''` |
| `CohesionApplicationName` | `$(CohesionApplication)` | Application identity; aliases CohesionApplication. | `Targets/Sdk.Resource.props`; when `'$(CohesionApplicationName)' == '' and '$(CohesionApplication)' != ''` |
| `CohesionResourceComposable` | `true` | Whether the manifest permits in-process composition. | `Targets/Sdk.Resource.props`; when `'$(CohesionResourceComposable)' == ''` |
| `CohesionReplicas` | `1` | Replica count written to lifecycle metadata. | `Targets/Sdk.Resource.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionStopGraceSeconds` | `30` | Graceful-stop budget, at least five seconds. | `Targets/Sdk.Resource.props`; when `'$(CohesionStopGraceSeconds)' == ''` |
| `CohesionRestartPolicy` | `OnFailure` | OnFailure, Always, or Never. | `Targets/Sdk.Resource.props`; when `'$(CohesionRestartPolicy)' == ''` |
| `CohesionPackRuntime` | `false` | Opt into a separate ordinary runtime package after manifest packing. | `Targets/Sdk.Resource.props`; when `'$(CohesionPackRuntime)' == ''` |
| `CohesionImageRequired` | `false` | Promote missing digest-pinned package image diagnostic to an error. | `Targets/Sdk.Resource.props`; when `'$(CohesionImageRequired)' == ''` |
| `CohesionPackImageArchive` | `false` | Include the image archive in the manifest package. | `Targets/Sdk.Resource.props`; when `'$(CohesionPackImageArchive)' == ''` |

| Item | Declaration | Metadata | Condition and source |
|---|---|---|---|
| `CohesionEndpoint` | Metadata defaults | `Scheme=`; `ContainerPort=`; `DevPort=`; `Public=false`; `Certificate=`; `Protocol=tcp` | `Targets/Sdk.Resource.props` |
| `CohesionProbe` | Metadata defaults | `Endpoint=`; `Http=`; `Tcp=`; `Exec=`; `Grpc=`; `None=` | `Targets/Sdk.Resource.props` |
| `CohesionMount` | Metadata defaults | `Kind=`; `ContainerPath=`; `Source=`; `Size=` | `Targets/Sdk.Resource.props` |
| `CohesionSetting` | Metadata defaults | `Default=`; `Type=string` | `Targets/Sdk.Resource.props` |
| `CohesionResourceReference` | Metadata defaults | `Version=`; `Optional=false`; `Endpoints=` | `Targets/Sdk.Resource.props` |
| `CohesionResourceProperty` | Metadata defaults | `Value=` | `Targets/Sdk.Resource.props` |

| Task | Assembly | Runtime |
|---|---|---|
| `Assimalign.Cohesion.Sdk.Tasks.CohesionCreateResourceManifest` | `..\Tasks\Assimalign.Cohesion.Sdk.Tasks.dll` | `NET` |
| `Assimalign.Cohesion.Sdk.Tasks.CohesionPrepareResourceManifestPackage` | `..\Tasks\Assimalign.Cohesion.Sdk.Tasks.dll` | `NET` |
| `Assimalign.Cohesion.Sdk.Tasks.CohesionValidateImageManifest` | `..\Tasks\Assimalign.Cohesion.Sdk.Tasks.dll` | `NET` |

## Generated `Targets/Build.Version.props`

The SDK package freezes `CohesionMajorVersion`, `CohesionMinorVersion`, `CohesionPatchVersion`,
and `CohesionVersion` at pack time. The repository’s dynamic version source is
`build/Targets/Build.Version.props`; its target-framework calculation is not shipped verbatim. The
frozen import precedes framework registrations so targeting pack versions are available when their
items are evaluated.

## Framework membership and pack contents

`frameworks/Assimalign.Cohesion.App.props` is the membership manifest, conditioned on
`CohesionFrameworkName`. Public `CohesionFrameworkAssembly` entries go into targeting and runtime
packs. `CohesionFrameworkPrivateAssembly` entries are runtime-only. `CohesionFrameworkAnalyzer`
entries ship in targeting packs as `analyzers/dotnet/cs/` assets.

The `.Refs` and `.Runtime` projects import the common framework targets.
`CohesionCollectFrameworkPack` resolves source paths at target execution, validates that listed
assemblies exist, and writes `data/FrameworkList.xml` or `data/RuntimeList.xml`. Ref assemblies
live in `ref/<tfm>/`; runtime implementations live in `runtimes/<rid>/lib/<tfm>/`. The SDK’s
registered runtime identifiers are
`win-x64;win-arm64;linux-x64;linux-arm64;linux-musl-x64;osx-x64;osx-arm64`.

Framework inclusion is evaluated in props. Set `CohesionAutoIncludeAppFramework` and
`CohesionAppFrameworkVersion` before that point, for example in consumer `Directory.Build.props` or
as global properties. Changing a property in the later project body does not remove an
already-created item or rewrite its metadata.

[MSBuild](index.md) · [Targets](targets.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Directory.Build.targets`
- **Source** — `cohesion/build/Targets/Build.Version.props`
- **Source** — `cohesion/build/Targets/Build.TargetFramework.props`
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.targets`
- **Source** — `cohesion/frameworks/Directory.Build.props`
- **Source** — `cohesion/frameworks/Directory.Build.targets`
