# Tasks

The base task assembly validates SDK pins and generates settings, resources, packages, and image indexes.

These are MSBuild task types, not application runtime APIs. `[Required]` denotes a required task
argument; `[Output]` denotes a value returned to MSBuild. Target bindings supply consumer properties
and items; direct task defaults are shown separately.

## `CreateStronglyTypedSettingsTask`

Merge JSON shapes into public nullable types and an explicit configuration binder; preserve property
values for missing leaves and use discovered array indices. Unchanged output is not rewritten.

`Tasks/src/Tasks/CodeGeneration/CodeGenerationTask.CreateStronglyTypedSettings.cs` defines this
type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `AppSettingsFiles` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the appsettings JSON files that define the generated settings shape. |
| `AppSettingsNamespace` | `string` | Required input | `string.Empty` | Gets or sets the namespace for the generated settings types. |
| `AppSettingsClass` | `string` | Required input | `string.Empty` | Gets or sets the name of the generated root settings type. |
| `AppSettingsOutputPath` | `string` | Required input | `string.Empty` | Gets or sets the generated source output path. |

### Errors

Missing files, invalid JSON, incompatible merged types, nested arrays, and scalar/object array
mixtures produce ordinary build errors.

### Tests in source

- **`StronglyTypedSettingsSdkIntegrationTests`** —
  `Build_WithoutCohesionAppSettingsClass_GeneratesAndCompilesNothing`;
  `Build_WithCohesionAppSettingsClass_GeneratesPublicTypeAndAotSafeBind`.

## `CodeGenerationTask`

Abstract base derived from `Microsoft.Build.Utilities.Task`; it adds no parameters or execution
implementation.

`Tasks/src/Tasks/CodeGeneration/CodeGenerationTask.cs` defines this type.

There are no declared task parameters.

### Errors

No additional diagnostics.

### Tests in source

- **`StronglyTypedSettingsSdkIntegrationTests`** —
  `Build_WithoutCohesionAppSettingsClass_GeneratesAndCompilesNothing`;
  `Build_WithCohesionAppSettingsClass_GeneratesPublicTypeAndAotSafeBind`.

## `CohesionCreateResourceManifest`

Resolve resource/application identifiers; validate declared metadata and referenced manifests;
generate the manifest, resource accessors, and control-plane source. Composite manifests lift
same-application member metadata and lifecycle constraints.

`Tasks/src/Tasks/Resources/CohesionCreateResourceManifest.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `ManifestOutputPath` | `string` | Required input | `string.Empty` | Gets or sets the resource manifest output path. |
| `ResourceSourceOutputPath` | `string` | Required input | `string.Empty` | Gets or sets the generated resource accessor output path. |
| `ControlPlaneSourceOutputPath` | `string` | Required input | `string.Empty` | Gets or sets the generated control-plane registration output path. |
| `ProjectFullPath` | `string` | Required input | `string.Empty` | Gets or sets the consuming project path. |
| `ProjectName` | `string` | Required input | `string.Empty` | Gets or sets the consuming project name. |
| `RootNamespace` | `string` | Required input | `string.Empty` | Gets or sets the consuming project's root namespace. |
| `AssemblyName` | `string` | Required input | `string.Empty` | Gets or sets the consuming assembly name. |
| `OutputType` | `string` | Required input | `string.Empty` | Gets or sets the consuming project's output type. |
| `TargetPath` | `string` | Optional input | `string.Empty` | Gets or sets the consuming assembly output path. |
| `AppHostPath` | `string` | Optional input | `string.Empty` | Gets or sets the consuming executable apphost path. |
| `ResourceName` | `string` | Optional input | `string.Empty` | Gets or sets the optional resource name. |
| `ResourceKind` | `string` | Required input | `string.Empty` | Gets or sets the resource kind. |
| `ApplicationName` | `string` | Optional input | `string.Empty` | Gets or sets the optional application name. |
| `ApplicationModelName` | `string` | Optional input | `string.Empty` | Gets or sets the optional application-model assembly name. |
| `ControlPlaneType` | `string` | Optional input | `string.Empty` | Gets or sets the optional fully qualified area default control-plane type. |
| `Composable` | `bool` | Optional input | `true` | Gets or sets whether the resource kind supports in-process composition. |
| `ControlPlaneEndpoint` | `string` | Required input | `string.Empty` | Gets or sets the area default control-plane endpoint. |
| `ControlPlanePath` | `string` | Required input | `string.Empty` | Gets or sets the area default control-plane path. |
| `WorkloadKind` | `string` | Optional input | `string.Empty` | Gets or sets the optional workload kind. |
| `Replicas` | `int` | Optional input | `1` | Gets or sets the default replica count. |
| `MaxReplicas` | `string` | Optional input | `string.Empty` | Gets or sets the optional maximum replica count. |
| `StopGraceSeconds` | `int` | Optional input | `30` | Gets or sets the graceful-stop budget in seconds. |
| `RestartPolicy` | `string` | Optional input | `"OnFailure"` | Gets or sets the restart policy. |
| `Endpoints` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the declared endpoints. |
| `Probes` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the declared probes. |
| `Mounts` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the declared mounts. |
| `Settings` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the declared settings. |
| `Commands` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the command kinds accepted by the resource's default control plane. |
| `ResourceReferences` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the declared resource references. |
| `ReferencedManifests` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the manifests returned by referenced projects or packages. |
| `ResourceProperties` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the kind-prefixed resource properties. |
| `ResolvedResourceName` | `string` | Output | `string.Empty` | Gets the resolved resource name. |
| `ResolvedApplicationName` | `string` | Output | `string.Empty` | Gets the resolved application name. |
| `ResolvedResourceKind` | `string` | Output | `string.Empty` | Gets the resolved resource kind. |

### Errors

`COHSDK001`, `COHSDK008`, `COHSDK009`, and `COHSDK010` cover disabled references, output type,
kind
prefixes, and certificate bindings. Ordinary errors cover invalid identifiers, unknown metadata,
ports, schemes, probes, mount kinds/paths, command names, lifecycle constraints, and manifest
structure.

### Tests in source

- **`ResourceManifestSdkIntegrationTests`** —
  `Build_MinimalResource_ShouldRestoreAndGenerateWithoutWarningsAsync`;
  `Build_EnabledWebReferencesEnabledDatabase_GeneratesValidManifestsAndTypedAccessors`;
  `Build_EnabledWebWithNamespacedExplicitMain_ShouldSucceed`;
  `Build_EnabledGenericResource_GeneratesAccessorsAndInertControlPlaneSource`;
  `Build_DisabledApplicationModel_ProducesNoManifestOrGeneratedSources`;
  `Build_ResourceReferenceTargetsDisabledProject_ReportsCOHSDK001`;
  `Build_EnabledApplicationModelUsesLibraryOutput_ReportsCOHSDK008`;
  `Build_ResourcePropertyUsesForeignKindPrefix_ReportsCOHSDK009`;
  `Build_EndpointUsesUnknownMetadata_ReportsItemAndMetadataNames`.
- **`ResourceCommandSdkIntegrationTests`** — `Build_ConfigurationStore_EmitsDefaultCommandStrings`;
  `Build_CommandItems_EmitsSortedUniqueStrings`;
  `Build_CommandWithPayloadMetadata_RejectsUnsupportedMetadata`.
- **`CertificateSdkTests`** — `Build_CompositePublicCertificate_ShouldPreserveReservedLiteral`;
  `Build_HttpsCertificate_ShouldGenerateMountAndRegistration`;
  `Build_CertificateContract_ShouldDiagnoseInvalidMetadata`.
- **`ResourceAreaDefaultsTests`** — `HttpsAreaDefaults_ShouldDeclareCertificateAndSecretMount`;
  `AreaDefaults_ShouldDeclareFactoryAndMatchingProbes`.

## `CohesionForwardImagePublish`

Start dotnet publish in a fresh process or cohesion publish --in-container. Merge outer global
properties with the resolved property vector; pass arguments without a shell, escaping MSBuild
percent and list delimiters.

`Tasks/src/Tasks/Resources/CohesionForwardImagePublish.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `ProjectPath` | `string` | Required input | `string.Empty` | Gets or sets the consumer project. |
| `Properties` | `ITaskItem[]` | Optional input | `[]` | Gets or sets the complete property vector to preserve across the CLI boundary. |
| `InContainer` | `bool` | Optional input | Type default | Gets or sets whether to use the in-container CLI route instead of a host dotnet publish. |
| `Configuration` | `string` | Optional input | `string.Empty` | Gets or sets the configuration for the Release-only unavailable-route diagnostic. |

### Errors

Failed or unstartable Release in-container processes report `COHSDK003`; other process failures are
ordinary errors.

### Tests in source

- **`ImagePublishTests`** — `Publish_DebugArchive_ShouldVerifyPayloadAndFreshnessAsync`;
  `Publish_InvalidInput_ShouldRejectAsync`; `Publish_UncontainedArchive_ShouldRejectAsync`;
  `Publish_FrameworkDependent_ShouldRejectAsync`; `Publish_ReleaseFalse_ShouldRejectDeviationAsync`
  ; `Publish_ReleaseNonLinux_ShouldRejectUnavailableRouteAsync`;
  `Targets_SinkSelection_ShouldRemainExclusive`;
  `Guard_RawSdkFlags_ShouldNotRequireOrganizationAsync`;
  `Write_RegistrySink_ShouldOmitArchiveAsync`; `Verify_InvalidDigest_ShouldRejectAsync`.

## `CohesionImageFingerprint`

Hash sorted distinct input paths, file contents, and container options. Reuse an image only when its
.inputs fingerprint, .sha256 index hash, and archive digest or registry identity remain valid.

`Tasks/src/Tasks/Resources/CohesionImageFingerprint.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `Files` | `ITaskItem[]` | Optional input | `[]` | Gets or sets files contributing to the published payload and manifest. |
| `Options` | `string` | Optional input | `string.Empty` | Gets or sets evaluated container options and metadata. |
| `ImagePath` | `string` | Required input | `string.Empty` | Gets or sets the image index path. |
| `Fingerprint` | `string` | Output | `string.Empty` | Gets the current publish-input fingerprint. |
| `IsCurrent` | `bool` | Output | Type default | Gets whether verified current image outputs may be reused. |

### Errors

Failure before producing a fingerprint is an error; a broken cache after hashing is logged as a
cache miss and causes image recreation.

### Tests in source

- **`ImagePublishTests`** — `Publish_DebugArchive_ShouldVerifyPayloadAndFreshnessAsync`;
  `Publish_InvalidInput_ShouldRejectAsync`; `Publish_UncontainedArchive_ShouldRejectAsync`;
  `Publish_FrameworkDependent_ShouldRejectAsync`; `Publish_ReleaseFalse_ShouldRejectDeviationAsync`
  ; `Publish_ReleaseNonLinux_ShouldRejectUnavailableRouteAsync`;
  `Targets_SinkSelection_ShouldRemainExclusive`;
  `Guard_RawSdkFlags_ShouldNotRequireOrganizationAsync`;
  `Write_RegistrySink_ShouldOmitArchiveAsync`; `Verify_InvalidDigest_ShouldRejectAsync`.

## `CohesionPrepareImagePackage`

Stage the image index for packaging. When an archive is supplied, verify it is the published archive
and rewrite archive to images/<filename>; otherwise omit archive. Preserve the publish index.

`Tasks/src/Tasks/Resources/CohesionPrepareImagePackage.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `SourcePath` | `string` | Required input | `string.Empty` | Gets or sets the source image document. |
| `OutputPath` | `string` | Required input | `string.Empty` | Gets or sets the staged package document. |
| `ArchivePath` | `string` | Optional input | `string.Empty` | Gets or sets the archive selected for the air-gapped package, or empty when not packing one. |

### Errors

Reject stale/different archive paths, digest failures, invalid JSON, or filesystem failures with
ordinary build errors.

### Tests in source

- **`ResourceManifestPackIntegrationTests`** — `Pack_DisabledApplicationModel_KeepsNormalPacking`;
  `Pack_EnabledResource_ProducesPortableManifestPackageAndRegistersConsumerItem`;
  `Pack_ImageRequiredWithoutDigest_FailsWithCOHSDK004`;
  `Pack_RuntimeOptIn_ProducesManifestAndLibraryPackages`;
  `Pack_ImageAndArchivePresent_IncludesStagedAssets`.

## `CohesionPrepareResourceManifestPackage`

Create a portable copy of the manifest, writing null for artifact.project and artifact.apphost while
preserving other fields; avoid replacing unchanged bytes.

`Tasks/src/Tasks/Resources/CohesionPrepareResourceManifestPackage.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `SourceManifestPath` | `string` | Required input | `string.Empty` | Gets or sets the build-time resource manifest path. |
| `PackageManifestPath` | `string` | Required input | `string.Empty` | Gets or sets the portable package manifest output path. |

### Errors

Invalid JSON, non-object root/artifact, missing artifact, and file access failures produce ordinary
build errors.

### Tests in source

- **`ResourceManifestPackIntegrationTests`** — `Pack_DisabledApplicationModel_KeepsNormalPacking`;
  `Pack_EnabledResource_ProducesPortableManifestPackageAndRegistersConsumerItem`;
  `Pack_ImageRequiredWithoutDigest_FailsWithCOHSDK004`;
  `Pack_RuntimeOptIn_ProducesManifestAndLibraryPackages`;
  `Pack_ImageAndArchivePresent_IncludesStagedAssets`.

## `CohesionResolveImagePublish`

Validate Debug/Release, image options, repository, registry prefix, and archive containment. Select
the runtime-deps base, NativeAOT decision, and native or container-forwarding route.

`Tasks/src/Tasks/Resources/CohesionResolveImagePublish.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `Configuration` | `string` | Optional input | `string.Empty` | Gets or sets the build configuration. |
| `Aot` | `string` | Optional input | `"auto"` | Gets or sets auto, true, or false. |
| `ProjectPath` | `string` | Required input | `string.Empty` | Gets or sets the owning project path. |
| `RuntimeIdentifier` | `string` | Optional input | `"linux-x64"` | Gets or sets the selected Linux runtime identifier. |
| `Repository` | `string` | Optional input | `string.Empty` | Gets or sets the repository. |
| `Registry` | `string` | Optional input | `string.Empty` | Gets or sets the configured registry and optional organization prefix. |
| `Push` | `bool` | Optional input | Type default | Gets or sets whether this build pushes its image. |
| `ImagePath` | `string` | Required input | `string.Empty` | Gets or sets the image index destination. |
| `ArchivePath` | `string` | Optional input | `string.Empty` | Gets or sets the archive destination. |
| `BaseImage` | `string` | Optional input | `"auto"` | Gets or sets the requested base or auto. |
| `InContainer` | `bool` | Optional input | Type default | Gets or sets the private CLI in-container request. |
| `PublishAot` | `bool` | Output | Type default | Gets the NativeAOT selection. |
| `ResolvedBaseImage` | `string` | Output | `string.Empty` | Gets the concrete base image. |
| `RegistryAuthority` | `string` | Output | `string.Empty` | Gets the authority for the registry sink, or empty for archives. |
| `ForwardToContainer` | `bool` | Output | Type default | Gets whether the caller must forward through cohesion publish --in-container. |

### Errors

Release capability failures are COHSDK003. Invalid runtime identifiers, Release Aot=false, illegal
options, repositories, registry prefixes, or archive locations produce ordinary errors. The
in-container image/mount/command contract is not implemented.

### Tests in source

- **`ImagePublishTests`** — `Publish_DebugArchive_ShouldVerifyPayloadAndFreshnessAsync`;
  `Publish_InvalidInput_ShouldRejectAsync`; `Publish_UncontainedArchive_ShouldRejectAsync`;
  `Publish_FrameworkDependent_ShouldRejectAsync`; `Publish_ReleaseFalse_ShouldRejectDeviationAsync`
  ; `Publish_ReleaseNonLinux_ShouldRejectUnavailableRouteAsync`;
  `Targets_SinkSelection_ShouldRemainExclusive`;
  `Guard_RawSdkFlags_ShouldNotRequireOrganizationAsync`;
  `Write_RegistrySink_ShouldOmitArchiveAsync`; `Verify_InvalidDigest_ShouldRejectAsync`.

## `CohesionValidateImageManifest`

Return HasDigestPinnedImage=false for missing, malformed, or non-digest JSON. Accept sha256: plus 64
hexadecimal characters. The calling target decides warning versus error.

`Tasks/src/Tasks/Resources/CohesionValidateImageManifest.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `ImageManifestPath` | `string` | Required input | `string.Empty` | Gets or sets the image manifest path to validate. |
| `HasDigestPinnedImage` | `bool` | Output | Type default | Gets whether the image manifest contains a valid SHA-256 digest. |

### Errors

File access failures are errors; `COHSDK004` is emitted by the target, not this task.

### Tests in source

- **`ResourceManifestPackIntegrationTests`** — `Pack_DisabledApplicationModel_KeepsNormalPacking`;
  `Pack_EnabledResource_ProducesPortableManifestPackageAndRegistersConsumerItem`;
  `Pack_ImageRequiredWithoutDigest_FailsWithCOHSDK004`;
  `Pack_RuntimeOptIn_ProducesManifestAndLibraryPackages`;
  `Pack_ImageAndArchivePresent_IncludesStagedAssets`.

## `CohesionVerifyImageDigest`

Validate the generated SHA-256 digest and compare it with archive manifest bytes/descriptors or the
registry-returned digest.

`Tasks/src/Tasks/Resources/CohesionVerifyImageDigest.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `Digest` | `string` | Required input | `string.Empty` | Gets or sets GeneratedContainerDigest from the SDK container task. |
| `ArchivePath` | `string` | Optional input | `string.Empty` | Gets or sets the selected OCI archive, or empty for a registry sink. |
| `RegistryDigest` | `string` | Optional input | `string.Empty` | Gets or sets the digest returned by the successful registry publish. |

### Errors

Malformed or mismatched digests and unreadable/invalid archives produce ordinary build errors.

### Tests in source

- **`ImagePublishTests`** — `Publish_DebugArchive_ShouldVerifyPayloadAndFreshnessAsync`;
  `Publish_InvalidInput_ShouldRejectAsync`; `Publish_UncontainedArchive_ShouldRejectAsync`;
  `Publish_FrameworkDependent_ShouldRejectAsync`; `Publish_ReleaseFalse_ShouldRejectDeviationAsync`
  ; `Publish_ReleaseNonLinux_ShouldRejectUnavailableRouteAsync`;
  `Targets_SinkSelection_ShouldRemainExclusive`;
  `Guard_RawSdkFlags_ShouldNotRequireOrganizationAsync`;
  `Write_RegistrySink_ShouldOmitArchiveAsync`; `Verify_InvalidDigest_ShouldRejectAsync`.

## `CohesionWriteImageIndex`

Write cohesion/image/v1 JSON, its .inputs fingerprint, and .sha256 hash sidecars. Use linux/amd64,
nullable registry/tag, and omit archive for a registry sink.

`Tasks/src/Tasks/Resources/CohesionWriteImageIndex.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `OutputPath` | `string` | Required input | `string.Empty` | Gets or sets the output document path. |
| `Resource` | `string` | Required input | `string.Empty` | Gets or sets the owning resource name. |
| `Repository` | `string` | Required input | `string.Empty` | Gets or sets the repository without its registry. |
| `Registry` | `string` | Optional input | `string.Empty` | Gets or sets the authority pinned by a successful push, or empty for late binding. |
| `Tag` | `string` | Optional input | `string.Empty` | Gets or sets the optional human-readable version. |
| `Digest` | `string` | Required input | `string.Empty` | Gets or sets the verified immutable digest. |
| `Aot` | `bool` | Optional input | Type default | Gets or sets whether the entry point is NativeAOT. |
| `BaseImage` | `string` | Required input | `string.Empty` | Gets or sets the base-image identity used by the publisher. |
| `ArchivePath` | `string` | Optional input | `string.Empty` | Gets or sets the archive produced in this build, or empty for a registry sink. |
| `Fingerprint` | `string` | Optional input | `string.Empty` | Gets or sets the evaluated publish-input fingerprint for incremental reuse. |

### Errors

Reject invalid identities, invalid digest, a late-bound image without an archive, uncontained
archive paths, or filesystem failures.

### Tests in source

- **`ImagePublishTests`** — `Publish_DebugArchive_ShouldVerifyPayloadAndFreshnessAsync`;
  `Publish_InvalidInput_ShouldRejectAsync`; `Publish_UncontainedArchive_ShouldRejectAsync`;
  `Publish_FrameworkDependent_ShouldRejectAsync`; `Publish_ReleaseFalse_ShouldRejectDeviationAsync`
  ; `Publish_ReleaseNonLinux_ShouldRejectUnavailableRouteAsync`;
  `Targets_SinkSelection_ShouldRemainExclusive`;
  `Guard_RawSdkFlags_ShouldNotRequireOrganizationAsync`;
  `Write_RegistrySink_ShouldOmitArchiveAsync`; `Verify_InvalidDigest_ShouldRejectAsync`.

## `ValidateCohesionSdkPinsTask`

Walk upward to the nearest global.json. Parse JSON with comments and trailing commas; compare
present recognized SDK pins exactly, using the base pin as anchor or the first recognized identity
in ordinal name order. Validate a non-empty string .NET SDK pin against `MinimumDotNetSdkVersion`.

`Tasks/src/Tasks/Validation/ValidateCohesionSdkPinsTask.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `ProjectDirectory` | `string` | Required input | `string.Empty` | Gets or sets the consumer project directory from which the global.json search begins. |
| `MinimumDotNetSdkVersion` | `string` | Required input | `string.Empty` | Gets or sets the minimum supported .NET SDK version. |

### Errors

`COHSDK002` reports version disagreement, invalid/old platform pins, malformed JSON, or file access
errors. No file succeeds without checking.

### Tests in source

- **`SdkPinValidationTests`** —
  `Restore_WithDisagreeingThenCorrectedPins_ReportsCOHSDK002ThenBuildSucceeds`;
  `Build_WithSkipProperty_BypassesSdkPinValidation`;
  `Build_WithMatchingLocalPinIdentities_Succeeds`;
  `Build_WithDotNetSdkBelowMinimum_ReportsCOHSDK002`;
  `Build_WithoutGlobalJson_SucceedsWithoutPinValidation`.


The test names above describe repository coverage; no SDK build or test was run while writing these
pages. Internal helpers include `ResourceFileWriter`, `ResourceManifestWriter`,
`ResourceSourceWriter`, `ImageIndexFile`, and `CohesionIdentifier`; they are not separately
registered MSBuild tasks.

[MSBuild](index.md) · [Targets](targets.md)

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/CodeGeneration/CodeGenerationTask.CreateStronglyTypedSettings.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/StronglyTypedSettingsSdkIntegrationTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/CodeGeneration/CodeGenerationTask.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionCreateResourceManifest.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceManifestSdkIntegrationTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceCommandSdkIntegrationTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/CertificateSdkTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceAreaDefaultsTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionForwardImagePublish.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ImagePublishTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionImageFingerprint.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionPrepareImagePackage.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceManifestPackIntegrationTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionPrepareResourceManifestPackage.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionResolveImagePublish.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionValidateImageManifest.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionVerifyImageDigest.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Resources/CohesionWriteImageIndex.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Validation/ValidateCohesionSdkPinsTask.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/SdkPinValidationTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
