# Tasks

The NotificationHub SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.NotificationHub.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.NotificationHub.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-notificationhub-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NotificationHub/Tasks/src/Assimalign.Cohesion.Sdk.NotificationHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NotificationHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NotificationHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NotificationHub/Targets/Sdk.NotificationHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NotificationHub/Targets/Sdk.NotificationHub.targets`.
