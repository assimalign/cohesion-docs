# Tasks

The IdentityHub SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.IdentityHub.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.IdentityHub.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-identityhub-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Tasks/src/Assimalign.Cohesion.Sdk.IdentityHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.targets`.
