# Targets

The Gateway SDK adds self-contained defaults and no targets of its own.

## `Sdk/Sdk.targets`

For an executable, defaults `SelfContained=true`, `RuntimeIdentifier=$(NETCoreSdkRuntimeIdentifier)`
when self-contained, and `ValidateExecutableReferencesMatchSelfContained=false`, each only when
empty, before importing the base `Sdk.targets`, which consumes them. It then imports
`Targets/Sdk.Gateway.targets`.

## `Targets/Sdk.Gateway.targets`

An empty `Project` element. The SDK declares no `Target`, generates no source, gathers no image, and
selects no provider.

## Inherited execution

The [base target reference](../../sdk/msbuild/targets.md) lists the inherited hooks: pin validation
before package collection and build preparation, and opted-in strongly typed settings before
`CoreCompile`.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`.
