# Props

Database props register schema tasks and set schema-compilation defaults.

## `Sdk/Sdk.props`

Marks a resource SDK before importing the base SDK with no inline version, conditionally adds
`Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Database` when
`CohesionAutoIncludeAppFramework` is not `false`, then imports `Targets/Sdk.Database.props`.
Pin the base and Database SDKs together.

## `Targets/Sdk.Database.props`

Property defaults apply when empty. Database props set schema-compilation and migration options;
the SDK supplies no orchestration defaults. Configure wire listeners in application code and use
[AddResource](../../../resources/database/assimalign-cohesion-database-hosting-applicationmodel/index.md)
to register a managed instance.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `_CohesionDatabaseTaskAssembly` | `$(MSBuildThisFileDirectory)..\Tasks\Assimalign.Cohesion.Sdk.Database.Tasks.dll` | Assigned during `evaluation`. | `Targets/Sdk.Database.props`; unconditional |
| `CohesionDatabaseProject` | `false` | Opt into build-time C# schema compilation. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseProject)' == ''` |
| `CohesionDatabaseModel` | `Sql` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseModel)' == ''` |
| `CohesionDatabaseMigrationsRoot` | `Migrations` | Project-relative destination for migration scripts and baselines. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseMigrationsRoot)' == ''` |

The `_CohesionDatabaseTaskAssembly` path points to
`../Tasks/Assimalign.Cohesion.Sdk.Database.Tasks.dll` in the package. `CompileDatabaseSchemaTask`
and `CreateDatabaseMigrationTask` use that assembly with `Runtime=NET`. The same path is an
incremental compiler input. Schema output paths are assigned later in the migration targets after
the Microsoft SDK finalizes `IntermediateOutputPath`.

[MSBuild](index.md) · [Base props](../../sdk/msbuild/props.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.KeyValuePair.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.Sql.targets`
- **Source** — `cohesion/sdks/Directory.Build.targets`
