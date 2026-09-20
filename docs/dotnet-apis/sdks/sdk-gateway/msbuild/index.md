# MSBuild

Gateway establishes its package boundary before base props and forces Composite state before base targets.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props` sets `CohesionAutoIncludeAppFramework=false`
   before its base import.
2. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` enters the base SDK.
3. `Targets/Assimalign.Cohesion.Sdk.Defaults.props` supplies empty-value defaults before Microsoft
   defaults run.
4. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
5. `Targets/Build.Version.props` loads the static version snapshot generated when the SDK was packed.
6. `Targets/Assimalign.Cohesion.Sdk.Common.props` sets the SDK marker and intermediate path.
7. `Targets/Sdk.Resource.props` registers manifest tasks and resource metadata defaults.
8. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props` registers the settings task.
9. `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` registers all frameworks and
   conditionally adds `Assimalign.Cohesion.App`.
10. `Targets/Sdk.Gateway.props` registers tasks, assigns resource/provider defaults, and creates
    fixed orchestration references. Repository builds may import the Local provider contribution
    directly.
11. The consumer project body declares application identity, providers, and resource references.
12. `Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets` forces executable Composite state, selects
    optional packages, injects the finite typed-area bootstrap, sets in-process compiler
    edges/frameworks, and performs early AOT selection. Repository builds may import InProcess
    provider props.
13. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` captures image publish inputs and supplies
    self-contained host-runtime defaults for enabled Debug resources.
14. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
15. `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` registers pin validation.
16. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` wires settings generation and cleanup.
17. `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets` imports `Sdk.Resource.targets`
    , preserves the container extension chain, then imports `Sdk.Image.targets`.
18. `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` converts Cohesion reference items.
19. `Targets/Sdk.Gateway.targets` registers source generation, AOT decisions, isolation/content
    hooks, and cleanup; its final import is `Sdk.Gateway.Images.targets`.

The `CohesionCreateResourceVerbs` target explicitly depends on
`CohesionResolveResourceReferences`, which establishes manifest availability; sharing an `AfterTargets=ResolveProjectReferences` hook alone
would not order the two.

- **Props** — [Defaults and contributed item contracts](props.md).
- **Targets** — [All execution hooks and outputs](targets.md).
- **Tasks** — [Task parameters, behavior, and tests](tasks.md).

## Property and item inventory

This index includes read-only Microsoft Build Engine (MSBuild) inputs and private state as well as
consumer options. Names beginning with an underscore are implementation details. The
[overview](../overview.md) describes supported consumer settings; this inventory records every
explicit property and item name referenced or assigned by the shipped files.

### Properties

| Name | Role | Files |
|---|---|---|
| `_CohesionGatewayAutoAotKnownSafe` | Private state | `Sdk/Sdk.targets` |
| `_CohesionGatewayControlPlaneProject` | Private state | `Sdk/Sdk.targets` |
| `_CohesionGatewayDockerSelected` | Private state | `Sdk/Sdk.targets` |
| `_CohesionGatewayInProcessActive` | Private state | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayInProcessProject` | Private state | `Sdk/Sdk.targets` |
| `_CohesionGatewayInProcessSelected` | Private state | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayKubernetesSelected` | Private state | `Sdk/Sdk.targets` |
| `_CohesionGatewayOnlyInProcess` | Private state | `Targets/Sdk.Gateway.Images.targets` |
| `BuildInParallel` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `CohesionApplicationModel` | Enable or disable resource generation. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionApplicationName` | Application identity; aliases CohesionApplication. | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `CohesionAutoIncludeAppFramework` | Suppress implicit framework references only when set to false before inclusion. | `Sdk/Sdk.props` |
| `CohesionControlPlaneEndpoint` | Endpoint used for the default control plane. | `Targets/Sdk.Gateway.props` |
| `CohesionControlPlanePath` | Absolute control-plane route prefix. | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayAdminPort` | Container port for the admin endpoint; refreshed after the project body. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionGatewayAot` | auto, true, or false; controls whole-gateway `PublishAot` selection. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props`; `Targets/Sdk.Gateway.targets` |
| `CohesionGatewayInProcess` | Explicit opt-in required alongside the InProcess provider. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props`; `Targets/Sdk.Gateway.targets` |
| `CohesionGatewayRequiresJit` | Computed from contributed provider RequiresJit metadata. | `Targets/Sdk.Gateway.targets` |
| `CohesionGateways` | Semicolon-delimited provider selection. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.props`; `Targets/Sdk.Gateway.targets` |
| `CohesionGatewaySourcePath` | Destination for generated Gateway.g.cs. | `Targets/Sdk.Gateway.targets` |
| `CohesionIsApplicationSet` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets` |
| `CohesionPlatformsVersion` | Version of selected external Docker/Kubernetes provider packages. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionRepositoryDirectory` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionResourceApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionResourceComposable` | Whether the manifest permits in-process composition. | `Targets/Sdk.Gateway.props` |
| `CohesionResourceKind` | Resource kind recorded in the manifest. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionResourceReferencesAreRuntime` | Controls resource project compilation edges for in-process mode. | `Sdk/Sdk.targets` |
| `CohesionSkipResourceApplicationModelReference` | Suppress base automatic area dependency injection. | `Targets/Sdk.Gateway.props` |
| `CohesionVersion` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionWorkloadKind` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Gateway.props` |
| `Configuration` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `IntermediateOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `IsAotCompatible` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.props` |
| `MSBuildProjectDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.Images.targets` |
| `MSBuildProjectFullPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `MSBuildProjectName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `OutputType` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets` |
| `Platform` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `PublishAot` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.targets` |
| `PublishDir` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.Images.targets` |
| `RuntimeIdentifier` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `SelfContained` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `TargetFramework` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Gateway.targets` |
| `ValidateExecutableReferencesMatchSelfContained` | MSBuild input, output, or assignment; see the file contract. | `Sdk/Sdk.targets` |

### Items

| Name | Role | Files |
|---|---|---|
| `_CohesionApplicationImageIndex` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionCompositeImage` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionForbiddenGatewayHostingReference` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayApplicationModelProject` | Private intermediate item | `Targets/Sdk.Gateway.props` |
| `_CohesionGatewayConfigurationStoreApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayConfigurationStoreClientProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayConnectionsProject` | Private intermediate item | `Targets/Sdk.Gateway.props` |
| `_CohesionGatewayDatabaseApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayDatabaseClientProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayDockerPackage` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayIdentityHubApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayInProcessContentFromResource` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayInProcessEntryAssembly` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayInProcessPackage` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayInProcessProjectReference` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayInProcessRuntimeAsset` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayJitProvider` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayKubernetesPackage` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayLogSpaceApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayRequiredApplicationModel` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayRequiredClientPackage` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_CohesionGatewayResourceHostingProject` | Private intermediate item | `Targets/Sdk.Gateway.props` |
| `_CohesionGatewayRezolvrApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayRuntimeProject` | Private intermediate item | `Targets/Sdk.Gateway.props` |
| `_CohesionGatewaySecretStoreApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewaySecretStoreClientProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionGatewayWebApplicationModelProject` | Private intermediate item | `Sdk/Sdk.targets` |
| `_CohesionImageGateway` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionImageOtherGateway` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionImageProjectCandidate` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionImageProjects` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionMemberImages` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets` |
| `_CohesionReferencedManifest` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `_CohesionResourceProjectReference` | Private intermediate item | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `_ResolvedCopyLocalBuildAssets` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `_ResolvedCopyLocalPublishAssets` | Private intermediate item | `Targets/Sdk.Gateway.targets` |
| `CohesionEndpoint` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.props`; `Targets/Sdk.Gateway.targets` |
| `CohesionGatewayPackage` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionGatewayProvider` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `CohesionGatewayResourceKind` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.props`; `Targets/Sdk.Gateway.targets` |
| `CohesionPackageReference` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionProbe` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.props` |
| `CohesionProjectReference` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets`; `Targets/Sdk.Gateway.props` |
| `CohesionResourceManifest` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `CohesionResourceProperty` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets` |
| `CohesionResourceReference` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.Images.targets`; `Targets/Sdk.Gateway.targets` |
| `Compile` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `ContentWithTargetPath` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `FileWrites` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `FrameworkReference` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.targets` |
| `NativeCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `Reference` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `ReferenceCopyLocalPaths` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `ReferencePath` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `ResourceCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `RuntimeCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |
| `RuntimeTargetsCopyLocalItems` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Gateway.targets` |

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
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
