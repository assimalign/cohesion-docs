# Tasks

The EmailHub SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.EmailHub.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.EmailHub.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-emailhub-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Tasks/src/Assimalign.Cohesion.Sdk.EmailHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Targets/Sdk.EmailHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Targets/Sdk.EmailHub.targets`.
