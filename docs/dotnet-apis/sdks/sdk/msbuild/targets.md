# Targets

The base SDK's targets files and the hooks they add to the build.

## `Sdk/Sdk.targets`

Before importing `Microsoft.NET.Sdk/Sdk/Sdk.targets`, it sets
`DisableTransitiveFrameworkReferenceDownloads=true` when empty. For a resource-area executable it
defaults `SelfContained=true`, `RuntimeIdentifier=$(NETCoreSdkRuntimeIdentifier)`, and
`ValidateExecutableReferencesMatchSelfContained=false`, and removes `None` items that duplicate the
project's `Content` items. It then imports the three base targets files below.

## `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`

| Target | Hook | Condition |
|---|---|---|
| `_CohesionValidateSdkPins` | `BeforeTargets="CollectPackageReferences;PrepareForBuild"` | `CohesionSkipSdkPinCheck` is not `true` |

It runs `ValidateCohesionSdkPinsTask` with the project directory and `MinimumDotNetSdkVersion=10.0.300`.

## `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`

| Target | Hook | Condition |
|---|---|---|
| `_CollectStronglyTypedSettingsInputs` | Dependency of the generation target | `CohesionAppSettingsClass` is set |
| `_GeneratedStronglyTypedAppSettings` | `BeforeTargets="CoreCompile"`, incremental over the inputs cache, the settings files, and the task assembly | `CohesionAppSettingsClass` is set |

The generated `<Class>.generated.cs` lands under `$(IntermediateOutputPath)Cohesion` and is added as
`Compile` by the target that produces it.

## `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`

Resolves `CohesionProjectReference` names against `$(CohesionProjectReferences)`, turns resolved
items into `ProjectReference` items, and turns `CohesionPackageReference` into `PackageReference`.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`.
