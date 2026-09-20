# MSBuild

The Database SDK imports shared build behavior before selecting one allowlisted model tool set.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.props` enters.
2. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` enters the base SDK.
3. `Targets/Assimalign.Cohesion.Sdk.Defaults.props` supplies empty-value defaults before Microsoft
   defaults run.
4. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
5. `Targets/Build.Version.props` loads the static version snapshot generated when the SDK was packed.
6. `Targets/Assimalign.Cohesion.Sdk.Common.props` sets the SDK marker and intermediate path.
7. `Targets/Sdk.Resource.props` registers manifest tasks and resource metadata defaults.
8. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props` registers the settings task.
9. `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` registers all frameworks and
   conditionally adds `Assimalign.Cohesion.App`.
10. Database props add `Assimalign.Cohesion.App.Database` and import `Targets/Sdk.Database.props`.
11. The consumer project body sets resource and schema options.
12. `Assimalign.Cohesion.Sdk.Database/Sdk/Sdk.targets` imports the base targets.
13. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` captures image publish inputs and supplies
    self-contained host-runtime defaults for enabled Debug resources.
14. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
15. `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` registers pin validation.
16. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` wires settings generation and cleanup.
17. `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets` imports `Sdk.Resource.targets`
    , preserves the container extension chain, then imports `Sdk.Image.targets`.
18. `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` converts Cohesion reference items.
19. `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` establishes
    target-framework-specific artifact paths and conditionally imports `Sdk.Database.Sql.targets` or
    `Sdk.Database.KeyValuePair.targets`.

- **Props** — [Defaults and task registrations](props.md).
- **Targets** — [Model selection, compilation, and migration scheduling](targets.md).
- **Tasks** — [Parameters, validation, and tests](tasks.md).

## Property and item inventory

This index includes read-only Microsoft Build Engine (MSBuild) inputs and private state as well as
consumer options. Names beginning with an underscore are implementation details. The
[overview](../overview.md) describes supported consumer settings; this inventory records every
explicit property and item name referenced or assigned by the shipped files.

### Properties

| Name | Role | Files |
|---|---|---|
| `_CohesionDatabaseModelCompileTarget` | Private state | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `_CohesionDatabaseModelTargetsLoaded` | Private state | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `_CohesionDatabaseTaskAssembly` | Private state | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.props`; `Targets/Sdk.Database.Sql.targets` |
| `AssemblyName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionAutoIncludeAppFramework` | Suppress implicit framework references only when set to false before inclusion. | `Sdk/Sdk.props` |
| `CohesionControlPlaneEndpoint` | Endpoint used for the default control plane. | `Targets/Sdk.Database.props` |
| `CohesionControlPlanePath` | Absolute control-plane route prefix. | `Targets/Sdk.Database.props` |
| `CohesionDatabaseMigrationName` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionDatabaseMigrationsRoot` | Project-relative destination for migration scripts and baselines. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |
| `CohesionDatabaseModel` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |
| `CohesionDatabaseProject` | Opt into build-time C# schema compilation. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.props`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionDatabaseSchemaHash` | Resolved hash exposed to later targets. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionDatabaseSchemaHashOutputPath` | Lowercase SHA-256 sidecar output. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionDatabaseSchemaOutputPath` | Canonical SQL schema JSON output. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `CohesionMaxReplicas` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Database.props` |
| `CohesionReplicas` | Replica count written to lifecycle metadata. | `Targets/Sdk.Database.props` |
| `CohesionResourceApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Database.props` |
| `CohesionResourceControlPlaneType` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Database.props` |
| `CohesionResourceKind` | Resource kind recorded in the manifest. | `Targets/Sdk.Database.props` |
| `CohesionWorkloadKind` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Database.props` |
| `DefineConstants` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `IntermediateOutputPath` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `LangVersion` | MSBuild input, output, or assignment; see the file contract. | `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `MSBuildProjectDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.KeyValuePair.targets`; `Targets/Sdk.Database.Sql.targets` |
| `MSBuildThisFileDirectory` | MSBuild input, output, or assignment; see the file contract. | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets`; `Targets/Sdk.Database.props` |

### Items

| Name | Role | Files |
|---|---|---|
| `_CohesionDatabaseSchemaHashLine` | Private intermediate item | `Targets/Assimalign.Cohesion.Sdk.Database.Migration.targets` |
| `CohesionCommand` | Declared, consumed, or produced by the listed files. | `Targets/Sdk.Database.props` |
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
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
