# MSBuild

The base SDK establishes defaults before Microsoft imports and adds generation and validation after them.

## Evaluation order

1. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` enters the base SDK.
2. `Targets/Assimalign.Cohesion.Sdk.Defaults.props` supplies empty-value defaults before Microsoft
   defaults run.
3. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
4. `Targets/Build.Version.props` loads the static version snapshot generated when the SDK was packed.
5. `Targets/Assimalign.Cohesion.Sdk.Common.props` sets the SDK marker and intermediate path.
6. `Targets/Sdk.Resource.props` registers manifest tasks and resource metadata defaults.
7. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props` registers the settings task.
8. `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` registers all frameworks and
   conditionally adds `Assimalign.Cohesion.App`.
9. The consumer project body evaluates.
10. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` captures image publish inputs and supplies
    self-contained host-runtime defaults for enabled Debug resources.
11. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
12. `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` registers pin validation.
13. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` wires settings generation and cleanup.
14. `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets` imports `Sdk.Resource.targets`
    , preserves the container extension chain, then imports `Sdk.Image.targets`.
15. `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` converts Cohesion reference items.

The packaged `Build.Version.props` exists because `sdks/Directory.Build.targets` generates it. It is
not a missing repository source file. `Assimalign.Cohesion.Sdk.ResourceManifest.props` is packaged
into resource manifest packages as `buildTransitive/<PackageId>.props`; it is not a direct import
of the base `Sdk.props`.

- **Props** — [Files, properties, metadata, and frameworks](props.md).
- **Targets** — [Target hooks, inputs, outputs, and packaging](targets.md).
- **Tasks** — [Task parameters, errors, and source tests](tasks.md).

## Property and item inventory

This index includes read-only Microsoft Build Engine (MSBuild) inputs and private state as well as
consumer options. Names beginning with an underscore are implementation details. The
[overview](../overview.md) describes supported consumer settings; this inventory records every
explicit property and item name referenced or assigned by the shipped files.

### Properties

| Name | Role | Files |
|---|---|---|
| `_CohesionApplicationLower` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionApplicationSeed` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionApplicationSlug` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionArchiveFullPath` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionGetManifestDependsOn` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionHasDigestPinnedImage` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionImageArchiveSink` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageBase` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageBuild` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageForward` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageFullPath` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageInContainer` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageInputHash` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageIsCurrent` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageProperties` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImagePublishAot` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImagePublishSelfContainedInput` | Private state | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `_CohesionImageRegistry` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageRid` | Private state | `Targets/Sdk.Image.targets` |
| `_CohesionImageRuntimeIdentifierInput` | Private state | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `_CohesionImageSelfContainedInput` | Private state | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `_CohesionIntermediateAppHostFullPath` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionIntermediateOutputPath` | Private state | `Targets/Assimalign.Cohesion.Sdk.Common.props` |
| `_CohesionOutputTypeLower` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionPackImageArchivePath` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionPackImageManifestPath` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionPackManifest` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionProjectNameHasThreeOrMoreSegments` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionProjectNameHasTwoSegments` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionResolvedApplicationName` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionResolvedResourceKind` | Private state | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `_CohesionResolvedResourceName` | Private state | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `_CohesionResourceEnabled` | Private state | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `_CohesionResourceManifestFullPath` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionResourceNameLower` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionResourceNameSeed` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionResourceNameSlug` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionRuntimePack` | Private state | `Targets/Sdk.Resource.targets` |
| `_CohesionSdk` | Private state | `Targets/Assimalign.Cohesion.Sdk.Common.props` |
| `_NativeExecutableExtension` | Private state | `Targets/Sdk.Resource.targets` |
| `_StronglyTypedSettingsInputsCache` | Private state | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `_StronglyTypedSettingsIntermediateOutputPath` | Private state | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `_StronglyTypedSettingsOutput` | Private state | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `_StronglyTypedSettingsTaskAssembly` | Private state | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `AppHostIntermediatePath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `AssemblyName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `BuildInParallel` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `CohesionAppFrameworkVersion` | Version used by all registered framework targeting and runtime packs. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionApplication` | Application identity; aliases CohesionApplicationName. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionApplicationModel` | Enable or disable resource generation. | `Sdk/Sdk.targets`; `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionApplicationName` | Application identity; aliases CohesionApplication. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionAppSettingsClass` | Opt in by naming the generated settings root class. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `CohesionAppSettingsNamespace` | Namespace of generated settings types. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `CohesionAutoIncludeAppFramework` | Suppress implicit framework references only when set to false before inclusion. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionBuildResourceContainersDependsOn` | Extension chain used by CohesionPublishImage. | `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`; `Targets/Sdk.Image.targets` |
| `CohesionContainerArchiveOutputPath` | Archive sink path contained beneath the image-index directory. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `CohesionContainerBaseImage` | Base image identity; auto selects a .NET 10 runtime-deps image. | `Targets/Sdk.Image.targets` |
| `CohesionContainerPush` | Select registry sink when true; otherwise create an archive. | `Targets/Sdk.Image.targets` |
| `CohesionContainerRegistry` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `CohesionContainerRepository` | Container repository; defaults from organization and resolved resource name. | `Targets/Sdk.Image.targets` |
| `CohesionControlPlaneEndpoint` | Endpoint used for the default control plane. | `Targets/Sdk.Resource.targets` |
| `CohesionControlPlanePath` | Absolute control-plane route prefix. | `Targets/Sdk.Resource.targets` |
| `CohesionImageAot` | Image compilation policy: auto, true, or false. | `Targets/Sdk.Image.targets` |
| `CohesionImageFreshness` | Rebuild for source image production; Pinned is package consumption only. | `Targets/Sdk.Image.targets` |
| `CohesionImageManifestPath` | Generated image index path. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `CohesionImageRequired` | Promote missing digest-pinned package image diagnostic to an error. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionManifestPackageId` | NuGet identity of the manifest-only package. | `Targets/Sdk.Resource.targets` |
| `CohesionMaxReplicas` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Resource.targets` |
| `CohesionOrganization` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `CohesionPackImageArchive` | Include the image archive in the manifest package. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionPackResourceManifestPath` | Staged portable manifest path. | `Targets/Sdk.Resource.targets` |
| `CohesionPackRuntime` | Opt into a separate ordinary runtime package after manifest packing. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionProjectReferences` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` |
| `CohesionReplicas` | Replica count written to lifecycle metadata. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionRepositoryDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceComposable` | Whether the manifest permits in-process composition. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionResourceControlPlaneSourcePath` | Generated ResourceControlPlane.g.cs registration path. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceControlPlaneType` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceKind` | Resource kind recorded in the manifest. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceManifestPath` | Generated resource manifest path. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `CohesionResourceName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceReferencesAreRuntime` | Controls resource project compilation edges for in-process mode. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceSourcePath` | Generated Resource.g.cs accessor path. | `Targets/Sdk.Resource.targets` |
| `CohesionRestartPolicy` | OnFailure, Always, or Never. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionSkipResourceApplicationModelReference` | Suppress base automatic area dependency injection. | `Targets/Sdk.Resource.targets` |
| `CohesionSkipSdkPinCheck` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` |
| `CohesionStopGraceSeconds` | Graceful-stop budget, at least five seconds. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionVersion` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`; `Targets/Sdk.Resource.targets` |
| `CohesionWorkloadKind` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Resource.targets` |
| `Configuration` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `ContainerArchiveOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerBaseImage` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerImageFormat` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerImageTags` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerRegistry` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerRepository` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerUser` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ContainerWorkingDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `EnablePreviewFeatures` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `GeneratedContainerDigest` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `ImplicitUsings` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IncludeBuildOutput` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `IntermediateOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Common.props`; `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `IsAotCompatible` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `MSBuildAllProjects` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `MSBuildProjectDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`; `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `MSBuildProjectFullPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `MSBuildProjectName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `MSBuildThisFileDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`; `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; `Targets/Sdk.Resource.targets` |
| `MSBuildThisFileName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props` |
| `NETCoreSdkRuntimeIdentifier` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets` |
| `Nullable` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `OutputType` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props`; `Targets/Sdk.Resource.targets` |
| `PackageId` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `PackageOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `PackageReadmeFile` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `PackageType` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `PublishAot` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |
| `PublishSelfContained` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `RootNamespace` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; `Targets/Sdk.Resource.targets` |
| `RuntimeIdentifier` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `SelfContained` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Image.targets` |
| `SuppressDependenciesWhenPacking` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `TargetDir` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `TargetFramework` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `TargetPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `TargetsForTfmSpecificContentInPackage` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Resource.targets` |
| `ValidateExecutableReferencesMatchSelfContained` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets` |
| `Version` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Image.targets` |

### Items

| Name | Role | Files |
|---|---|---|
| `_AppSettings` | Private intermediate item | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `_CohesionGetManifestResult` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionImageInput` | Private intermediate item | `Targets/Sdk.Image.targets` |
| `_CohesionInProcessCompileCandidate` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionInProcessCompileWithTargetPath` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionInProcessContentCandidate` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionInProcessContentResult` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionInProcessRuntimeAssetResult` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionProject` | Private intermediate item | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` |
| `_CohesionPublishedImage` | Private intermediate item | `Targets/Sdk.Image.targets` |
| `_CohesionReferencedManifest` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionResourceApplicationModelProject` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionResourcePackageReference` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_CohesionResourceProjectReference` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_NativeRestoredAppHostNETCore` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_NoneWithTargetPath` | Private intermediate item | `Targets/Sdk.Resource.targets` |
| `_StronglyTypedSettingsGeneratedArtifact` | Private intermediate item | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `_StronglyTypedSettingsInputLine` | Private intermediate item | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` |
| `CohesionCommand` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `CohesionEndpoint` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionMount` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionPackageReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`; `Targets/Sdk.Resource.targets` |
| `CohesionProbe` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionProjectReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`; `Targets/Sdk.Resource.targets` |
| `CohesionResourceManifest` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`; `Targets/Sdk.Resource.targets` |
| `CohesionResourcePackAsset` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `CohesionResourceProperty` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionResourceReference` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `CohesionSetting` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.props`; `Targets/Sdk.Resource.targets` |
| `Compile` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`; `Targets/Sdk.Resource.targets` |
| `ContainerAppCommand` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerAppCommandArgs` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerDefaultArgs` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerEntrypoint` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerEntrypointArgs` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerEnvironmentVariable` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContainerLabel` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets`; `Targets/Sdk.Resource.targets` |
| `ContainerPort` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ContentWithTargetPath` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `EmbeddedResource` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `FileWrites` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `FrameworkReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`; `Targets/Sdk.Resource.targets` |
| `KnownFrameworkReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `NativeCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `PackageReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`; `Targets/Sdk.Resource.targets` |
| `ProjectReference` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`; `Targets/Sdk.Resource.targets` |
| `ReferenceCopyLocalPaths` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `ResolvedFileToPublish` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Image.targets` |
| `ResourceCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `RuntimeCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `RuntimeTargetsCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |
| `TfmSpecificPackageFile` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Resource.targets` |

[SDK home](../index.md)

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
- **Source** — `cohesion/sdks/Directory.Build.targets`
