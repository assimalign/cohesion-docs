# Props

The Scheduler props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.Scheduler"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.Scheduler.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Scheduler.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.Scheduler.ApplicationModel.SchedulerResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Scheduler` | Resource kind recorded in the manifest. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `http` | Endpoint used for the default control plane. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionReplicas` | `1` | Replica count written to lifecycle metadata. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.Scheduler.props`; when `'$(CohesionMaxReplicas)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=http`; `Scheme=http`; `ContainerPort=8080`; `Public=false` | No additional child metadata | `Targets/Sdk.Scheduler.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=http`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Scheduler.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=http`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Scheduler.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Targets/Sdk.Scheduler.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Scheduler/Targets/Sdk.Scheduler.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
