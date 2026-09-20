# Tasks

Gateway tasks generate composition source and assemble ordered image indexes from resolved manifests.

These are MSBuild task types, not application runtime APIs. `[Required]` denotes a required task
argument; `[Output]` denotes a value returned to MSBuild. Target bindings supply consumer properties
and items; direct task defaults are shown separately.

## `CohesionCreateResourceVerbs`

Read project/package manifests and provider metadata, then generate application identity,
Gateway.CreateBuilder, resource verbs, externals, applications, and provider dispatch. Return
required model/client sets and composable same-application project references; these outputs do not
retroactively modify the NuGet restore graph.

`Tasks/src/Tasks/CohesionCreateResourceVerbs.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `SourceOutputPath` | `string` | Required input | `string.Empty` | Gets or sets the generated C# output path. |
| `ProjectFullPath` | `string` | Required input | `string.Empty` | Gets or sets the gateway project path used to resolve relative references. |
| `ApplicationName` | `string` | Required input | `string.Empty` | Gets or sets the application name compiled into the gateway. |
| `ResourceReferences` | `ITaskItem[]` | Optional input | `[]` | Gets or sets direct project and manifest-package references. |
| `ReferencedManifests` | `ITaskItem[]` | Optional input | `[]` | Gets or sets project and package manifests in the boundary-aware closure. |
| `ResourceKinds` | `ITaskItem[]` | Optional input | `[]` | Gets or sets area-specific typed resource verb metadata. |
| `GatewayProviders` | `ITaskItem[]` | Optional input | `[]` | Gets or sets gateway providers contributed by platform packages. |
| `Gateways` | `string` | Required input | `string.Empty` | Gets or sets the semicolon-delimited providers selected by the gateway project. |
| `InProcessEnabled` | `bool` | Optional input | Type default | Gets or sets whether project-referenced composable resources run in process. |
| `ClientKinds` | `ITaskItem[]` | Optional input | `[]` | Gets or sets mount-source and command target-kind to client-package mappings. |
| `RequiredApplicationModels` | `ITaskItem[]` | Output | `[]` | Gets the application-model packages named by referenced manifests. |
| `RequiredClientPackages` | `ITaskItem[]` | Output | `[]` | Gets the client packages required by protected mount sources and command targets. |
| `InProcessProjectReferences` | `ITaskItem[]` | Output | `[]` | Gets the enabled, composable project resources whose content is required by generated in-process bindings. |

### Errors

`COHSDK001` rejects disabled resource references. Ordinary errors reject malformed manifests,
unresolved references, invalid application/provider identifiers, duplicate providers or generated
members, invalid RequiresJit metadata, and missing type/client mappings. Missing optional provider
command-line metadata follows the constructor path.

### Tests in source

- **`GatewaySdkIntegrationTests`** —
  `Build_ApplicationSetClosure_GeneratesExternalsApplicationsAndReferences`;
  `Build_GatewaySmoke_GeneratesSurfaceAndDescribeIncludesInferredDependency`;
  `Build_GatewayReferencesDisabledResource_ReportsCOHSDK001`;
  `Build_OutOfProcessGatewayResolvesAreaHostingAssembly_ReportsCOHGW001`;
  `Build_InProcessGateway_BindsComposableProjectClosure`;
  `Build_InProcessGateway_FromCleanInParallelTwice_UsesRuntimeReferences`;
  `Publish_RidInProcessGateway_BindsTransitiveComposableProjectClosure`;
  `Build_InProcessProviderWithoutOptIn_ReportsCOHGW002`;
  `Build_InProcessOptInWithoutProvider_DoesNotActivateRuntimeClosure`.
- **`ResourceCommandSdkIntegrationTests`** —
  `Build_CommandTargets_PreservesTypedDescriptorsAndResolvesCommandClients`.
- **`TypedResourceSdkIntegrationTests`** — `Build_TypedArea_ShouldGenerateTypedDescriptorAndAddMethod`.

## `CohesionGatherImageIndexes`

Gather project and pinned-package images in declaration order. Verify and copy archives beneath
images/<ordinal>, rewrite relative paths, reject duplicate resource identities, and write
cohesion/images/v1 application.images.json. OnlyComposite selects the gateway image alone.

`Tasks/src/Tasks/CohesionGatherImageIndexes.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `Application` | `string` | Required input | `string.Empty` | Gets or sets the application identity. |
| `OutputPath` | `string` | Required input | `string.Empty` | Gets or sets the application index destination beside the gateway publish output. |
| `ProjectDirectory` | `string` | Required input | `string.Empty` | Gets or sets the gateway project directory. |
| `ResourceReferences` | `ITaskItem[]` | Optional input | `[]` | Gets or sets resource declarations in source order. |
| `ProjectImages` | `ITaskItem[]` | Optional input | `[]` | Gets or sets published project image indexes, with ProjectFullPath metadata. |
| `PackageManifests` | `ITaskItem[]` | Optional input | `[]` | Gets or sets restored package resource manifests with ReferenceIdentity metadata. |
| `CompositeImage` | `string` | Optional input | `string.Empty` | Gets or sets the composite image when InProcess is the only provider. |
| `OnlyComposite` | `bool` | Optional input | Type default | Gets or sets whether InProcess is the entire selected provider set. |

### Errors

Ordinary errors cover empty application identity, missing project/package indexes, duplicate
resource ownership, invalid indexes/digests, uncontained or missing archives, and file access
failures.

### Tests in source

- **`ImageGatherTests`** — `Publish_SourceClosure_ShouldGatherRealArchivesAsync`;
  `Gather_MemberImages_ShouldRelocateInOrderAsync`; `Gather_InProcessOnly_ShouldUseCompositeAsync`
  ; `Gather_PinnedPackage_ShouldReadItsIndexAsync`; `Gather_DuplicateResource_ShouldRejectAsync`.


The source generator also warns when no referenced resource belongs to the application; the runtime
builder rejects that empty application. `GatewaySourceWriter` and `GatewayModels` support generation
but are not independently registered tasks. Shared identifier, image-index, and file-writing helpers
are linked from the base SDK source.

[MSBuild](index.md) · [Targets](targets.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionCreateResourceVerbs.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/tests/GatewaySdkIntegrationTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/tests/ResourceCommandSdkIntegrationTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/tests/TypedResourceSdkIntegrationTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionGatherImageIndexes.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/tests/ImageGatherTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/GatewaySourceWriter.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/GatewayModels.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Assimalign.Cohesion.Sdk.Gateway.Tasks.csproj`
