# Props

The Rezolvr props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.Rezolvr"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.Rezolvr.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Rezolvr.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.Rezolvr.ApplicationModel.RezolvrResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Rezolvr` | Resource kind recorded in the manifest. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `admin` | Endpoint used for the default control plane. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Rezolvr.props`; when `'$(CohesionWorkloadKind)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionCommand` | `Include=rezolvr.add-a-record;rezolvr.add-cname-record` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |
| `CohesionEndpoint` | `Include=dns`; `Scheme=dns`; `Protocol=udp`; `ContainerPort=53`; `Public=false` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |
| `CohesionEndpoint` | `Include=dns-tcp`; `Scheme=dns`; `Protocol=tcp`; `ContainerPort=53`; `Public=false` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |
| `CohesionEndpoint` | `Include=admin`; `Scheme=http`; `ContainerPort=8081`; `Public=false` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=admin`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=admin`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Rezolvr.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
