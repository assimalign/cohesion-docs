# Tasks

The Scheduler SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.Scheduler.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.Scheduler.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-scheduler-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Tasks/src/Assimalign.Cohesion.Sdk.Scheduler.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Targets/Sdk.Scheduler.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Targets/Sdk.Scheduler.targets`.
