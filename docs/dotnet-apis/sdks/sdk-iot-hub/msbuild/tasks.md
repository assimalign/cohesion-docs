# Tasks

The IoTHub SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.IoTHub.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.IoTHub.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-iothub-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Tasks/src/Assimalign.Cohesion.Sdk.IoTHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.targets`.
