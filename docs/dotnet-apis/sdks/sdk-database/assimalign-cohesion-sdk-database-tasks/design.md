# Design

Database build artifacts share the SQL runtime schema contract while remaining independent of runtime execution.

> **Status:** Partial. A model-owned KeyValuePair schema package is not implemented.

## Single schema source

The retained C# `SqlSchema.Compile(name, configure)` or `SqlSchema.Create(name, configure)`
declaration or inline engine-builder `database.Schema(...)` is the schema source. Roslyn binds
source files against resolved compiler references. Each declared database produces an artifact pair;
unsupported expressions report source locations.

The compiler lowers source into `SqlCompiledSchema` and uses `SqlCompiledSchemaSerializer` for
canonical JSON and hash parity with runtime compilation. Portable type identities use the consumer
assembly name; semantic output contains neither timestamps nor source paths. Unchanged outputs are
preserved. Invalid schemas do not replace final artifacts.

## Migration boundary

Runtime and SDK artifacts use `cohesion/database-schema/v12`. Migration generation refuses earlier
artifacts and baselines before writing outputs or creating their directory; rebuild them from their declarations.
Index SQL omits ASC NULLS FIRST and DESC NULLS LAST. Explicit ASC NULLS LAST and DESC NULLS FIRST
remain visible. Runtime and SDK use the same key-order mapping and renderer.

`SqlSchemaMigrationPlanner` compares the newest four-digit baseline with the desired schema. The
renderer follows planner order and the guarded `dbo` -qualified dialect. New files are written as a
pair with rollback if the second final move fails. Names are sanitized, existing paths are rejected,
and ordinals cannot exceed `9999`. Unsupported or destructive operations are errors.

## ALTER COLUMN migration files

The SDK and engine compile Sql.Schema's stateless shared MigrationLowering module. ALTER
COLUMN TYPE, DEFAULT and nullability phases therefore come from the engine's own lowering,
including rewrite classification and payload/type refusals. Physical rewrites drop an old
default before TYPE, restore the declared target default next and change nullability last.
Compatible facets keep full-definition ALTER, followed by any separate default change.
DEFAULT-only SET/DROP retains the planner's destructive-change opt-in requirement.

For each lowered ALTER COLUMN step, the SDK parses forward and rollback SQL at the engine's
expression ceiling before accepting the step. First parse errors and parser-stack refusals
use the engine's reason text; COHDBSDK206 reports failure before either file is written.
Files contain forward statements only; compensation text is validated, never written. The
SDK binding preserves engine text without structured CS diagnostics. DEFAULT admission shares
scalar-carrier, literal-only and persisted-sequence checks. Canonical literals reparse at the
ceiling and must reach a render fixed point; the engine also compares expression trees.

This guarantees parser admission for those lowered steps, not that an engine will apply every
generated file. The writer does not build an engine. Domain registrations, conversions, stored
values, default bounds and typed-literal DEFAULT kind remain runtime validation. Both per-step
writers can emit a typed-literal DEFAULT on another storage type or a TYPE without an assignment
cast; application refuses CS05013 or CS02064 respectively. Moving these checks earlier in both
writers is tracked by #1863/#1864.

CREATE TABLE, ADD COLUMN and index rendering still use separate SDK helpers, including DEFAULT
guard limits and keyword quoting (#1860); the ceiling step parser above is called by ALTER
COLUMN lowering, not every non-ALTER statement. Engine CREATE/ADD/reconciliation helper
convergence is tracked by #1861. Constraint operations and ALTER TABLE metadata retain SDK
refusals. Required column additions still need a non-null literal default or backfill.

Typed schema application publishes jointly; a file applies per statement, as its generated
header explains. Files do not reproduce runtime joint publication, receipts or applied-history
rollback classification. An empty plan writes no file (COHDBSDK204). Formats are unchanged.

## Task host and runtime boundary

Build tasks can use the JIT-based MSBuild host and Roslyn. They reference `Database.Sql.Schema`
rather than the SQL engine, storage, or TCP connection implementation. Those build dependencies do
not become application runtime dependencies through the SDK. Microsoft Build host assemblies are
excluded from the shipped task dependency closure.

The SDK emits no resource manifest. A Database program becomes a managed instance with
`builder.Services.AddResource(...)` from
[Database.Hosting.ApplicationModel](../../../resources/database/assimalign-cohesion-database-hosting-applicationmodel/index.md).
Schema compilation and migrations remain independent of management.

## Verification

The source tests compare build/runtime canonical documents and hashes, validate type/reference
semantics, exercise model import selection, and verify SQL
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
