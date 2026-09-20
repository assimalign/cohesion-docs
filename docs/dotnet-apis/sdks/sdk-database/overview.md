# Overview

The Database SDK separates resource orchestration defaults from optional schema and migration generation.

> **Status:** Partial. KeyValuePair is a recognized model selector with unsupported compilation and migration artifacts.

## Shared surface

The [base overview](../sdk/overview.md) is the inherited property, item, artifact, and diagnostic
contract.
The area layer does not replace settings generation, pin validation, resource packaging, or image
targets.

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; explicitly choose Library for a base-SDK library. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | Compile against the .NET 10 target framework. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | Enable preview C# syntax. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | Permit preview APIs; also affects language-version selection. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | Require explicit using directives. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | Enable nullable reference analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | Enable ahead-of-time compatibility analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `CohesionApplicationModel` | `disabled` | Opt in to manifest generation with enabled. | `base Targets/Sdk.Resource.props` |
| `CohesionAutoIncludeAppFramework` | `Unset; inclusion enabled` | Set false before the props include to suppress both frameworks. | `base Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionAppFrameworkVersion` | `$(CohesionVersion)` | Version used by the base and area framework packs. | `base Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionSkipSdkPinCheck` | `Unset` | Tooling-only escape for pin validation. | `base Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` |

## Database properties

| Name | Default or assignment | Effect | Where defined and condition |
|---|---|---|---|
| `CohesionDatabaseSchemaOutputPath` | `$(IntermediateOutputPath)cohesion/database.schema.json` | Canonical SQL schema JSON output. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; when `'$(CohesionDatabaseSchemaOutputPath)' == ''` |
| `CohesionDatabaseSchemaHashOutputPath` | `$(IntermediateOutputPath)cohesion/database.schema.sha256` | Lowercase SHA-256 sidecar output. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; when `'$(CohesionDatabaseSchemaHashOutputPath)' == ''` |
| `CohesionDatabaseSchemaHash` | `@(_CohesionDatabaseSchemaHashLine)` | Resolved hash exposed to later targets. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; when `'$(CohesionDatabaseProject)' == 'true'` |
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Database.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.Database.ApplicationModel.DatabaseResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Database` | Resource kind recorded in the manifest. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `admin` | Endpoint used for the default control plane. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Database.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionReplicas` | `1` | Replica count written to lifecycle metadata. | `Targets/Sdk.Database.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Database.props`; when `'$(CohesionMaxReplicas)' == ''` |
| `CohesionDatabaseProject` | `false` | Opt into build-time C# schema compilation. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseProject)' == ''` |
| `CohesionDatabaseModel` | `Sql` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseModel)' == ''` |
| `CohesionDatabaseMigrationsRoot` | `Migrations` | Project-relative destination for migration scripts and baselines. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseMigrationsRoot)' == ''` |

| Additional input | Default | Effect | Read by |
|---|---|---|---|
| `CohesionDatabaseMigrationName` | Unset | Required sanitized name for explicit migration generation. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |

## Database items

| Item | Declaration | Metadata | Where defined |
|---|---|---|---|
| `FrameworkReference` | `Include=Assimalign.Cohesion.App.Database` | No additional child metadata | `Sdk/Sdk.props`; when `'$(CohesionAutoIncludeAppFramework)' != 'false'` |
| `CohesionCommand` | `Include=database.add-database;database.add-principal` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionEndpoint` | `Include=db`; `Scheme=cohesion-db`; `ContainerPort=5740`; `Public=false` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionEndpoint` | `Include=admin`; `Scheme=http`; `ContainerPort=8081`; `Public=false` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=admin`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=admin`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.Database.props` |

Defaults use `Include` before the project body, so `Update` and `Remove` work by identity. Compiler
inputs are `Compile` and `ReferencePath`; `DefineConstants`, `LangVersion`, `AssemblyName`, and
`MSBuildProjectDirectory` are passed to the schema task. `Outputs` are tracked with `FileWrites`.

## Generated artifacts

| Opt-in or operation | Output | Meaning |
|---|---|---|
| `CohesionDatabaseProject=true` | `$(IntermediateOutputPath)cohesion/database.schema.json` | Canonical SQL compiled schema. |
| `CohesionDatabaseProject=true` | `$(IntermediateOutputPath)cohesion/database.schema.sha256` | Lowercase SHA-256 over exact canonical UTF-8 bytes. |
| `CohesionDatabaseCreateMigration` | `Migrations/NNNN_name.sql` and `NNNN_name.schema.json` | Ordered migration and matching canonical baseline. |
| `CohesionApplicationModel=enabled` | Inherited resource outputs | Manifest, accessors, and Database control-plane registration. |

## Model and migration limits

`Sql` and `KeyValuePair` are the only allowlisted, case-sensitive imports. The latter has a target
but no model-owned compiled-schema contract, so compilation fails with `COHDBSDK106`. A direct
migration task rejects it with `COHDBSDK201`; the normal migration target reaches compilation
first.

SQL analysis uses C# syntax and symbols, not `Program.Main` or `Schema/**/*.sql`. Names and numeric
settings must be constants; schema callbacks must be inline and statically analyzable. The current
SQL migration renderer rejects constraints, custom types, ALTER metadata, and required added columns
without a default or backfill.

```sh
dotnet msbuild -t:CohesionDatabaseCreateMigration -p:CohesionDatabaseMigrationName=add-orders
```

## Diagnostics

All Database-specific codes below are errors. [Base diagnostics](../sdk/overview.md#diagnostics),
including `COHSDK002` pin validation, also apply.

| Code | Meaning |
|---|---|
| `COHDBSDK001` | Unknown or case-mismatched model selector. |
| `COHDBSDK002` | Loaded model tool set did not register a compile target. |
| `COHDBSDK100` | Source/reference I/O, invalid language version, or artifact I/O failure. |
| `COHDBSDK101` | Schema declaration count is not exactly one. |
| `COHDBSDK102` | Schema, table, function, or principal name is not a non-empty compile-time string. |
| `COHDBSDK103` | Required configuration/body is not an inline lambda. |
| `COHDBSDK104` | Unsupported schema/table/principal operation or nonconstant configuration. |
| `COHDBSDK105` | Duplicate declarations or multiple primary keys. |
| `COHDBSDK106` | Model lacks a compiled-schema package, or schema semantics are invalid. |
| `COHDBSDK107` | Expression cannot be compiled as deterministic schema syntax. |
| `COHDBSDK201` | Model has no supported SQL migration tooling. |
| `COHDBSDK202` | Missing or invalid migration name. |
| `COHDBSDK203` | Compiled desired schema does not exist. |
| `COHDBSDK204` | Desired schema has no changes from the newest baseline. |
| `COHDBSDK205` | Migration output ordinal/name already exists. |
| `COHDBSDK206` | Migration planning, rendering, serialization, ordinal limit, or filesystem failure. |

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.KeyValuePair.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.Sql.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/Compilation/CSharpSchemaExtractor.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CompileSchema.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Tasks/src/Tasks/DatabaseTask.CreateMigration.cs`
