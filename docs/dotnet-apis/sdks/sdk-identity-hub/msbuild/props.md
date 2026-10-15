# Props

The IdentityHub props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.IdentityHub"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.IdentityHub.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.IdentityHub.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.IdentityHub.ApplicationModel.IdentityHubResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `IdentityHub` | Resource kind recorded in the manifest. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `https` | Endpoint used for the default control plane. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionReplicas` | `1` | Replica count written to lifecycle metadata. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionReplicas)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.IdentityHub.props`; when `'$(CohesionMaxReplicas)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionCommand` | `Include=identityhub.add-audience;identityhub.add-client` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |
| `CohesionEndpoint` | `Include=https`; `Scheme=https`; `Certificate=tls`; `ContainerPort=8443`; `Public=false` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=https`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=https`; `Http=/livez` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |
| `CohesionMount` | `Include=tls`; `Kind=Secret`; `ContainerPath=/cohesion/mounts/tls` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.IdentityHub.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IdentityHub/Targets/Sdk.IdentityHub.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
