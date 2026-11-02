# Design

The NatGateway task project is a packaging shell around the common SDK build pipeline.

The area owns framework selection only. Execution lives in `Assimalign.Cohesion.Sdk.Tasks`; this
project defines no task API and no area defaults.

`sdks/Directory.Build.props` resolves `CohesionSdkRootDirectory` two directories above the project.
`sdks/Directory.Build.targets` uses that family root to pack `Sdk/` and `Targets/`, collects
task-project output, and emits a frozen `Build.Version.props` snapshot. Package paths deliberately
differ from the source tree's `Tasks/src/` paths.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Tasks/src/Assimalign.Cohesion.Sdk.NatGateway.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.targets`.
- **Source** — `cohesion/sdks/Directory.Build.props`.
- **Source** — `cohesion/sdks/Directory.Build.targets`.
