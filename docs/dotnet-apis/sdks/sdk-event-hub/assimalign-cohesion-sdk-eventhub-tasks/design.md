# Design

The EventHub task project is a packaging shell around the common SDK build pipeline.

The area owns framework selection and its declarative defaults. Execution lives in
`Assimalign.Cohesion.Sdk.Tasks`; this project does not duplicate manifest generation or define a
task API. After substituting area names, this props file is byte-identical to the `ApiManager`,
`EmailHub`, `IoTHub`, `MediaHub`, `MessageHub`, `NotificationHub` props files.

`sdks/Directory.Build.props` resolves `CohesionSdkRootDirectory` two directories above the project.
`sdks/Directory.Build.targets` uses that family root to pack `Sdk/` and `Targets/`, collects
task-project output, and emits a frozen `Build.Version.props` snapshot. Package paths deliberately
differ from the source tree’s `Tasks/src/` paths.

This family has no `Tasks/docs/OVERVIEW.md` or `Tasks/docs/DESIGN.md`; the project and imported
MSBuild files establish this limited design.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Tasks/src/Assimalign.Cohesion.Sdk.EventHub.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Targets/Sdk.EventHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Targets/Sdk.EventHub.targets`
- **Source** — `cohesion/sdks/Directory.Build.props`
- **Source** — `cohesion/sdks/Directory.Build.targets`
