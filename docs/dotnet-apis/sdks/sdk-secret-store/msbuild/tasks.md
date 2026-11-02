# Tasks

The SecretStore SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.SecretStore.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.SecretStore.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-secretstore-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Tasks/src/Assimalign.Cohesion.Sdk.SecretStore.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.targets`.
