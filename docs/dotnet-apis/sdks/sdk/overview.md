# Overview

The base SDK exposes shared project defaults, resource metadata, settings generation, and image publication.

> **Status:** Partial. Resource and settings generation are implemented; some image routes remain unavailable.

## Project defaults and overrides

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; explicitly choose Library for a base-SDK library. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | Compile against the .NET 10 target framework. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | Enable preview C# syntax. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | Permit preview APIs; also affects language-version selection. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | Require explicit using directives. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | Enable nullable reference analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | Enable ahead-of-time compatibility analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |

Defaults apply only when empty, before Microsoft’s props. Later ordinary consumer assignments win;
global command-line properties are honored. Resource executables do not multi-target. Override
`LangVersion` together with `EnablePreviewFeatures`. For enabled Debug resources, the targets
supply `SelfContained=true`, the host `RuntimeIdentifier=$(NETCoreSdkRuntimeIdentifier)`, and
`ValidateExecutableReferencesMatchSelfContained=false` only if each is unset.

## Consumer properties

The following tables show source assignments, not promises that every assignment is a user override
point. Conditions and evaluation timing matter. Framework inclusion and framework pack versions must
be supplied before their props evaluation.

| Name | Default or assignment | Effect | Where defined and condition |
|---|---|---|---|
| `SelfContained` | `true` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(SelfContained)' == '' and '$(Configuration)' == 'Debug' and '$(CohesionApplicationModel)' == 'enabled'` |
| `RuntimeIdentifier` | `$(NETCoreSdkRuntimeIdentifier)` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(RuntimeIdentifier)' == '' and '$(Configuration)' == 'Debug' and '$(CohesionApplicationModel)' == 'enabled'` |
| `ValidateExecutableReferencesMatchSelfContained` | `false` | Assigned during `evaluation`. | `Sdk/Sdk.targets`; when `'$(ValidateExecutableReferencesMatchSelfContained)' == '' and '$(Configuration)' == 'Debug' and '$(CohesionApplicationModel)' == 'enabled'` |
| `CohesionBuildResourceContainersDependsOn` | `$(CohesionBuildResourceContainersDependsOn)` | Extension chain used by CohesionPublishImage. | `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`; unconditional |
| `CohesionAppFrameworkVersion` | `$(CohesionVersion)` | Version used by all registered framework targeting and runtime packs. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`; when `'$(CohesionAppFrameworkVersion)' == ''` |
| `CohesionAppSettingsNamespace` | `$(RootNamespace)` | Namespace of generated settings types. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; when `'$(CohesionAppSettingsNamespace)' == '' and '$(CohesionAppSettingsClass)' != ''` |
| `CohesionContainerRepository` | `$(CohesionOrganization)/$(_CohesionResolvedResourceName)` | Container repository; defaults from organization and resolved resource name. | `Targets/Sdk.Image.targets`; when `'$(CohesionContainerRepository)' == '' and '$(CohesionOrganization)' != ''` |
| `CohesionContainerBaseImage` | `auto` | Base image identity; auto selects a .NET 10 runtime-deps image. | `Targets/Sdk.Image.targets`; when `'$(CohesionContainerBaseImage)' == ''` |
| `CohesionContainerArchiveOutputPath` | `$(IntermediateOutputPath)cohesion/images/$(_CohesionResolvedResourceName).tar` | Archive sink path contained beneath the image-index directory. | `Targets/Sdk.Image.targets`; when `'$(CohesionContainerArchiveOutputPath)' == ''` |
| `CohesionContainerPush` | `false` | Select registry sink when true; otherwise create an archive. | `Targets/Sdk.Image.targets`; when `'$(CohesionContainerPush)' == ''` |
| `CohesionImageAot` | `auto` | Image compilation policy: auto, true, or false. | `Targets/Sdk.Image.targets`; when `'$(CohesionImageAot)' == ''` |
| `CohesionImageFreshness` | `Rebuild` | Rebuild for source image production; Pinned is package consumption only. | `Targets/Sdk.Image.targets`; when `'$(CohesionImageFreshness)' == ''` |
| `CohesionBuildResourceContainersDependsOn` | `$(CohesionBuildResourceContainersDependsOn);_CohesionPublishResourceImage` | Extension chain used by CohesionPublishImage. | `Targets/Sdk.Image.targets`; unconditional |
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
| `CohesionApplication` | `$(CohesionApplicationName)` | Application identity; aliases CohesionApplicationName. | `Targets/Sdk.Resource.targets`; when `'$(CohesionApplication)' == '' and '$(CohesionApplicationName)' != ''` |
| `CohesionApplicationName` | `$(CohesionApplication)` | Application identity; aliases CohesionApplication. | `Targets/Sdk.Resource.targets`; when `'$(CohesionApplicationName)' == '' and '$(CohesionApplication)' != ''` |
| `CohesionResourceManifestPath` | `$(IntermediateOutputPath)cohesion/resource.json` | Generated resource manifest path. | `Targets/Sdk.Resource.targets`; when `'$(CohesionResourceManifestPath)' == ''` |
| `CohesionResourceSourcePath` | `$(IntermediateOutputPath)cohesion/Resource.g.cs` | Generated Resource.g.cs accessor path. | `Targets/Sdk.Resource.targets`; when `'$(CohesionResourceSourcePath)' == ''` |
| `CohesionResourceControlPlaneSourcePath` | `$(IntermediateOutputPath)cohesion/ResourceControlPlane.g.cs` | Generated ResourceControlPlane.g.cs registration path. | `Targets/Sdk.Resource.targets`; when `'$(CohesionResourceControlPlaneSourcePath)' == ''` |
| `CohesionImageManifestPath` | `$(IntermediateOutputPath)cohesion/image.json` | Generated image index path. | `Targets/Sdk.Resource.targets`; when `'$(CohesionImageManifestPath)' == ''` |
| `CohesionPackResourceManifestPath` | `$(IntermediateOutputPath)cohesion/pack/resource.json` | Staged portable manifest path. | `Targets/Sdk.Resource.targets`; when `'$(CohesionPackResourceManifestPath)' == ''` |
| `CohesionManifestPackageId` | `$(AssemblyName).Manifest` | NuGet identity of the manifest-only package. | `Targets/Sdk.Resource.targets`; when `'$(CohesionManifestPackageId)' == ''` |
| `PackageId` | `$(CohesionManifestPackageId)` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |
| `PackageType` | `CohesionResourceManifest` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |
| `PackageReadmeFile` | `README.md` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |
| `IncludeBuildOutput` | `false` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |
| `SuppressDependenciesWhenPacking` | `true` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |
| `TargetsForTfmSpecificContentInPackage` | `$(TargetsForTfmSpecificContentInPackage);CohesionPackManifest` | Assigned during `evaluation`. | `Targets/Sdk.Resource.targets`; when `'$(_CohesionPackManifest)' == 'true'` |

### Optional inputs without a base value

| Name | Default | Effect | Read by |
|---|---|---|---|
| `CohesionAppSettingsClass` | `Unset` | Opt into generated settings by naming the root class. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `CohesionAppSettingsNamespace` | `$(RootNamespace)` | Namespace used by generated settings. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `CohesionResourceName` | `Derived from project name` | Explicit resource identity or normalized project-name suffix. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceKind` | `Unset in base` | Required when enabled; resource SDKs supply it. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceApplicationModel` | `Unset in base` | Area application-model package/project name. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceControlPlaneType` | `Unset in base` | Optional generated area control-plane factory. | `Targets/Sdk.Resource.targets` |
| `CohesionControlPlaneEndpoint` | `Unset in base` | Required declared endpoint when enabled. | `Targets/Sdk.Resource.targets` |
| `CohesionControlPlanePath` | `Unset in base` | Required absolute route when enabled. | `Targets/Sdk.Resource.targets` |
| `CohesionWorkloadKind` | `Resolved by manifest task if unset` | Lifecycle kind; area SDKs supply Deployment or StatefulSet. | `Targets/Sdk.Resource.targets` |
| `CohesionMaxReplicas` | `Unset` | Optional upper replica limit. | `Targets/Sdk.Resource.targets` |
| `CohesionOrganization` | `Unset` | Organization prefix used to form the default image repository. | `Targets/Sdk.Image.targets` |
| `CohesionContainerRegistry` | `Unset` | Late-bound for archive output; required usable authority for a registry push. | `Targets/Sdk.Image.targets` |
| `CohesionSkipSdkPinCheck` | `Unset` | Tooling-only bypass of SDK pin validation. | `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` |
| `CohesionProjectReferences` | `Unset` | List of project paths used to populate name-only ProjectPath metadata. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` |
| `CohesionSkipResourceApplicationModelReference` | `Unset` | true suppresses automatic area application-model reference. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceReferencesAreRuntime` | `Unset` | true changes resource project references to the in-process compiler-edge shape. | `Targets/Sdk.Resource.targets` |

## Consumer items

| Item | Metadata and defaults | Effect |
|---|---|---|
| `CohesionEndpoint` | `Scheme`, `ContainerPort`, `DevPort`, `Public=false`, `Certificate`, `Protocol=tcp` | Declare named network endpoints. HTTPS without Certificate receives the tls default during generation. |
| `CohesionProbe` | `Endpoint`, `Http`, `Tcp`, `Exec`, `Grpc`, `None` | Use readiness, liveness, or startup identity and exactly one probe kind. |
| `CohesionMount` | `Kind`, `ContainerPath`, `Source`, `Size` | Declare Volume, Configuration, or Secret mounts. Volumes require Size. |
| `CohesionSetting` | `Default`, `Type=string` | Declare named runtime settings. |
| `CohesionCommand` | `No custom metadata` | Advertise non-empty accepted command strings; generation trims, deduplicates, and sorts them. |
| `CohesionResourceReference` | `Version`, `Optional=false`, `Endpoints` | Reference an enabled project or portable manifest package. |
| `CohesionResourceProperty` | `Value` | Declare kind-prefixed properties; keys use the current kind’s lower-case prefix. |
| `CohesionResourceManifest` | `ManifestPath`, `PackageId`, `ReferenceIdentity`, `IsSelf` | Read project or package manifests; buildTransitive props expose package manifests. |
| `CohesionResourcePackAsset` | `PackagePath` | Additional manifest-package content passed to TfmSpecificPackageFile. |
| `CohesionProjectReference` | `ProjectPath` | Name-only reference converted to `ProjectReference` after optional CohesionProjectReferences mapping. |
| `CohesionPackageReference` | `Package reference metadata` | Converted to `PackageReference`; packaged consumers must supply required versions. |

`FrameworkReference` and `KnownFrameworkReference` deliver the framework; generated `Compile`,
`EmbeddedResource`, `FileWrites`, container, and package items are detailed in
[props](msbuild/props.md) and [targets](msbuild/targets.md). Standard project/compiler/publish
inputs and private state are indexed in the
[MSBuild inventory](msbuild/index.md#property-and-item-inventory).

## Generated artifacts

| Trigger | Artifact | Purpose |
|---|---|---|
| Enabled application model | `$(IntermediateOutputPath)cohesion/resource.json` | Manifest, also embedded as cohesion/resource.json. |
| Enabled application model | `$(IntermediateOutputPath)cohesion/Resource.g.cs` | Typed resource accessors and runtime metadata. |
| Enabled application model | `$(IntermediateOutputPath)cohesion/ResourceControlPlane.g.cs` | Area control-plane registration, or inert source for a generic resource. |
| Settings opt-in | `$(IntermediateOutputPath)Cohesion/<class>.generated.cs` and `<class>.generated.inputs` | Generated public types and incremental shape fingerprint. |
| Manifest packing | `$(IntermediateOutputPath)cohesion/pack/resource.json` | Portable copy with project and apphost artifact fields cleared. |
| Image publication | `$(IntermediateOutputPath)cohesion/image.json` | Verified digest identity and optional contained archive. |
| Archive sink | `$(IntermediateOutputPath)cohesion/images/<resource>.tar` | Open Container Initiative (OCI) archive. |

## Strongly typed settings

Set `CohesionAppSettingsClass` to opt in. The task merges shapes from `appsettings*.json`, widens
integer/floating-point combinations, and rejects incompatible shapes. It generates public nullable
root/nested types and explicit `IConfiguration.GetEntry` reads in `Bind(IConfiguration)`. Missing
values preserve existing properties. Array indices are fixed by the input JSON shape; runtime-only
indices are not added. Nested arrays and mixed scalar/object arrays are rejected.

## Pin validation details

The validator skips a missing, non-string, or blank `sdk.version`; a non-empty string is parsed
after removing its prerelease suffix and its numeric version is compared with `10.0.300`.
Recognized package names are matched without case, while their version strings must agree exactly.
It checks agreement among present pins, not whether all required resolver pins were declared.

## Resource package behavior

Enabled resources pack as `$(AssemblyName).Manifest` by default, with
`PackageType=CohesionResourceManifest`, no build output, and suppressed dependencies.
`buildTransitive/<PackageId>.props` exposes `cohesion/resource.json` to consumers.
`CohesionPackRuntime=true` adds a second ordinary package. `CohesionPackImageArchive=true` includes
the verified archive under `cohesion/images/` and stages a rewritten image index. The original
publish index is retained.

## Diagnostics

| Code | Severity | Meaning |
|---|---|---|
| `COHSDK001` | Error | A resource reference resolves to a project with the application model disabled. |
| `COHSDK002` | Error | Recognized SDK pins disagree; sdk.version is malformed or below 10.0.300; or the nearest global.json cannot be read/parsed. |
| `COHSDK003` | Error | Release NativeAOT image publishing has no usable native or in-container route. |
| `COHSDK004` | Warning or error | Manifest packing lacks a digest-pinned image; `CohesionImageRequired`=true makes this an error. |
| `COHSDK005` | Error | Container publication is framework-dependent despite requiring Cohesion shared frameworks. |
| `COHSDK008` | Error | An enabled resource does not use executable output. |
| `COHSDK009` | Error | A resource property uses a foreign kind prefix. |
| `COHSDK010` | Error | Certificate metadata does not identify a Secret mount or is used on a non-HTTPS endpoint; public is reserved. |

Other invalid switches, unknown item metadata, invalid names, probe declarations, settings shapes,
and filesystem failures produce ordinary MSBuild errors without an assigned diagnostic code. See
[tasks](msbuild/tasks.md) for per-task validation.

## Image limits

`CohesionPublishImage` accepts `linux-x64` or opted-in `linux-musl-x64`. Debug defaults to a
self-contained just-in-time (JIT) image. Release requires NativeAOT and rejects
`CohesionImageAot=false`. A Linux x64 host with reachable clang can build natively; the alternate
CLI container route is still blocked by an unspecified build-image contract. `Pinned` is accepted at
Gateway’s package boundary, not as a source-project publication mode. Digest-preserving push-tool
transfer is deferred.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/RESOURCE_MANIFEST_README.md`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionResolveImagePublish.cs`
