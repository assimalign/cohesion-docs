# Targets

The base targets turn evaluated metadata into generated source, portable manifests, and verified images.

> **Status:** Partial. Native image production is available under the documented host conditions; the in-container recipe is unspecified.

## Resource and image lifecycle

The application-model switch is resolved after the consumer project body. Enabled resources get
generated `Compile` items, an embedded manifest, and a manifest label. Resource references are
project build edges or private manifest-package references. Outside the in-process exception,
project edges have `ReferenceOutputAssembly=false` and return `CohesionResourceManifest` items.

The name-only converter is evaluation-time logic: `CohesionProjectReferences` maps filenames to
`ProjectPath` metadata, then `CohesionProjectReference` and `CohesionPackageReference` become
ordinary reference items. It is not the repository-wide project search or central package-version
registry.

## Image entry points

```sh
dotnet publish -c Debug -t:CohesionPublishImage -p:CohesionOrganization=example
```

The child publication is self-contained and targets `linux-x64` unless explicitly changed to the
supported musl variant. Restore and publish receive the same properties in a fresh process. Exactly
one image creation selects an archive or registry sink. The index records `repository@digest`
identity; a version tag is informational. Registry output omits `archive`. An archive path must
remain contained beneath the index directory.

The index schema is `cohesion/image/v1` with `resource`, `repository`, nullable `registry` and
`tag`, `digest`, `platform`, `aot`, `baseImage`, and optional `archive`. Both supported
runtime identifiers currently record `linux/amd64`; the base image preserves the musl distinction.
`CohesionContainerPushTool` is deferred.

Evaluation registers targets; `DependsOnTargets` establishes execution dependencies, while
`BeforeTargets` and `AfterTargets` attach hooks. Absent `Inputs` /`Outputs` means the target
declares no timestamp-based incremental check.

## `Sdk/Sdk.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `Sdk.targets` | `Microsoft.NET.Sdk` | Unconditional |
| `..\Targets\Assimalign.Cohesion.Sdk.PinValidation.targets` | Local file | Unconditional |
| `..\Targets\Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` | Local file | Unconditional |
| `..\Targets\Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets` | Local file | Unconditional |
| `..\Targets\Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` | Local file | Unconditional |

This file contains evaluation-time wiring and no `Target` declarations.

## `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `Sdk.Resource.targets` | Local file | Unconditional |
| `Sdk.Image.targets` | Local file | Unconditional |

This file contains evaluation-time wiring and no `Target` declarations.

## `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`

This file contains evaluation-time wiring and no `Target` declarations.

## `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`

### `_CohesionValidateSdkPins`

Validate the nearest global.json before restore package collection and ordinary build preparation.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `CollectPackageReferences;PrepareForBuild` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionSkipSdkPinCheck)' != 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `ValidateCohesionSdkPinsTask` | `ProjectDirectory=$(MSBuildProjectDirectory)`; `MinimumDotNetSdkVersion=10.0.300` | None |

## `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`

### `_CollectStronglyTypedSettingsInputs`

Write a change-sensitive fingerprint containing class, namespace, and settings file paths.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionAppSettingsClass)' != ''` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `MakeDir` | `Directories=$(_StronglyTypedSettingsIntermediateOutputPath)` | None |
| `WriteLinesToFile` | `File=$(_StronglyTypedSettingsInputsCache)`; `Lines=@(_StronglyTypedSettingsInputLine)`; `Overwrite=true`; `WriteOnlyWhenDifferent=true` | None |

### `_GeneratedStronglyTypedAppSettings`

Generate the opted-in settings source only when its inputs are newer than its output.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `CoreCompile` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `_CollectStronglyTypedSettingsInputs` |
| `Condition` | `'$(CohesionAppSettingsClass)' != ''` |
| `Inputs` | `$(_StronglyTypedSettingsInputsCache);@(_AppSettings);$(_StronglyTypedSettingsTaskAssembly)` |
| `Outputs` | `$(_StronglyTypedSettingsOutput)` |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CreateStronglyTypedSettingsTask` | `AppSettingsClass=$(CohesionAppSettingsClass)`; `AppSettingsNamespace=$(CohesionAppSettingsNamespace)`; `AppSettingsFiles=@(_AppSettings)`; `AppSettingsOutputPath=$(_StronglyTypedSettingsOutput)` | None |

### `_CleanStronglyTypedAppSettings`

Delete generated settings source and fingerprint files, including files left after opt-out.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `Clean` |
| `DependsOnTargets` | Not declared |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Delete` | `Files=@(_StronglyTypedSettingsGeneratedArtifact)` | None |

## `Targets/Sdk.Image.targets`

### `CohesionValidateContainerRuntime`

Reject framework-dependent container publication with `COHSDK005`, including direct
`PublishContainer`
calls for enabled resources.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `PublishContainer;_PublishSingleContainer` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionResourceEnabled)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Condition='$(SelfContained)' != 'true' or '$(PublishSelfContained)' == 'false'`; `Code=COHSDK005`; `File=$(MSBuildProjectFullPath)`; `Text=A framework-dependent PublishContainer cannot start: Cohesion shared frameworks are NuGet Ref/Runtime packs and are absent from MCR base images. Publish self-contained; the Cohesion runtime base image is a later deliverable.` | None |

### `_CohesionResolveImage`

Validate resource enablement, sink, freshness, and ahead-of-time choices; resolve Linux runtime,
archive, base image, and child publish properties.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionValidateApplicationModel` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Condition='$(_CohesionResourceEnabled)' != 'true'`; `Text=CohesionPublishImage requires CohesionApplicationModel=enabled.` | None |
| `Error` | `Condition='$(CohesionImageFreshness)' != 'Rebuild' and '$(CohesionImageFreshness)' != 'Pinned'`; `Text=CohesionImageFreshness must be Rebuild or Pinned.` | None |
| `Error` | `Condition='$(CohesionImageFreshness)' == 'Pinned'`; `Text=Pinned images are consumed from manifest packages by the gateway CohesionPublishImages gather; a source project uses Rebuild.` | None |
| `Error` | `Condition='$(CohesionContainerPush)' != 'true' and '$(CohesionContainerPush)' != 'false'`; `Text=CohesionContainerPush must be true or false.` | None |
| `Error` | `Condition='$(_CohesionImageSelfContainedInput)' == 'false' or '$(_CohesionImagePublishSelfContainedInput)' == 'false'`; `Code=COHSDK005`; `Text=Cohesion PublishContainer requires SelfContained=true; MCR images do not contain Cohesion shared frameworks.` | None |
| `Error` | `Condition='$(CohesionImageAot)' != 'auto' and '$(CohesionImageAot)' != 'true' and '$(CohesionImageAot)' != 'false'`; `Text=CohesionImageAot must be auto, true, or false.` | None |
| `CohesionResolveImagePublish` | `Configuration=$(Configuration)`; `Aot=$(CohesionImageAot)`; `ProjectPath=$(MSBuildProjectFullPath)`; `RuntimeIdentifier=$(_CohesionImageRid)`; `Repository=$(CohesionContainerRepository)`; `Registry=$(CohesionContainerRegistry)`; `Push=$(CohesionContainerPush)`; `ImagePath=$(_CohesionImageFullPath)`; `ArchivePath=$(_CohesionArchiveFullPath)`; `BaseImage=$(CohesionContainerBaseImage)`; `InContainer=$([System.String]::Equals('$(_CohesionImageInContainer)', 'true', System.StringComparison.OrdinalIgnoreCase))` | `TaskParameter=PublishAot`; `PropertyName=_CohesionImagePublishAot`; `TaskParameter=ResolvedBaseImage`; `PropertyName=_CohesionImageBase`; `TaskParameter=RegistryAuthority`; `PropertyName=_CohesionImageRegistry`; `TaskParameter=ForwardToContainer`; `PropertyName=_CohesionImageForward` |

### `CohesionPublishImage`

Run the container extension chain and return the produced per-resource image index.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `_CohesionResolveImage;$(CohesionBuildResourceContainersDependsOn)` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | `@(_CohesionPublishedImage)` |

### `_CohesionPublishResourceImage`

Start a fresh publish process using the same restore and publish property vector, or forward to the
private in-container CLI route.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionForwardImagePublish` | `InContainer=$(_CohesionImageForward)`; `Configuration=$(Configuration)`; `ProjectPath=$(MSBuildProjectFullPath)`; `Properties=$(_CohesionImageProperties)` | None |

### `_CohesionPrepareImagePayload`

Add the resource and kind labels and endpoint container ports to the child publish.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `Publish` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionImageBuild)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

### `_CohesionCheckImageFreshness`

Publish the payload, hash published files, manifest, project imports, and container options, then
validate any cached index/archive.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `Publish` |
| `Condition` | `'$(_CohesionImageBuild)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionImageFingerprint` | `Files=@(_CohesionImageInput)`; `ImagePath=$(CohesionImageManifestPath)`; `Options=$(Configuration)\|$(RuntimeIdentifier)\|$(PublishAot)\|$(SelfContained)\|$(ContainerRepository)\|$(ContainerRegistry)\|$(ContainerArchiveOutputPath)\|$(ContainerBaseImage)\|$(ContainerImageTags)\|$(ContainerImageFormat)\|$(ContainerUser)\|$(ContainerWorkingDirectory)\|@(ContainerLabel->'%(Identity)=%(Value)')\|@(ContainerPort->'%(Identity)/%(Type)')\|@(ContainerEnvironmentVariable->'%(Identity)=%(Value)')\|@(ContainerEntrypoint)\|@(ContainerEntrypointArgs)\|@(ContainerAppCommand)\|@(ContainerAppCommandArgs)\|@(ContainerDefaultArgs)` | `TaskParameter=Fingerprint`; `PropertyName=_CohesionImageInputHash`; `TaskParameter=IsCurrent`; `PropertyName=_CohesionImageIsCurrent` |

### `_CohesionPublishImagePayload`

Invoke `PublishContainer` once unless the verified image fingerprint is current.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `_CohesionCheckImageFreshness` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Message` | `Condition='$(_CohesionImageIsCurrent)' == 'true'`; `Importance=High`; `Text=Cohesion image is current; skipping CreateNewImage (Rebuild).` | None |
| `CallTarget` | `Condition='$(_CohesionImageIsCurrent)' != 'true'`; `Targets=PublishContainer` | None |

### `_CohesionRecordPublishedImage`

Verify the SDK-produced digest and write the frozen image index and fingerprint after container
creation.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `_PublishSingleContainer` |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionImageBuild)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionVerifyImageDigest` | `Digest=$(GeneratedContainerDigest)`; `ArchivePath=$(ContainerArchiveOutputPath)`; `RegistryDigest=$(GeneratedContainerDigest)` | None |
| `CohesionWriteImageIndex` | `OutputPath=$(CohesionImageManifestPath)`; `Resource=$(_CohesionResolvedResourceName)`; `Repository=$(ContainerRepository)`; `Registry=$(ContainerRegistry)`; `Tag=$(Version)`; `Digest=$(GeneratedContainerDigest)`; `Aot=$(PublishAot)`; `BaseImage=$(ContainerBaseImage)`; `ArchivePath=$(ContainerArchiveOutputPath)`; `Fingerprint=$(_CohesionImageInputHash)` | None |

## `Targets/Sdk.Resource.targets`

### `CohesionValidateApplicationModel`

Reject unknown application-model switches and enabled projects that are not executable.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `PrepareForBuild;CoreCompile` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Condition='$(CohesionApplicationModel)' != 'disabled' and '$(CohesionApplicationModel)' != 'enabled'`; `Text=CohesionApplicationModel must be 'disabled' or 'enabled'.` | None |
| `Error` | `Condition='$(CohesionApplicationModel)' == 'enabled' and '$(_CohesionOutputTypeLower)' != 'exe'`; `Code=COHSDK008`; `File=$(MSBuildProjectFullPath)`; `Text=$(MSBuildProjectName) has CohesionApplicationModel enabled but OutputType is '$(OutputType)'; set OutputType to 'Exe'.` | None |

### `CohesionResolveResourceReferences`

Ask each resource project for `CohesionGetManifest` and collect its returned manifest metadata.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `ResolveProjectReferences` |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionResourceEnabled)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `MSBuild` | `Projects=@(_CohesionResourceProjectReference)`; `Targets=CohesionGetManifest`; `BuildInParallel=$(BuildInParallel)`; `SkipNonexistentTargets=true`; `Condition='@(_CohesionResourceProjectReference)' != ''` | `TaskParameter=TargetOutputs`; `ItemName=_CohesionReferencedManifest` |

### `CohesionCreateResourceManifest`

Validate resource metadata and write the JSON manifest, typed accessors, and control-plane
registration; register all three outputs for cleanup.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `PrepareResourceNames;CoreCompile` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionValidateApplicationModel;ResolveProjectReferences;CohesionResolveResourceReferences` |
| `Condition` | `'$(_CohesionResourceEnabled)' == 'true'` |
| `Inputs` | `$(MSBuildProjectFullPath);$(MSBuildAllProjects);@(_CohesionReferencedManifest)` |
| `Outputs` | `$(CohesionResourceManifestPath);$(CohesionResourceSourcePath);$(CohesionResourceControlPlaneSourcePath)` |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionCreateResourceManifest` | `ManifestOutputPath=$(CohesionResourceManifestPath)`; `ResourceSourceOutputPath=$(CohesionResourceSourcePath)`; `ControlPlaneSourceOutputPath=$(CohesionResourceControlPlaneSourcePath)`; `ProjectFullPath=$(MSBuildProjectFullPath)`; `ProjectName=$(MSBuildProjectName)`; `RootNamespace=$(RootNamespace)`; `AssemblyName=$(AssemblyName)`; `OutputType=$(OutputType)`; `TargetPath=$(TargetPath)`; `AppHostPath=$(TargetDir)$(TargetName)$(_NativeExecutableExtension)`; `ResourceName=$(CohesionResourceName)`; `ResourceKind=$(CohesionResourceKind)`; `ApplicationName=$(CohesionApplication)`; `ApplicationModelName=$(CohesionResourceApplicationModel)`; `ControlPlaneType=$(CohesionResourceControlPlaneType)`; `Composable=$(CohesionResourceComposable)`; `ControlPlaneEndpoint=$(CohesionControlPlaneEndpoint)`; `ControlPlanePath=$(CohesionControlPlanePath)`; `WorkloadKind=$(CohesionWorkloadKind)`; `Replicas=$(CohesionReplicas)`; `MaxReplicas=$(CohesionMaxReplicas)`; `StopGraceSeconds=$(CohesionStopGraceSeconds)`; `RestartPolicy=$(CohesionRestartPolicy)`; `Endpoints=@(CohesionEndpoint)`; `Probes=@(CohesionProbe)`; `Mounts=@(CohesionMount)`; `Settings=@(CohesionSetting)`; `Commands=@(CohesionCommand)`; `ResourceReferences=@(CohesionResourceReference)`; `ReferencedManifests=@(_CohesionReferencedManifest);@(CohesionResourceManifest)`; `ResourceProperties=@(CohesionResourceProperty)` | `TaskParameter=ResolvedResourceName`; `PropertyName=_CohesionResolvedResourceName`; `TaskParameter=ResolvedApplicationName`; `PropertyName=_CohesionResolvedApplicationName`; `TaskParameter=ResolvedResourceKind`; `PropertyName=_CohesionResolvedResourceKind` |

### `CohesionValidateManifestPackageImage`

Check for a digest-pinned image and report `COHSDK004` as a warning or error according to
CohesionImageRequired.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionPackManifest)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionValidateImageManifest` | `ImageManifestPath=$(CohesionImageManifestPath)` | `TaskParameter=HasDigestPinnedImage`; `PropertyName=_CohesionHasDigestPinnedImage` |
| `Warning` | `Condition='$(_CohesionHasDigestPinnedImage)' != 'true' and '$(CohesionImageRequired)' != 'true'`; `Code=COHSDK004`; `File=$(MSBuildProjectFullPath)`; `Text=$(MSBuildProjectName) is packing a Cohesion resource manifest without a digest-pinned image manifest at '$(CohesionImageManifestPath)'; the package will be manifest-only.` | None |
| `Error` | `Condition='$(_CohesionHasDigestPinnedImage)' != 'true' and '$(CohesionImageRequired)' == 'true'`; `Code=COHSDK004`; `File=$(MSBuildProjectFullPath)`; `Text=$(MSBuildProjectName) requires a digest-pinned image before packing because CohesionImageRequired=true; '$(CohesionImageManifestPath)' is missing or does not contain a valid SHA-256 digest.` | None |

### `CohesionPackManifest`

Stage a portable resource manifest, optionally stage an image index and archive, and add
buildTransitive props and the manifest README to the NuGet package.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionCreateResourceManifest;CohesionValidateManifestPackageImage` |
| `Condition` | `'$(_CohesionPackManifest)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionPrepareResourceManifestPackage` | `SourceManifestPath=$(CohesionResourceManifestPath)`; `PackageManifestPath=$(CohesionPackResourceManifestPath)` | None |
| `CohesionPrepareImagePackage` | `SourcePath=$(CohesionImageManifestPath)`; `OutputPath=$(_CohesionPackImageManifestPath)`; `ArchivePath=$(_CohesionPackImageArchivePath)`; `Condition=Exists('$(CohesionImageManifestPath)')` | None |

### `CohesionPackRuntimePackage`

Run a second Pack with the private runtime-pack flag and the assembly package identity when the
explicit runtime-package option is enabled.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `Pack` |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(_CohesionPackManifest)' == 'true' and '$(CohesionPackRuntime)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `MSBuild` | `Projects=$(MSBuildProjectFullPath)`; `Targets=Pack`; `BuildInParallel=false`; `Properties=_CohesionRuntimePack=true;CohesionPackRuntime=false;PackageId=$(AssemblyName);Configuration=$(Configuration);PackageOutputPath=$(PackageOutputPath)` | None |

### `CohesionGetManifest`

Return the enabled manifest plus referenced manifests, or disabled-project metadata so callers can
diagnose COHSDK001.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionValidateApplicationModel;$(_CohesionGetManifestDependsOn)` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | `@(_CohesionGetManifestResult)` |

### `CohesionGetInProcessContentItems`

Collect declared output/publish content, exclude the generated apphost, and remap files beneath
cohesion/resources/<resource-name>; add the resource manifest as .cohesion-resource.json.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionCreateResourceManifest;AssignTargetPaths;DefaultCopyToPublishDirectoryMetadata` |
| `Condition` | `'$(_CohesionResourceEnabled)' == 'true' and '$(CohesionResourceComposable)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | `@(_CohesionInProcessContentResult)` |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `AssignTargetPath` | `Files=@(_CohesionInProcessCompileCandidate)`; `RootFolder=$(MSBuildProjectDirectory)` | `TaskParameter=AssignedFiles`; `ItemName=_CohesionInProcessCompileWithTargetPath` |

### `CohesionGetInProcessRuntimeAssets`

Return eligible project, managed, native, satellite, and runtime-identifier-specific assets for an
enabled composable resource.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `ResolveReferences` |
| `Condition` | `'$(_CohesionResourceEnabled)' == 'true' and '$(CohesionResourceComposable)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | `@(_CohesionInProcessRuntimeAssetResult)` |

### `CohesionCleanResourceManifest`

Remove resource source and manifest outputs plus staged package manifests.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `Clean` |
| `DependsOnTargets` | Not declared |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Delete` | `Files=$(CohesionResourceManifestPath);$(CohesionResourceSourcePath);$(CohesionResourceControlPlaneSourcePath);$(CohesionPackResourceManifestPath);$(_CohesionPackImageManifestPath)` | None |


## Manifest package README contract

`RESOURCE_MANIFEST_README.md` is shipped as the package README. Manifest packages contain no runtime
assemblies or dependencies. HTTPS endpoints without certificate metadata default to `tls`;
generation synthesizes the absent Secret mount before composite lifting without modifying evaluated
`CohesionMount` items. Explicit names must identify Secret mounts. Reserved `public` creates no
mount and survives composite prefixing. Named certificate mounts are prefixed together with member
endpoints.

## Building the SDK package itself

Repository `sdks/Directory.Build.targets` copies entry files after `Build`, generates the frozen
version props, and extends `TargetsForTfmSpecificContentInPackage` with `CollectSdkTaskFiles` and
`CollectShippedVersionProps`. These producer operations are separate from the shipped consumer
targets above. The `Sdk/` and `Targets/` content is rooted at `CohesionSdkRootDirectory` rather than
the task project’s parent directory.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/RESOURCE_MANIFEST_README.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Directory.Build.targets`
- **Source** — `cohesion/tooling/Cli/Assimalign.Cohesion.Cli/src/Internal/CliApplication.cs`
