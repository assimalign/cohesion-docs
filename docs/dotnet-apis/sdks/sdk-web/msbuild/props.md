# Props

The Web props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Imports the base `Sdk.props` without an inline version. The base pin must therefore be present in
`global.json`. Adds `FrameworkReference Include="Assimalign.Cohesion.App.Web"` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.Web.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Web.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.Web.ApplicationModel.WebResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Web` | Resource kind recorded in the manifest. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `http` | Endpoint used for the default control plane. | `Targets/Sdk.Web.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Web.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Web.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `InterceptorsNamespaces` | `$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated` | Adds the namespace used by generated Web API interceptors. | `Targets/Sdk.Web.props`; unconditional |

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=http`; `Scheme=http`; `ContainerPort=8080`; `Public=false` | No additional child metadata | `Targets/Sdk.Web.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=http`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Web.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=http`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Web.props` |

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
