# Assimalign.Cohesion.Sdk.Database.Tasks

The Database task assembly supplies schema compilation and migration operations for MSBuild.

The SDK registers `CompileDatabaseSchemaTask` and `CreateDatabaseMigrationTask` from
`Tasks/Assimalign.Cohesion.Sdk.Database.Tasks.dll`. `DatabaseTask` is their abstract base. The task
project depends on the SQL schema contract and compiler tooling; it does not load the SQL engine or
execute consumer entry points.

- **Design** — [Canonical schema and deterministic migration design](design.md).
- **Tasks** — [Parameters, errors, and tests](../msbuild/tasks.md).
- **SDK** — [Database SDK](../index.md).

## Sources

- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Assimalign.Cohesion.Sdk.Database.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/DESIGN.md`
