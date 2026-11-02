# Targets

The IdentityHub layer delegates all target execution to the base SDK.

## `Sdk/Sdk.targets`

Imports `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`, then `Targets/Sdk.IdentityHub.targets`. There are no
area `Target` declarations or hooks.

## `Targets/Sdk.IdentityHub.targets`

This file contains an empty `Project` element and contributes no build hook.

## Inherited execution

The [base target reference](../../sdk/msbuild/targets.md) lists the exact hooks: pin checks run
before package collection and build preparation, and opted-in strongly typed settings are generated
before `CoreCompile`.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
