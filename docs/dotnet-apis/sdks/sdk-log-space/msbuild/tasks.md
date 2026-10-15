# Tasks

The LogSpace SDK uses shared base tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.LogSpace.Tasks` has a
`Tasks/src/Assimalign.Cohesion.Sdk.LogSpace.Tasks.csproj` packaging project but no C# task
implementation files. No area `UsingTask` is registered. The empty `Targets/Sdk.LogSpace.targets`
adds no task invocation.

## Shared task contract

[Base task parameters, behavior, and errors](../../sdk/msbuild/tasks.md) apply unchanged. Area
values flow into `CohesionCreateResourceManifest` through the inherited resource targets; settings,
pins, image publishing, and manifest packing also remain base responsibilities.

## Verification in the source repository

`ResourceAreaDefaultsTests` checks the area default control-plane factories, matching probes, and
HTTPS certificate mounts in the base SDK test project. This family contains no local `Tasks/tests`
suite. The shared integration tests cover manifest generation and error cases; their presence is not
a claim that tests were run for this documentation change.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-logspace-tasks/index.md)

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Tasks/src/Assimalign.Cohesion.Sdk.LogSpace.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceAreaDefaultsTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/ResourceManifestSdkIntegrationTests.cs`
