# Assimalign.Cohesion.Sdk.Gateway.Tasks

This packaging project packs the Gateway SDK's props and targets; it builds no task assembly.

The project's `PackageId` is `Assimalign.Cohesion.Sdk.Gateway`. It sets `CohesionSdkPackTasks=false`,
so the package carries `Sdk/` and `Targets/` (with the frozen `Build.Version.props`) and no
`Tasks/` folder. It takes no MSBuild package reference.

- **Design** — [Packaging boundary](design.md).
- **Tasks** — [Task ownership and tests](../msbuild/tasks.md).
- **SDK** — [Gateway SDK](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Assimalign.Cohesion.Sdk.Gateway.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`.
