# Tasks

The ApiManager SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.ApiManager.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.ApiManager.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-apimanager-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Tasks/src/Assimalign.Cohesion.Sdk.ApiManager.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.targets`.
