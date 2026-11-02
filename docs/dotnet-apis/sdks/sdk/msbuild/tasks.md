# Tasks

The base SDK's two task types, their parameters, and the tests that cover them.

Both tasks ship in `Tasks/Assimalign.Cohesion.Sdk.Tasks.dll` and are registered with
`Runtime="NET"`.

## `CreateStronglyTypedSettingsTask`

| Parameter | Required | Meaning |
|---|---|---|
| `AppSettingsFiles` | No | The `appsettings*.json` inputs, merged in ordinal-ignore-case path order. |
| `AppSettingsNamespace` | Yes | Namespace of the generated types. |
| `AppSettingsClass` | Yes | Root class name. |
| `AppSettingsOutputPath` | Yes | Path of the generated C# file. |

Null values do not erase known types, integer and floating-point overlays widen to `double`, and
incompatible shapes fail the build. Generated binding uses explicit `IConfiguration.GetEntry`
calls, with no reflection, dynamic activation, or `ConfigurationBinder`; missing values preserve
property initializers.

## `ValidateCohesionSdkPinsTask`

| Parameter | Required | Meaning |
|---|---|---|
| `ProjectDirectory` | Yes | Where the walk to the nearest `global.json` starts. |
| `MinimumDotNetSdkVersion` | Yes | Lowest accepted pinned .NET SDK, `10.0.300` from the targets. |

Every present, recognized `Assimalign.Cohesion.Sdk*` pin is compared with ordinal equality; the
base pin is the anchor when present. A mismatch, or an invalid or too-old .NET SDK pin, is
`COHSDK002`. A project without a `global.json` is unchanged.

## Verification in the source repository

`SdkPinValidationTests`, `StronglyTypedSettingsSdkIntegrationTests`,
`VisualStudioMsBuildCompatibilityTests`, and `WebEventSourceSupportTests` live in the base SDK's
`Tasks/tests` project. Package-boundary tests restore SDKs from `_out/packages` and compile consumer
fixtures; their presence is not a claim that tests were run for this documentation.

[MSBuild](index.md) · [Task assembly](../assimalign-cohesion-sdk-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/CodeGeneration/CodeGenerationTask.CreateStronglyTypedSettings.cs`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Validation/ValidateCohesionSdkPinsTask.cs`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
