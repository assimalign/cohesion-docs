# Targets

The ConfigurationStore layer delegates all target execution to the base SDK.

## `Sdk/Sdk.targets`

The only import is `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`. There are no area `Target`
declarations, `BeforeTargets`, `AfterTargets`, `DependsOnTargets`, `Inputs`, or `Outputs`
attributes.

## `Targets/Sdk.ConfigurationStore.targets`

This file contains an empty `Project` element. It is packaged but not imported by the area SDK. It
contributes no build hook.

## Inherited execution

The [base target reference](../../sdk/msbuild/targets.md) lists the exact hooks. Pin checks run
before package collection and build preparation; enabled resource manifests are produced before
resource naming and compilation; opted-in settings are generated before `CoreCompile`. Packing and
explicit image publishing use the same shared targets.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
