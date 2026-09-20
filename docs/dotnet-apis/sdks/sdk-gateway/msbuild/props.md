# Props

Gateway props suppress the implicit framework and declare its resource, provider, and dependency metadata.

## `Sdk/Sdk.props`

Sets `CohesionAutoIncludeAppFramework=false` before the base import, then imports
`Targets/Sdk.Gateway.props`. The ordering prevents an implicit base-framework item from being
created during base props evaluation.

## `Targets/Sdk.Gateway.props`

Registers `CohesionCreateResourceVerbs` using `../Tasks/Assimalign.Cohesion.Sdk.Gateway.Tasks.dll`
and `Runtime=NET`. The exact property assignments are:

| Property | Value | Effect | Condition and source |
|---|---|---|---|
| `CohesionAutoIncludeAppFramework` | `false` | Suppress implicit framework references only when set to false before inclusion. | `Sdk/Sdk.props`; unconditional |
| `IsAotCompatible` | `true` | Assigned during `evaluation`. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionApplicationModel` | `enabled` | Enable or disable resource generation. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceKind` | `Composite` | Resource kind recorded in the manifest. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionResourceComposable` | `true` | Whether the manifest permits in-process composition. | `Targets/Sdk.Gateway.props`; unconditional |
| `CohesionControlPlaneEndpoint` | `admin` | Endpoint used for the default control plane. | `Targets/Sdk.Gateway.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.Gateway.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionGatewayAdminPort` | `8082` | Container port for the admin endpoint; refreshed after the project body. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayAdminPort)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.Gateway.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionGateways` | `Local` | Semicolon-delimited provider selection. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGateways)' == ''` |
| `CohesionPlatformsVersion` | `$(CohesionVersion)` | Version of selected external Docker/Kubernetes provider packages. | `Targets/Sdk.Gateway.props`; when `'$(CohesionPlatformsVersion)' == ''` |
| `CohesionGatewayAot` | `auto` | auto, true, or false; controls whole-gateway `PublishAot` selection. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayAot)' == ''` |
| `CohesionGatewayInProcess` | `false` | Explicit opt-in required alongside the InProcess provider. | `Targets/Sdk.Gateway.props`; when `'$(CohesionGatewayInProcess)' == ''` |
| `CohesionSkipResourceApplicationModelReference` | `true` | Suppress base automatic area dependency injection. | `Targets/Sdk.Gateway.props`; unconditional |

The file declares the admin endpoint, probes, seven typed resource mappings, five client-kind
mappings, four provider-package mappings, and fixed orchestration references. These are the actual
item declarations and metadata:

| Item | Declaration | Metadata | Condition and source |
|---|---|---|---|
| `CohesionEndpoint` | `Include=admin`; `Scheme=http`; `ContainerPort=$(CohesionGatewayAdminPort)`; `Public=false` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=admin`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=admin`; `Http=/livez` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=Web` | `ApplicationModel=Assimalign.Cohesion.Web.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.Web.ApplicationModel.WebResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.Web.ApplicationModel.IWebResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.Web.ApplicationModel.WebResourceExtensions.AddWeb` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=Database` | `ApplicationModel=Assimalign.Cohesion.Database.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.Database.ApplicationModel.DatabaseResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.Database.ApplicationModel.IDatabaseResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.Database.ApplicationModel.DatabaseResourceExtensions.AddDatabase` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=ConfigurationStore` | `ApplicationModel=Assimalign.Cohesion.ConfigurationStore.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.ConfigurationStore.ApplicationModel.ConfigurationStoreResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.ConfigurationStore.ApplicationModel.IConfigurationStoreResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.ConfigurationStore.ApplicationModel.ConfigurationStoreResourceExtensions.AddConfigurationStore` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=SecretStore` | `ApplicationModel=Assimalign.Cohesion.SecretStore.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.SecretStore.ApplicationModel.SecretStoreResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.SecretStore.ApplicationModel.ISecretStoreResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.SecretStore.ApplicationModel.SecretStoreResourceExtensions.AddSecretStore` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=IdentityHub` | `ApplicationModel=Assimalign.Cohesion.IdentityHub.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.IdentityHub.ApplicationModel.IdentityHubResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.IdentityHub.ApplicationModel.IIdentityHubResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.IdentityHub.ApplicationModel.IdentityHubResourceExtensions.AddIdentityHub` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=Rezolvr` | `ApplicationModel=Assimalign.Cohesion.Rezolvr.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.Rezolvr.ApplicationModel.RezolvrResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.Rezolvr.ApplicationModel.IRezolvrResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.Rezolvr.ApplicationModel.RezolvrResourceExtensions.AddRezolvr` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayResourceKind` | `Include=LogSpace` | `ApplicationModel=Assimalign.Cohesion.LogSpace.ApplicationModel`; `OptionsType=global::Assimalign.Cohesion.LogSpace.ApplicationModel.LogSpaceResourceOptions`; `DescriptorType=global::Assimalign.Cohesion.LogSpace.ApplicationModel.ILogSpaceResourceDescriptor`; `AddMethod=global::Assimalign.Cohesion.LogSpace.ApplicationModel.LogSpaceResourceExtensions.AddLogSpace` | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | `Include=ConfigurationStore`; `PackageId=Assimalign.Cohesion.ConfigurationStore.Client` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | `Include=Database`; `PackageId=Assimalign.Cohesion.Database.Client` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | `Include=IdentityHub`; `PackageId=Assimalign.Cohesion.IdentityHub.Client` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | `Include=Rezolvr`; `PackageId=Assimalign.Cohesion.Rezolvr.Client` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayClientKind` | `Include=SecretStore`; `PackageId=Assimalign.Cohesion.SecretStore.Client` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayPackage` | `Include=Local`; `PackageId=Assimalign.Cohesion.ApplicationModel.Gateway`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayPackage` | `Include=InProcess`; `PackageId=Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayPackage` | `Include=Docker`; `PackageId=Assimalign.Cohesion.ApplicationModel.Gateway.Docker`; `Version=$(CohesionPlatformsVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionGatewayPackage` | `Include=Kubernetes`; `PackageId=Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes`; `Version=$(CohesionPlatformsVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props` |
| `CohesionProjectReference` | `Include=Assimalign.Cohesion.ApplicationModel`; `ProjectPath=@(_CohesionGatewayApplicationModelProject)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayApplicationModelProject)' != ''` |
| `CohesionProjectReference` | `Include=Assimalign.Cohesion.ApplicationModel.Gateway`; `ProjectPath=@(_CohesionGatewayRuntimeProject)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayRuntimeProject)' != ''` |
| `CohesionProjectReference` | `Include=Assimalign.Cohesion.Hosting.Resources`; `ProjectPath=@(_CohesionGatewayResourceHostingProject)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayResourceHostingProject)' != ''` |
| `CohesionProjectReference` | `Include=Assimalign.Cohesion.Connections`; `ProjectPath=@(_CohesionGatewayConnectionsProject)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayConnectionsProject)' != ''` |
| `CohesionPackageReference` | `Include=Assimalign.Cohesion.ApplicationModel`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayApplicationModelProject)' == ''` |
| `CohesionPackageReference` | `Include=Assimalign.Cohesion.ApplicationModel.Gateway`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayRuntimeProject)' == ''` |
| `CohesionPackageReference` | `Include=Assimalign.Cohesion.Hosting.Resources`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayResourceHostingProject)' == ''` |
| `CohesionPackageReference` | `Include=Assimalign.Cohesion.Connections`; `Version=$(CohesionVersion)` | No additional child metadata | `Targets/Sdk.Gateway.props`; when `'@(_CohesionGatewayConnectionsProject)' == ''` |

## Provider contribution contract

| Metadata | Meaning |
|---|---|
| `Name` | Stable selection name. |
| `GatewayType` | Concrete gateway implementation generated code constructs. |
| `OptionsType` | Provider-specific configuration type. |
| `RequiresJit` | Boolean controlling whole-executable automatic AOT eligibility. |
| `CommandLineApplyMethod` | Optional public static Apply(OptionsType, string[]) entry point. |

Provider packages contribute `CohesionGatewayProvider` through `buildTransitive` props. The SDK
refreshes admin port and provider package versions after consumer properties are known.
`CohesionGatewayResourceKind.DescriptorType` is optional; custom mappings without it use the generic
descriptor interface.

[MSBuild](index.md) · [Base props](../../sdk/msbuild/props.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
