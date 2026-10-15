# Props

The EmailHub props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.EmailHub"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.EmailHub.props`

After substituting area names, this props file is byte-identical to the `ApiManager`, `EventHub`,
`IoTHub`, `MediaHub`, `MessageHub`, `NotificationHub` props files. `CohesionApplicationModel` is
not enabled here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.EmailHub.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.EmailHub.ApplicationModel.EmailHubResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `EmailHub` | Resource kind recorded in the manifest. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `http` | Endpoint used for the default control plane. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.EmailHub.props`; when `'$(CohesionWorkloadKind)' == ''` |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=http`; `Scheme=http`; `ContainerPort=8080`; `Public=false` | No additional child metadata | `Targets/Sdk.EmailHub.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=http`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.EmailHub.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=http`; `Http=/livez` | No additional child metadata | `Targets/Sdk.EmailHub.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Targets/Sdk.EmailHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EmailHub/Targets/Sdk.EmailHub.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
