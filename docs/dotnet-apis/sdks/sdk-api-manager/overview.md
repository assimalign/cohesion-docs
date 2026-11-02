# Overview

The ApiManager SDK inherits shared build behavior and adds only its framework reference.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

## Chaining and overrides

The [base overview](../sdk/overview.md) is the inherited property, item, and diagnostic contract.
Properties conditioned on an empty value apply before the consumer project body, and later project
assignments override them.

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; the area SDK sets the private `_CohesionResourceSdk` marker before the base import. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `SelfContained` | `true` | Executables run self-contained, because the App frameworks ship only as runtime packs. | base `Sdk/Sdk.targets` |
| `RuntimeIdentifier` | `$(NETCoreSdkRuntimeIdentifier)` | Set when empty and self-contained. | base `Sdk/Sdk.targets` |
| `CohesionAutoIncludeAppFramework` | Unset; inclusion enabled | `false` before the props import suppresses both framework references. | `Sdk/Sdk.props` |
| `CohesionSkipSdkPinCheck` | Unset | Tooling-only escape for pin validation. | base `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` |

## Area properties and items

`Targets/Sdk.ApiManager.props` and `Targets/Sdk.ApiManager.targets` declare no properties, items, or targets.
`Sdk/Sdk.props` adds the `FrameworkReference` items for `Assimalign.Cohesion.App` and
`Assimalign.Cohesion.App.ApiManager` when `CohesionAutoIncludeAppFramework` is not `false`.

## Artifacts and validation

The SDK generates no artifact: there is no resource manifest and no generated resource or
control-plane source. A resource is managed by its `Program.cs` registration
(`builder.Services.AddResource`). Pin disagreement is `COHSDK002`, inherited from the base SDK;
this layer adds no validation of its own.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`.
