# Assimalign.Cohesion.Sdk.Tasks

This task assembly implements the shared SDK generation, validation, packaging, and image operations.

The assembly is delivered inside `Assimalign.Cohesion.Sdk` under
`Tasks/Assimalign.Cohesion.Sdk.Tasks.dll`. `UsingTask` registrations select the .NET task runtime;
generated C# is added to the consumer compilation. The task assembly is a build tool, not an
application runtime dependency.

Its public tasks cover settings shape generation, resource manifests and portable packages, SDK pin
agreement, image capability selection, process forwarding, fingerprints, digest verification, image
indexes, and image-package staging. [Task parameters and tests](../msbuild/tasks.md) enumerate every
task type.

- **Design** — [Build-time boundaries and generated contracts](design.md).
- **SDK** — [Base SDK](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Assimalign.Cohesion.Sdk.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Directory.Build.targets`
