# Tasks

The MediaHub SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.MediaHub.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.MediaHub.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-mediahub-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Tasks/src/Assimalign.Cohesion.Sdk.MediaHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Targets/Sdk.MediaHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Targets/Sdk.MediaHub.targets`.
