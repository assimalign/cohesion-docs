# Props

Database props register the schema tasks and supply declarative resource and compiler defaults.

## `Sdk/Sdk.props`

Imports the base SDK with no inline version, conditionally adds `Assimalign.Cohesion.App.Database`
when `CohesionAutoIncludeAppFramework` is not `false`, then imports `Targets/Sdk.Database.props`.
Pin the base and Database SDKs together.

## `Targets/Sdk.Database.props`

All public Database defaults are conditional on empty values. Endpoints, probes, mounts, and command
kinds use ordinary `Include` items before the consumer project. The Database resource defaults to a
single-replica `StatefulSet`, with its control plane on `admin` and persistent data at `/data`.

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Database.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.Database.ApplicationModel.DatabaseResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Database` | Resource kind recorded in the manifest. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `admin` | Endpoint used for the default control plane. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Database.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionReplicas` | `1` | Replica count written to lifecycle metadata. | `Targets/Sdk.Database.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Database.props`; when `'$(CohesionMaxReplicas)' == ''` |
| `_CohesionDatabaseTaskAssembly` | `$(MSBuildThisFileDirectory)..\Tasks\Assimalign.Cohesion.Sdk.Database.Tasks.dll` | Assigned during `evaluation`. | `Targets/Sdk.Database.props`; unconditional |
| `CohesionDatabaseProject` | `false` | Opt into build-time C# schema compilation. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseProject)' == ''` |
| `CohesionDatabaseModel` | `Sql` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseModel)' == ''` |
| `CohesionDatabaseMigrationsRoot` | `Migrations` | Project-relative destination for migration scripts and baselines. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseMigrationsRoot)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionCommand` | `Include=database.add-database;database.add-principal` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionEndpoint` | `Include=db`; `Scheme=cohesion-db`; `ContainerPort=5740`; `Public=false` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionEndpoint` | `Include=admin`; `Scheme=http`; `ContainerPort=8081`; `Public=false` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=admin`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=admin`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Database.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.Database.props` |

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
