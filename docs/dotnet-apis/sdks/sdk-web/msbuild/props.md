# Props

The Web props files add its framework and declare overridable orchestration metadata.

## `Sdk/Sdk.props`

Sets `_CohesionResourceSdk=true` before importing the base `Sdk.props`, so the shared defaults
select executable output. The import has no inline version; the base pin must therefore be present
in `global.json`. Adds both `Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Web` framework
references unless `CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.Web.props`

The defaults below come from this area’s own props file. `CohesionApplicationModel` is not enabled
here.

| Property | Assigned value | Effect | Source and condition |
|---|---|---|---|
| `EventSourceSupport` | `true` | Preserve NativeAOT EventSource delivery and forwarding to configured sinks. | `Targets/Sdk.Web.props`; when `'$(EventSourceSupport)' == ''` |
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.Web.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.ApplicationModel.WebResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `Web` | Resource kind recorded in the manifest. | `Targets/Sdk.Web.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `http` | Endpoint used for the default control plane. | `Targets/Sdk.Web.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Web.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Web.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `InterceptorsNamespaces` | `$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated` | Adds the namespace used by generated Web API interceptors. | `Targets/Sdk.Web.props`; unconditional |

`EventSourceSupport` defaults to `true` only when unset. Explicit values before the import are
preserved; later project-body assignments override the default, and command-line global properties
win over both. The default is Web-specific. Setting it to `false` disables EventSource delivery,
in-process listeners, and forwarding in a native application, while server behavior and internal
counter maintenance remain unchanged. See the [NativeAOT design](../assimalign-cohesion-sdk-web-tasks/design.md#nativeaot-event-delivery).

| Item | Declaration | Metadata | Source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=http`; `Scheme=http`; `ContainerPort=8080`; `Public=false` | No additional child metadata | `Targets/Sdk.Web.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=http`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Web.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=http`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Web.props` |
| `None` | `Update=wwwroot/**`; `CopyToOutputDirectory=PreserveNewest`; `CopyToPublishDirectory=PreserveNewest` | No additional child metadata | `Targets/Sdk.Web.props` |

`wwwroot` is the web root `UseStaticFiles()` serves, resolved under the content root
(`AppContext.BaseDirectory` for a plain application), so the `None` update ships it with the build
and publish output. A consumer’s own `None Update` in the project body still wins.

For item metadata defaults, including `Protocol=tcp`, see [base props](../../sdk/msbuild/props.md)
. Setting a property later does not retroactively change a framework item already added during props
evaluation; configure inclusion and pack version before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Targets/Sdk.Resource.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
