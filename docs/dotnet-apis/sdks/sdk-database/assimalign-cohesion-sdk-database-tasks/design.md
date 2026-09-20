# Design

Database build artifacts share the SQL runtime schema contract while remaining independent of runtime execution.

> **Status:** Partial. A model-owned KeyValuePair schema package is not implemented.

## Single schema source

The retained C# `SqlSchema.Compile(name, configure)` or `SqlSchema.Create(name, configure)`
declaration is the only schema source. Roslyn binds source files against resolved compiler
references. Exactly one declaration is required because the current artifact represents one logical
database. Unsupported expressions report source locations.

The compiler lowers source into `SqlCompiledSchema` and uses `SqlCompiledSchemaSerializer` for
canonical JSON and hash parity with runtime compilation. Portable type identities use the consumer
assembly name; semantic output contains neither timestamps nor source paths. Unchanged outputs are
preserved. Invalid schemas do not replace final artifacts.

## Migration boundary

`SqlSchemaMigrationPlanner` compares the newest four-digit baseline with the desired schema. The
renderer follows planner order and the guarded `dbo` -qualified dialect. New files are written as a
pair with rollback if the second final move fails. Names are sanitized, existing paths are rejected,
and ordinals cannot exceed `9999`. Unsupported or destructive operations are errors.

## Task host and runtime boundary

Build tasks can use the JIT-based MSBuild host and Roslyn. They reference `Database.Sql.Schema`
rather than the SQL engine, storage, or TCP connection implementation. Those build dependencies do
not become application runtime dependencies through the SDK. Microsoft Build host assemblies are
excluded from the shipped task dependency closure.

The Database command advertisements remain independent: `database.add-database` and
`database.add-principal` enter the resource manifest as strings only when the application model is
enabled. Runtime application-model declarations own payloads and execution.

## Verification

The source tests compare build/runtime canonical documents and hashes, validate type/reference
semantics, require exactly one declaration, exercise model import selection, and verify SQL
migration numbering and explicit KeyValuePair failures. See [task tests](../msbuild/tasks.md).

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/DESIGN.md`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Assimalign.Cohesion.Sdk.Database.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CompileSchema.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CreateMigration.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/CompileDatabaseSchemaTaskTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/CreateDatabaseMigrationTaskTests.cs`
