# Tasks

The ConfigurationStore SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.ConfigurationStore.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.ConfigurationStore.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-configurationstore-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Tasks/src/Assimalign.Cohesion.Sdk.ConfigurationStore.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.targets`.
