# Props

The SecretStore props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.SecretStore"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.SecretStore.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.SecretStore.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.SecretStore.ApplicationModel.SecretStoreResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `SecretStore` | Resource kind recorded in the manifest. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `api` | Endpoint used for the default control plane. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionMaxReplicas)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionCommand` | `Include=secretstore.add-secret;secretstore.issue-certificate` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionEndpoint` | `Include=api`; `Scheme=https`; `Certificate=tls`; `ContainerPort=8443`; `Public=false` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionMount` | `Include=tls`; `Kind=Secret`; `ContainerPath=/cohesion/mounts/tls` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.SecretStore.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
