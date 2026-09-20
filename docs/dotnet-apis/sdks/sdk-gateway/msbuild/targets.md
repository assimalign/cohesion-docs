# Targets

Gateway targets generate composition code, preserve selected resource assets, and gather application images.

> **Status:** Partial. The restore graph still uses the explicit bootstrap and external providers require their own packages.

## Evaluation-time dependency selection

Before importing base targets, `Sdk/Sdk.targets` forces Composite/executable state, refreshes item
metadata, adds the finite ApplicationModel/client bootstrap, and selects InProcess/Docker/Kubernetes
packages. Core ApplicationModel, Gateway, resource-hosting, connections, and control-plane
dependencies are always present. The shared base converter turns Cohesion reference items into
ordinary references. No target performs an implicit second restore.

In-process activation requires both provider selection and the opt-in property. The source includes
explicit base/Web/Database framework references for that mode; this is not selective per-manifest
framework injection. The hosting isolation target checks the opt-in property directly, while
runtime-reference activation uses the two-factor state.

## Image gather entry point

```sh
dotnet publish -c Debug -p:CohesionGatewayAot=false -t:CohesionPublishImages
```

Project resources call the singular base `CohesionPublishImage`. Package manifests supply pinned
`cohesion/image.json` without a rebuild. An active InProcess-only provider set selects the composite
alone; mixed sets retain member images. Archive relocation verifies immutable digests and preserves
contained paths.

Evaluation registers targets; `DependsOnTargets` establishes execution dependencies, while
`BeforeTargets` and `AfterTargets` attach hooks. Absent `Inputs` /`Outputs` means the target
declares no timestamp-based incremental check.

## `Sdk/Sdk.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `$(CohesionRepositoryDirectory)libraries\ApplicationModel\Assimalign.Cohesion.ApplicationModel.Gateway.InProcess\buildTransitive\Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.props` | Local file | `'$(_CohesionGatewayInProcessSelected)' == 'True' and '$(CohesionRepositoryDirectory)' != '' and Exists('$(CohesionRepositoryDirectory)libraries\ApplicationModel\Assimalign.Cohesion.ApplicationModel.Gateway.InProcess\buildTransitive\Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.props')` |
| `Sdk.targets` | `Assimalign.Cohesion.Sdk` | Unconditional |
| `..\Targets\Sdk.Gateway.targets` | Local file | Unconditional |

This file contains evaluation-time wiring and no `Target` declarations.

## `Targets/Sdk.Gateway.Images.targets`

### `CohesionPublishImages`

Publish source resources, consume package image indexes without rebuilding, optionally publish a
composite image, and gather application.images.json.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionResolveResourceReferences` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | `@(_CohesionApplicationImageIndex)` |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `RemoveDuplicates` | `Inputs=@(_CohesionImageProjectCandidate)` | `TaskParameter=Filtered`; `ItemName=_CohesionImageProjects` |
| `MSBuild` | `Projects=@(_CohesionImageProjects)`; `Targets=CohesionPublishImage`; `BuildInParallel=false`; `RemoveProperties=OutDir;PublishDir;CohesionImageManifestPath;CohesionContainerArchiveOutputPath;CohesionContainerRepository;PublishAot;RuntimeIdentifier;SelfContained`; `Condition='$(_CohesionGatewayOnlyInProcess)' != 'true' and '@(_CohesionImageProjects)' != ''` | `TaskParameter=TargetOutputs`; `ItemName=_CohesionMemberImages` |
| `CallTarget` | `Targets=CohesionPublishImage`; `Condition='$(_CohesionGatewayInProcessActive)' == 'true'` | `TaskParameter=TargetOutputs`; `ItemName=_CohesionCompositeImage` |
| `CohesionGatherImageIndexes` | `Application=$(CohesionApplicationName)`; `OutputPath=$(PublishDir)application.images.json`; `ProjectDirectory=$(MSBuildProjectDirectory)`; `ResourceReferences=@(CohesionResourceReference)`; `ProjectImages=@(_CohesionMemberImages)`; `PackageManifests=@(CohesionResourceManifest->WithMetadataValue('IsSelf', 'false'))`; `CompositeImage=@(_CohesionCompositeImage)`; `OnlyComposite=$(_CohesionGatewayOnlyInProcess)` | None |

## `Targets/Sdk.Gateway.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `Sdk.Gateway.Images.targets` | Local file | Unconditional |

### `CohesionResolveGatewayAot`

Read provider RequiresJit metadata and compute the whole-gateway `PublishAot` decision.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `CollectPackageReferences;ProcessFrameworkReferences;PrepareForBuild;Publish` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

### `CohesionValidateGatewayConfiguration`

Require application identity, valid switches, and the explicit in-process opt-in for project
resources; report `COHGW002` for missing opt-in.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `PrepareForBuild;CoreCompile` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionResolveGatewayAot` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Condition='$(CohesionApplicationName)' == ''`; `Text=Sdk.Gateway requires CohesionApplicationName.` | None |
| `Error` | `Condition='$(CohesionGatewayAot)' != 'auto' and '$(CohesionGatewayAot)' != 'true' and '$(CohesionGatewayAot)' != 'false'`; `Text=CohesionGatewayAot must be auto, true, or false.` | None |
| `Error` | `Condition='$(CohesionGatewayInProcess)' != 'true' and '$(CohesionGatewayInProcess)' != 'false'`; `Text=CohesionGatewayInProcess must be true or false.` | None |
| `Error` | `Condition='$(_CohesionGatewayInProcessSelected)' == 'True' and '$(CohesionGatewayInProcess)' != 'true' and '@(_CohesionResourceProjectReference)' != ''`; `Code=COHGW002`; `File=$(MSBuildProjectFullPath)`; `Text=Sdk.Gateway selected InProcess for project resources, but CohesionGatewayInProcess is not enabled. Set CohesionGatewayInProcess=true to opt into loading the enabled, composable project-resource closure in the gateway process.` | None |
| `Message` | `Condition='$(CohesionGatewayAot)' != 'auto' or '$(CohesionGatewayRequiresJit)' == 'true'`; `Importance=High`; `Text=Sdk.Gateway AOT selection: CohesionGatewayAot=$(CohesionGatewayAot), CohesionGatewayRequiresJit=$(CohesionGatewayRequiresJit), PublishAot=$(PublishAot).` | None |

### `CohesionCreateResourceVerbs`

Read the resolved manifest closure and provider contributions, generate Gateway.g.cs, and collect
the selected in-process entry assemblies and runtime assets.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `CoreCompile` |
| `AfterTargets` | `ResolveProjectReferences` |
| `DependsOnTargets` | `CohesionResolveResourceReferences` |
| `Condition` | Not declared |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CohesionCreateResourceVerbs` | `SourceOutputPath=$(CohesionGatewaySourcePath)`; `ProjectFullPath=$(MSBuildProjectFullPath)`; `ApplicationName=$(CohesionApplicationName)`; `Gateways=$(CohesionGateways)`; `InProcessEnabled=$(_CohesionGatewayInProcessActive)`; `ResourceReferences=@(CohesionResourceReference)`; `ReferencedManifests=@(_CohesionReferencedManifest);@(CohesionResourceManifest->WithMetadataValue('IsSelf', 'false'))`; `ResourceKinds=@(CohesionGatewayResourceKind)`; `GatewayProviders=@(CohesionGatewayProvider)`; `ClientKinds=@(CohesionGatewayClientKind)` | `TaskParameter=RequiredApplicationModels`; `ItemName=_CohesionGatewayRequiredApplicationModel`; `TaskParameter=RequiredClientPackages`; `ItemName=_CohesionGatewayRequiredClientPackage`; `TaskParameter=InProcessProjectReferences`; `ItemName=_CohesionGatewayInProcessProjectReference` |
| `MSBuild` | `Projects=@(_CohesionGatewayInProcessProjectReference)`; `Targets=CohesionGetInProcessRuntimeAssets`; `BuildInParallel=$(BuildInParallel)`; `Properties=Configuration=$(Configuration);Platform=$(Platform);TargetFramework=$(TargetFramework);RuntimeIdentifier=$(RuntimeIdentifier);SelfContained=$(SelfContained)`; `SkipNonexistentTargets=false`; `RemoveProperties=OutDir;PublishDir`; `Condition='$(_CohesionGatewayInProcessActive)' == 'true' and '@(_CohesionGatewayInProcessProjectReference)' != ''` | `TaskParameter=TargetOutputs`; `ItemName=_CohesionGatewayInProcessRuntimeAsset` |
| `Message` | `Importance=Low`; `Text=Sdk.Gateway manifest application models: @(_CohesionGatewayRequiredApplicationModel).` | None |
| `Message` | `Importance=Low`; `Text=Sdk.Gateway manifest mount and command clients: @(_CohesionGatewayRequiredClientPackage).` | None |

### `CohesionAddInProcessRuntimeAssetsToPublish`

Restore the selected child runtime asset closure after publish recomputes its own package assets.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `_HandlePackageFileConflictsForPublish` |
| `AfterTargets` | `_ResolveCopyLocalAssetsForPublish` |
| `DependsOnTargets` | `CohesionCreateResourceVerbs` |
| `Condition` | `'$(_CohesionGatewayInProcessActive)' == 'true' and ('@(_CohesionGatewayInProcessEntryAssembly)' != '' or '@(_CohesionGatewayInProcessRuntimeAsset)' != '')` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

### `CohesionCollectInProcessResourceContent`

Collect isolated resource content and attach it as gateway-owned copy items with only the required
copy metadata.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `_GetCopyToOutputDirectoryItemsFromThisProject;GetCopyToPublishDirectoryItems` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionCreateResourceVerbs` |
| `Condition` | `'$(_CohesionGatewayInProcessActive)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `MSBuild` | `Projects=@(_CohesionGatewayInProcessProjectReference)`; `Targets=CohesionGetInProcessContentItems`; `BuildInParallel=$(BuildInParallel)`; `Properties=Configuration=$(Configuration);Platform=$(Platform);TargetFramework=$(TargetFramework);RuntimeIdentifier=$(RuntimeIdentifier);SelfContained=$(SelfContained)`; `SkipNonexistentTargets=false`; `RemoveProperties=OutDir;PublishDir`; `Condition='@(_CohesionGatewayInProcessProjectReference)' != ''` | `TaskParameter=TargetOutputs`; `ItemName=_CohesionGatewayInProcessContentFromResource` |

### `CohesionValidateGatewayHostingIsolation`

Reject resolved Assimalign.Cohesion.*.Hosting assemblies with `COHGW001` unless
`CohesionGatewayInProcess` is true.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | `ResolveAssemblyReferences` |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionGatewayInProcess)' != 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Condition='@(_CohesionForbiddenGatewayHostingReference)' != ''`; `Code=COHGW001`; `File=$(MSBuildProjectFullPath)`; `Text=Out-of-process Sdk.Gateway project '$(MSBuildProjectName)' resolved forbidden Hosting assembly '%(_CohesionForbiddenGatewayHostingReference.Filename)'. Remove the runtime reference or set CohesionGatewayInProcess=true for the sanctioned Composite boundary.` | None |

### `CohesionCleanGatewaySource`

Remove Gateway.g.cs after Clean.

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
| `Delete` | `Files=$(CohesionGatewaySourcePath)` | None |


[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
