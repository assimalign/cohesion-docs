# Overview

The base SDK's consumer properties, items, and diagnostics, inherited by every layered SDK.

> **Status:** Implemented.

## Project defaults

Every default is conditional on an empty value. Command-line global properties are honored, and
later assignments in a consumer project override the props-time values.

| Property | Default | Defined in |
|---|---|---|
| `OutputType` | `Library` from `Microsoft.NET.Sdk`; `Exe` when an area SDK set `_CohesionResourceSdk` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `DisableTransitiveFrameworkReferenceDownloads` | `true` | `Sdk/Sdk.targets` |
| `SelfContained`, `RuntimeIdentifier` | `true` and `$(NETCoreSdkRuntimeIdentifier)` for a resource-area executable | `Sdk/Sdk.targets` |

Resource-area executables run self-contained because the App frameworks ship only as NuGet runtime
packs, never under the dotnet root's `shared/` folder.

## Consumer properties

| Property | Default | Effect |
|---|---|---|
| `CohesionAutoIncludeAppFramework` | Unset; inclusion enabled | `false` suppresses a resource-area SDK's two implicit framework references. |
| `CohesionAppFrameworkVersion` | `$(CohesionVersion)` | Version of every registered App framework pack. |
| `CohesionAppSettingsClass` | Unset | Opts into strongly typed settings generation and names the root class. |
| `CohesionAppSettingsNamespace` | `$(RootNamespace)` | Namespace of the generated settings class. |
| `CohesionSkipSdkPinCheck` | Unset | Tooling-only escape for pin validation. |

## Items

| Item | Effect |
|---|---|
| `CohesionProjectReference` | Name-only project reference; resolvable items become ordinary `ProjectReference` items. |
| `CohesionPackageReference` | Becomes an ordinary `PackageReference`. |
| `KnownFrameworkReference` | The base registers `Assimalign.Cohesion.App` and every `Assimalign.Cohesion.App.<Area>` framework but includes none. |
| `None Update="appsettings*.json"` | A resource-area executable copies its settings files to build and publish output. |

## Generated artifacts

With `CohesionAppSettingsClass` set, the SDK generates one strongly typed settings source from
`appsettings*.json` before `CoreCompile`, with an AOT-safe `Bind(IConfiguration)` made from explicit
configuration reads. No SDK in the family generates a resource manifest, a resource accessor, a
control-plane registration, an image index, or a gateway surface.

## Diagnostics

| Code | Severity | Meaning |
|---|---|---|
| `COHSDK002` | Error | Cohesion SDK pins in the nearest `global.json` disagree, or the pinned .NET SDK is invalid or older than `10.0.300`. |

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
