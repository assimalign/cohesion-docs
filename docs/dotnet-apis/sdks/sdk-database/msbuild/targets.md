# Targets

Database targets select a fixed model compiler before C# compilation and expose explicit migration generation.

> **Status:** Partial. The KeyValuePair model target currently invokes a compiler that rejects that model.

## Selection and incremental behavior

Model imports use ordinal string equality with fixed paths; consumer text never becomes an arbitrary
import path. Both model files set `_CohesionDatabaseModelTargetsLoaded=true` and their own
`_CohesionDatabaseModelCompileTarget`.

The model compiler tracks `Compile`, `ReferencePath`, and the task assembly as `Inputs`, with
per-database artifacts tracked by its output record. The outer target exposes `CohesionDatabaseSchema`
items with `DatabaseName` and `HashPath` metadata, and tracks artifacts with `FileWrites`. The targets do not declare properties such as
`DefineConstants` as separate timestamp inputs.

Evaluation registers targets; `DependsOnTargets` establishes execution dependencies, while
`BeforeTargets` and `AfterTargets` attach hooks. Absent `Inputs` /`Outputs` means the target
declares no timestamp-based incremental check.

## `Sdk/Sdk.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `Sdk.targets` | `Assimalign.Cohesion.Sdk` | Unconditional |
| `..\Targets\Assimalign.Cohesion.Sdk.Database.Migration.targets` | Local file | Unconditional |

This file contains evaluation-time wiring and no `Target` declarations.

## `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`

| Imported project | SDK | Condition |
|---|---|---|
| `$(MSBuildThisFileDirectory)Sdk.Database.Sql.targets` | Local file | `$([System.String]::Equals('$(CohesionDatabaseModel)', 'Sql', System.StringComparison.Ordinal))` |
| `$(MSBuildThisFileDirectory)Sdk.Database.KeyValuePair.targets` | Local file | `$([System.String]::Equals('$(CohesionDatabaseModel)', 'KeyValuePair', System.StringComparison.Ordinal))` |

### `_CohesionDatabaseValidateModel`

Report `COHDBSDK001` for an unknown or case-mismatched model and `COHDBSDK002` for a loaded tool set
without a compile target.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionDatabaseProject)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `Error` | `Code=COHDBSDK001`; `Condition='$(_CohesionDatabaseModelTargetsLoaded)' != 'true'`; `Text=Database model '$(CohesionDatabaseModel)' is not supported by Assimalign.Cohesion.Sdk.Database. Supported models are 'Sql' and 'KeyValuePair'; model names are case-sensitive.` | None |
| `Error` | `Code=COHDBSDK002`; `Condition='$(_CohesionDatabaseModelCompileTarget)' == ''`; `Text=Database model tool set '$(CohesionDatabaseModel)' did not register a schema compile target. Set _CohesionDatabaseModelCompileTarget in Sdk.Database.$(CohesionDatabaseModel).targets.` | None |

### `CohesionDatabaseCompileSchema`

Resolve references, validate the model, check recorded artifacts, and invoke the selected compiler.
Then expose each declared schema as a `CohesionDatabaseSchema` item. Missing recorded artifacts
invalidate the incremental output so compilation runs again.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | `CoreCompile` |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `ResolveReferences;_CohesionDatabaseValidateModel;_CohesionDatabaseCheckSchemaArtifacts;$(_CohesionDatabaseModelCompileTarget)` |
| `Condition` | `'$(CohesionDatabaseProject)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| Schema items | Per-database artifact paths under `CohesionDatabaseSchemaOutputDirectory` | `CohesionDatabaseSchema`, with `DatabaseName` and `HashPath` metadata |

### `CohesionDatabaseCreateMigration`

Explicitly create an ordered SQL migration and canonical baseline after compiling the desired
schema.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | `CohesionDatabaseCompileSchema` |
| `Condition` | `'$(CohesionDatabaseProject)' == 'true'` |
| `Inputs` | Not declared |
| `Outputs` | Not declared |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CreateDatabaseMigrationTask` | `SchemaDirectory=$(CohesionDatabaseSchemaOutputDirectory)`; `DatabaseName=$(CohesionDatabaseName)`; `MigrationsRoot=$(CohesionDatabaseMigrationsRoot)`; `MigrationName=$(CohesionDatabaseMigrationName)`; `Model=$(CohesionDatabaseModel)`; `ProjectDirectory=$(MSBuildProjectDirectory)` | None |

## `Targets/Sdk.Database.KeyValuePair.targets`

### `_CohesionCompileKeyValuePairDatabaseSchema`

Invoke the compiler with KeyValuePair; this path currently fails with `COHDBSDK106` because no
model-owned schema package exists.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionDatabaseProject)' == 'true'` |
| `Inputs` | `@(Compile);@(ReferencePath);$(_CohesionDatabaseTaskAssembly)` |
| `Outputs` | `$(_CohesionDatabaseSchemaManifest)` |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CompileDatabaseSchemaTask` | `SourceFiles=@(Compile)`; `ReferencePaths=@(ReferencePath)`; `DefineConstants=$(DefineConstants)`; `LanguageVersion=$(LangVersion)`; `AssemblyName=$(AssemblyName)`; `Model=KeyValuePair`; `ProjectDirectory=$(MSBuildProjectDirectory)`; `OutputDirectory=$(CohesionDatabaseSchemaOutputDirectory)` | None bound by the target |

## `Targets/Sdk.Database.Sql.targets`

### `_CohesionCompileSqlDatabaseSchema`

Statically compile each declared database's C# SQL schema into canonical JSON and a SHA-256 sidecar.

| Contract | Exact value |
|---|---|
| `BeforeTargets` | Not declared |
| `AfterTargets` | Not declared |
| `DependsOnTargets` | Not declared |
| `Condition` | `'$(CohesionDatabaseProject)' == 'true'` |
| `Inputs` | `@(Compile);@(ReferencePath);$(_CohesionDatabaseTaskAssembly)` |
| `Outputs` | `$(_CohesionDatabaseSchemaManifest)` |
| `Returns` | Not declared |

| Task or operation | Parameters | `Outputs` |
|---|---|---|
| `CompileDatabaseSchemaTask` | `SourceFiles=@(Compile)`; `ReferencePaths=@(ReferencePath)`; `DefineConstants=$(DefineConstants)`; `LanguageVersion=$(LangVersion)`; `AssemblyName=$(AssemblyName)`; `Model=Sql`; `ProjectDirectory=$(MSBuildProjectDirectory)`; `OutputDirectory=$(CohesionDatabaseSchemaOutputDirectory)` | None bound by the target |


[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.KeyValuePair.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.Sql.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/DESIGN.md`
