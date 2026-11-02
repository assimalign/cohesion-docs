# Assimalign.Cohesion.Sdk.Tasks

This task assembly implements the base SDK's strongly typed settings generation and SDK pin validation.

The assembly is delivered inside `Assimalign.Cohesion.Sdk` under
`Tasks/Assimalign.Cohesion.Sdk.Tasks.dll`. `UsingTask` registrations select the .NET task runtime;
generated C# is added to the consumer compilation. The task assembly is a build tool, not an
application runtime dependency.

Its public tasks are `CreateStronglyTypedSettingsTask` and `ValidateCohesionSdkPinsTask`.
[Task parameters and tests](../msbuild/tasks.md) document both. No task generates application-model
sources.

- **Design** — [Build-time boundaries and generated contracts](design.md).
- **SDK** — [Base SDK](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Assimalign.Cohesion.Sdk.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Directory.Build.targets`.
