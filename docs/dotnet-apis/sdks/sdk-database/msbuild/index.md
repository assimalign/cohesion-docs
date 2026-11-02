# MSBuild

The Database SDK imports shared build behavior before selecting one allowlisted model tool set.

## Evaluation order

1. **Database props** mark a resource SDK and import the base SDK props.
2. **Base props** load defaults, Microsoft SDK props, the packed version snapshot, common props,
   strongly typed settings, and framework-reference registration.
3. **Database framework references** add `App` and `App.Database`, unless suppressed, then import
   `Targets/Sdk.Database.props` for task registrations and defaults.
4. **The consumer project** sets schema options and declares source and references.
5. **Base targets** supply self-contained resource-executable defaults and disable transitive
   framework-reference downloads before importing Microsoft SDK targets. They then import pin
   validation, strongly typed settings, and name-only project-reference handling.
6. **Database migration targets** establish the per-database schema output directory and statically
   import the exact `Sql` or `KeyValuePair` model tool set.

There is no resource-manifest target or generated control-plane registration in this chain.

- **Props** — [Defaults and task registrations](props.md).
- **Targets** — [Model selection, compilation, and migration scheduling](targets.md).
- **Tasks** — [Parameters, validation, and tests](tasks.md).

## Property and item inventory

This index includes read-only Microsoft Build Engine (MSBuild) inputs and private state as well as
consumer options. Names beginning with an underscore are implementation details. The
[overview](../overview.md) describes supported consumer settings; the tables below summarize
compiler inputs and the retained metadata defaults. Metadata does not configure runtime management.

### Properties

| Name | Role | Files |
|---|---|---|
| `_CohesionDatabaseModelCompileTarget` | Private state | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `_CohesionDatabaseModelTargetsLoaded` | Private state | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `_CohesionDatabaseTaskAssembly` | Private state | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.props`; `Targets/Sdk.Database.Sql.targets` |
| `AssemblyName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionAutoIncludeAppFramework` | Suppress implicit framework references only when set to false before inclusion. | `Sdk/Sdk.props` |
| `CohesionControlPlaneEndpoint` | Retained admin endpoint metadata default. | `Targets/Sdk.Database.props` |
| `CohesionControlPlanePath` | Retained admin route metadata default. | `Targets/Sdk.Database.props` |
| `CohesionDatabaseMigrationName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionDatabaseMigrationsRoot` | Project-relative destination for migration scripts and baselines. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |
| `CohesionDatabaseModel` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |
| `CohesionDatabaseProject` | Opt into build-time C# schema compilation. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.props`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionDatabaseSchemaOutputDirectory` | Directory for per-database schema artifacts. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionDatabaseName` | Database selected for migration generation. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionMaxReplicas` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Database.props` |
| `CohesionReplicas` | Replica count written to lifecycle metadata. | `Targets/Sdk.Database.props` |
| `CohesionResourceKind` | Retained resource-kind metadata default. | `Targets/Sdk.Database.props` |
| `CohesionWorkloadKind` | Retained workload metadata default. | `Targets/Sdk.Database.props` |
| `DefineConstants` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `IntermediateOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `LangVersion` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `MSBuildProjectDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `MSBuildThisFileDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |

### Items

| Name | Role | Files |
|---|---|---|
| `CohesionEndpoint` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.props` |
| `CohesionMount` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.props` |
| `CohesionProbe` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.props` |
| `Compile` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `FileWrites` | Declared, consumed, or produced by the listed files. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `FrameworkReference` | Declared, consumed, or produced by the listed files. | `Sdk/Sdk.props` |
| `ReferencePath` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.KeyValuePair.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Database/Targets/Sdk.Database.Sql.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
