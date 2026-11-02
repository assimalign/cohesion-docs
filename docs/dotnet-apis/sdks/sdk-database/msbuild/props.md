# Props

Database props register schema tasks and retain metadata alongside compiler defaults.

## `Sdk/Sdk.props`

Marks a resource SDK before importing the base SDK with no inline version, conditionally adds
`Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Database` when
`CohesionAutoIncludeAppFramework` is not `false`, then imports `Targets/Sdk.Database.props`.
Pin the base and Database SDKs together.

## `Targets/Sdk.Database.props`

Property defaults apply when empty. Endpoint, probe, and mount items use ordinary `Include` before
the consumer project. Database props retain `StatefulSet`, replica, endpoint, and mount metadata,
but no current target turns it into a resource manifest. It does not select the platform, bind
Database listeners, or register a managed instance. Use explicit application composition and
[AddResource](../../../resources/database/assimalign-cohesion-database-hosting-applicationmodel/index.md).

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `CohesionResourceKind` | `Database` | Retained resource-kind metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `admin` | Retained admin endpoint metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Retained admin route metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Retained workload metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionReplicas` | `1` | Retained replica-count metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionMaxReplicas` | `1` | Retained maximum-replica metadata default. | `Targets/Sdk.Database.props`; when `'$(CohesionMaxReplicas)' == ''` |
| `_CohesionDatabaseTaskAssembly` | `$(MSBuildThisFileDirectory)..\Tasks\Assimalign.Cohesion.Sdk.Database.Tasks.dll` | Assigned during `evaluation`. | `Targets/Sdk.Database.props`; unconditional |
| `CohesionDatabaseProject` | `false` | Opt into build-time C# schema compilation. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseProject)' == ''` |
| `CohesionDatabaseModel` | `Sql` | Exact, case-sensitive model selector: Sql or KeyValuePair. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseModel)' == ''` |
| `CohesionDatabaseMigrationsRoot` | `Migrations` | Project-relative destination for migration scripts and baselines. | `Targets/Sdk.Database.props`; when `'$(CohesionDatabaseMigrationsRoot)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
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
