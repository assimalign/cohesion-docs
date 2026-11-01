# Design

The IoTHub task project is a packaging shell around the common SDK build pipeline.

The area owns framework selection only. Execution lives in `Assimalign.Cohesion.Sdk.Tasks`; this
project defines no task API and no area defaults.

`sdks/Directory.Build.props` resolves `CohesionSdkRootDirectory` two directories above the project.
`sdks/Directory.Build.targets` uses that family root to pack `Sdk/` and `Targets/`, collects
task-project output, and emits a frozen `Build.Version.props` snapshot. Package paths deliberately
differ from the source tree's `Tasks/src/` paths.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Tasks/src/Assimalign.Cohesion.Sdk.IoTHub.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.targets`.
- **Source** — `cohesion/sdks/Directory.Build.props`.
- **Source** — `cohesion/sdks/Directory.Build.targets`.
