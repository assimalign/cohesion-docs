# Props

The LogSpace props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.LogSpace"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.LogSpace.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.LogSpace.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.LogSpace.ApplicationModel.LogSpaceResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `LogSpace` | Resource kind recorded in the manifest. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `query` | Endpoint used for the default control plane. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.LogSpace.props`; when `'$(CohesionWorkloadKind)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=otlp`; `Scheme=https`; `Certificate=tls`; `ContainerPort=4318`; `Public=false` | No additional child metadata | `Targets/Sdk.LogSpace.props` |
| `CohesionEndpoint` | `Include=query`; `Scheme=https`; `Certificate=tls`; `ContainerPort=8443`; `Public=false` | No additional child metadata | `Targets/Sdk.LogSpace.props` |
| `CohesionMount` | `Include=tls`; `Kind=Secret`; `ContainerPath=/cohesion/mounts/tls` | No additional child metadata | `Targets/Sdk.LogSpace.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.LogSpace.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=query`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.LogSpace.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=query`; `Http=/livez` | No additional child metadata | `Targets/Sdk.LogSpace.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LogSpace/Targets/Sdk.LogSpace.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
