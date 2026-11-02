# Props

The base SDK's props files: defaults, the version snapshot, common properties, settings, and framework registration.

## `Targets/Assimalign.Cohesion.Sdk.Defaults.props`

Imported before `Microsoft.NET.Sdk` props, so a direct base-SDK consumer keeps the Microsoft
`OutputType=Library` default while a resource-area SDK, which sets `_CohesionResourceSdk` before the
import, selects `Exe`. It also defaults `TargetFramework=net10.0`, `LangVersion=Preview`,
`EnablePreviewFeatures=true`, `ImplicitUsings=disable`, `Nullable=enable`, and
`IsAotCompatible=true`, each only when empty.

## `Targets/Build.Version.props`

The static version snapshot written when the SDK was packed. It defines `$(CohesionVersion)` before
`FrameworkReference.props`, which needs it for the framework pack versions.

## `Targets/Assimalign.Cohesion.Sdk.Common.props`

Sets `_CohesionSdk=true` and `_CohesionIntermediateOutputPath`, and marks `appsettings*.json` to
copy to build and publish output for a resource-area executable. A consumer's own `None Update`
still wins.

## `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`

Registers `CreateStronglyTypedSettingsTask` from `Tasks/Assimalign.Cohesion.Sdk.Tasks.dll`.

## `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`

Defaults `CohesionAppFrameworkVersion` to `$(CohesionVersion)` and registers a
`KnownFrameworkReference` for `Assimalign.Cohesion.App` and each area framework, with targeting pack
`<Framework>.Ref` and runtime packs `<Framework>.Runtime.<rid>` for `win-x64`, `win-arm64`,
`linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64`, and `osx-arm64`. It includes no framework.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`.
