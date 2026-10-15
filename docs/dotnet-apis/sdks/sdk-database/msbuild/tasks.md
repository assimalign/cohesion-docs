# Tasks

Database tasks statically compile SQL schemas and create ordered migration artifacts.

> **Status:** Partial. Only the Sql model supplies the compiled-schema and migration contracts.

These are MSBuild task types, not application runtime APIs. `[Required]` denotes a required task
argument; `[Output]` denotes a value returned to MSBuild. Target bindings supply consumer properties
and items; direct task defaults are shown separately.

## `CompileDatabaseSchemaTask`

Use Roslyn syntax and symbols to analyze exactly one `SqlSchema.Compile` or `SqlSchema.Create`
declaration. Lower it into `SqlCompiledSchema` and serialize canonical semantic JSON plus its
SHA-256
hash. Never execute the consumer entry point.

`Tasks/src/Tasks/DatabaseTask.CompileSchema.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `SourceFiles` | `ITaskItem[]` | Required input | `Array.Empty<ITaskItem>()` | The consumer C# source files to analyze. |
| `ReferencePaths` | `ITaskItem[]` | Optional input | `Array.Empty<ITaskItem>()` | The compiler reference assemblies used to bind the schema DSL. |
| `DefineConstants` | `string?` | Optional input | Type default | The consumer project's preprocessor constants. |
| `LanguageVersion` | `string?` | Optional input | Type default | The consumer project's C# language version. |
| `AssemblyName` | `string` | Required input | `string.Empty` | The consumer assembly's simple name, used in portable CLR type identities. |
| `Model` | `string` | Required input | `string.Empty` | The database model the project targets. Only Sql currently supplies a compiled-schema package. |
| `OutputPath` | `string` | Required input | `string.Empty` | The path the schema model artifact is written to. |
| `HashOutputPath` | `string` | Required input | `string.Empty` | The path the lowercase SHA-256 sidecar is written to. |
| `ProjectDirectory` | `string` | Required input | `string.Empty` | The consumer project directory used to resolve relative item paths. |
| `SchemaHash` | `string` | Output | `string.Empty` | The lowercase SHA-256 of the canonical semantic document. |

### Errors

`COHDBSDK100` –107 describe input, declaration, constant, lambda, operation, duplicate, semantic,
and
expression failures. KeyValuePair produces `COHDBSDK106` without artifacts.

### Tests in source

- **`CompileDatabaseSchemaTaskTests`** —
  `Execute_WithSqlCSharpSchema_ShouldWriteCanonicalDocumentAndHash`;
  `Execute_WithKeyValuePairModel_ShouldFailWithoutArtifacts`;
  `Execute_WithReferencePackPrimitives_ShouldUseRuntimeTypeIdentities`;
  `Execute_WithSupportedSchema_ShouldMatchRuntimeCompilerDocumentAndHash`;
  `Execute_WithUnsupportedExpression_ShouldReportNamedDiagnostic`;
  `Execute_WithDanglingReference_ShouldFailWithoutArtifacts`;
  `Execute_WithMismatchedReferenceType_ShouldFailPrecisely`;
  `Execute_WithoutExactlyOneSchemaDeclaration_ShouldFail`.
- **`DatabaseModelTargetsTests`** —
  `Evaluate_WithTargetFrameworkIntermediatePath_ShouldUseItForSchemaArtifacts`;
  `Evaluate_WithKnownModel_ShouldImportOnlyItsToolSet`;
  `Evaluate_WithUnknownModel_ShouldExposeNamedError`.

## `CreateDatabaseMigrationTask`

Read the compiled desired schema and newest four-digit baseline, plan SQL changes, and atomically
create the next NNNN_name.sql and NNNN_name.schema.json pair. Write deterministic content without
timestamps.

`Tasks/src/Tasks/DatabaseTask.CreateMigration.cs` defines this type.

| Parameter | Type | Direction | Task default | Meaning |
|---|---|---|---|---|
| `SchemaModelPath` | `string` | Required input | `string.Empty` | The desired compiled schema produced by CompileDatabaseSchemaTask. |
| `MigrationsRoot` | `string` | Required input | `string.Empty` | The directory generated migration files are written to. |
| `MigrationName` | `string` | Required input | `string.Empty` | The source-control-safe migration name. |
| `Model` | `string` | Required input | `string.Empty` | The selected database model. |
| `ProjectDirectory` | `string` | Required input | `string.Empty` | The consumer project directory used to resolve relative output paths. |
| `MigrationPath` | `string` | Output | `string.Empty` | The generated SQL script path. |
| `BaselinePath` | `string` | Output | `string.Empty` | The generated compiled-schema baseline path. |

### Errors

`COHDBSDK201` –206 cover unsupported models, invalid names, missing schemas, no changes, existing
output paths, and planner/serialization/filesystem failures. Destructive or unsupported SQL
operations fail without partial outputs.

### Tests in source

- **`CreateDatabaseMigrationTaskTests`** —
  `Execute_WithSqlSchema_ShouldWriteOrdinalScriptAndBaseline`;
  `Execute_WithKeyValuePairModel_ShouldFailExplicitly`;
  `Execute_WithNullableReferencePrimaryKey_ShouldEmitNotNull`;
  `Execute_WithRequiredColumnAddition_ShouldFailWithoutFiles`.

## `DatabaseTask`

Abstract `Microsoft.Build.Utilities.Task` base for the Database tasks; it contributes no parameters
or
additional behavior.

`Tasks/src/Tasks/DatabaseTask.cs` defines this type.

There are no declared task parameters.

### Errors

No additional diagnostics.

### Tests in source

- **`CompileDatabaseSchemaTaskTests`** —
  `Execute_WithSqlCSharpSchema_ShouldWriteCanonicalDocumentAndHash`;
  `Execute_WithKeyValuePairModel_ShouldFailWithoutArtifacts`;
  `Execute_WithReferencePackPrimitives_ShouldUseRuntimeTypeIdentities`;
  `Execute_WithSupportedSchema_ShouldMatchRuntimeCompilerDocumentAndHash`;
  `Execute_WithUnsupportedExpression_ShouldReportNamedDiagnostic`;
  `Execute_WithDanglingReference_ShouldFailWithoutArtifacts`;
  `Execute_WithMismatchedReferenceType_ShouldFailPrecisely`;
  `Execute_WithoutExactlyOneSchemaDeclaration_ShouldFail`.
- **`CreateDatabaseMigrationTaskTests`** —
  `Execute_WithSqlSchema_ShouldWriteOrdinalScriptAndBaseline`;
  `Execute_WithKeyValuePairModel_ShouldFailExplicitly`;
  `Execute_WithNullableReferencePrimaryKey_ShouldEmitNotNull`;
  `Execute_WithRequiredColumnAddition_ShouldFailWithoutFiles`.


The compiler helpers `CSharpSchemaExtractor`, `CSharpExpressionCanonicalizer`,
`CSharpTypeIdentity`, and `CompiledSchemaSourceWriter` bind and normalize source without loading a
consumer assembly. They are not separately registered tasks.
[Diagnostic meanings](../overview.md#diagnostics) cover the helper-produced codes.

[MSBuild](index.md) · [Targets](targets.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CompileSchema.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/CompileDatabaseSchemaTaskTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/DatabaseModelTargetsTests.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CreateMigration.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/tests/CreateDatabaseMigrationTaskTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/CompiledSchemaSourceWriter.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/CSharpExpressionCanonicalizer.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/CSharpSchemaExtractor.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/CSharpTypeIdentity.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/SchemaSourceModel.cs`
