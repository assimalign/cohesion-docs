# Tasks

The Rezolvr SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.Rezolvr.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.Rezolvr.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-rezolvr-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Tasks/src/Assimalign.Cohesion.Sdk.Rezolvr.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.targets`.
