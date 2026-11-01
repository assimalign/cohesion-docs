# Targets

The LogSpace layer delegates all target execution to the base SDK.

## `Sdk/Sdk.targets`

Imports `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`, then `Targets/Sdk.LogSpace.targets`. There are no
area `Target` declarations or hooks.

## `Targets/Sdk.LogSpace.targets`

This file contains an empty `Project` element and contributes no build hook.

## Inherited execution

The [base target reference](../../sdk/msbuild/targets.md) lists the exact hooks: pin checks run
before package collection and build preparation, and opted-in strongly typed settings are generated
before `CoreCompile`.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
