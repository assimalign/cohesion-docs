# Design

The Gateway SDK's packaging project exists only to pack the SDK files.

> **Status:** Implemented.

The SDK generates nothing: it keeps its name, which the CLI's gateway discovery and consumers'
`global.json` pins rely on. Everything that describes the application is code in the gateway's
`Program.cs`.

`sdks/Directory.Build.targets` packs `Sdk/` and `Targets/` and emits the frozen
`Build.Version.props` snapshot; with `CohesionSdkPackTasks=false` it collects no task output.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Assimalign.Cohesion.Sdk.Gateway.Tasks.csproj`.
- **Source** — `cohesion/sdks/Directory.Build.targets`.
