# Assimalign.Cohesion.Sdk.Gateway.Tasks

This task assembly converts resource and provider metadata into gateway source and application image indexes.

The package places the assembly at `Tasks/Assimalign.Cohesion.Sdk.Gateway.Tasks.dll`. It implements
`CohesionCreateResourceVerbs` and `CohesionGatherImageIndexes`. Both run in the build host;
generated source supplies the application-specific composition surface.

The project links `CohesionIdentifier`, `ImageIndexFile`, and `ResourceFileWriter` from the base
task source and excludes Microsoft Build runtime assets to avoid loading a second host assembly
identity.

- **Design** — [Manifest boundaries and dependency restoration](design.md).
- **Tasks** — [Complete task contracts and source tests](../msbuild/tasks.md).
- **SDK** — [Gateway SDK](../index.md).

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Assimalign.Cohesion.Sdk.Gateway.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
