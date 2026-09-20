# Assimalign.Cohesion.Sdk.SecretStore.Tasks

This build-time assembly project packages the SecretStore SDK imports and targets.

The project’s `PackageId` is `Assimalign.Cohesion.Sdk.SecretStore`. Its output directory is
`$(CohesionOutputPathForSdk)\$(NETCoreSdkVersion)\sdks\$(PackageId)\Tasks`. Shared packaging places
the SDK entry files in `Sdk/`, area build files in `Targets/`, and built task-project output in
`Tasks/`.

There are no task implementation classes in this project. Its references to `Microsoft.Build`,
`Microsoft.Build.Framework`, and `Microsoft.Build.Utilities.Core` support the packaging project;
they do not establish an area task API.

- **Design** — [Packaging and shared-task boundary](design.md).
- **SDK** — [SDK home](../index.md) and [task reference](../msbuild/tasks.md).

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Tasks/src/Assimalign.Cohesion.Sdk.SecretStore.Tasks.csproj`
- **Source** — `cohesion/sdks/Directory.Build.props`
- **Source** — `cohesion/sdks/Directory.Build.targets`
