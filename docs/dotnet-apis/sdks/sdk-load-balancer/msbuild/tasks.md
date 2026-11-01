# Tasks

The LoadBalancer SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.LoadBalancer.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.LoadBalancer.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged: pin
validation and strongly typed settings generation remain base responsibilities.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-loadbalancer-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Tasks/src/Assimalign.Cohesion.Sdk.LoadBalancer.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Targets/Sdk.LoadBalancer.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Targets/Sdk.LoadBalancer.targets`.
