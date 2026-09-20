# Overview

The SecretStore SDK inherits shared build behavior and adds area-specific defaults.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

## Chaining and overrides

The [base overview](../sdk/overview.md) is the inherited property, item, artifact, and diagnostic
contract.
The area layer does not replace settings generation, pin validation, resource packaging, or image
targets.

Properties conditioned on an empty value apply before the consumer project body. Later project
assignments can override them. Default items use `Include` so consumers can use `Update` or `Remove`
by identity.

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; explicitly choose Library for a base-SDK library. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | Compile against the .NET 10 target framework. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | Enable preview C# syntax. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | Permit preview APIs; also affects language-version selection. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | Require explicit using directives. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | Enable nullable reference analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | Enable ahead-of-time compatibility analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `CohesionApplicationModel` | `disabled` | Opt in to manifest generation with enabled. | `base Targets/Sdk.Resource.props` |
| `CohesionAutoIncludeAppFramework` | `Unset; inclusion enabled` | Set false before the props include to suppress both frameworks. | `base Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionAppFrameworkVersion` | `$(CohesionVersion)` | Version used by the base and area framework packs. | `base Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` |
| `CohesionSkipSdkPinCheck` | `Unset` | Tooling-only escape for pin validation. | `base Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` |

## Area properties

| Name | Default or assignment | Effect | Where defined and condition |
|---|---|---|---|
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.SecretStore.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.SecretStore.ApplicationModel.SecretStoreResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `SecretStore` | Resource kind recorded in the manifest. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `api` | Endpoint used for the default control plane. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `StatefulSet` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionWorkloadKind)' == ''` |
| `CohesionMaxReplicas` | `1` | Upper bound on replicas; must not be below the replica count. | `Targets/Sdk.SecretStore.props`; when `'$(CohesionMaxReplicas)' == ''` |

## Area items

| Item | Declaration | Metadata | Where defined |
|---|---|---|---|
| `CohesionCommand` | `Include=secretstore.add-secret;secretstore.issue-certificate` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionEndpoint` | `Include=api`; `Scheme=https`; `Certificate=tls`; `ContainerPort=8443`; `Public=false` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionMount` | `Include=tls`; `Kind=Secret`; `ContainerPath=/cohesion/mounts/tls` | No additional child metadata | `Targets/Sdk.SecretStore.props` |
| `CohesionMount` | `Include=data`; `Kind=Volume`; `ContainerPath=/data`; `Size=10Gi` | No additional child metadata | `Targets/Sdk.SecretStore.props` |

The `FrameworkReference` item for `Assimalign.Cohesion.App.SecretStore` is added by `Sdk/Sdk.props`
when `CohesionAutoIncludeAppFramework` is not `false`. The defaults below come from this area’s own
props file.

## Artifacts and validation

When enabled, the inherited targets generate `cohesion/resource.json`, `Resource.g.cs`, and
`ResourceControlPlane.g.cs` beneath `IntermediateOutputPath`. The application-model package is
injected at `CohesionVersion` unless suppressed; generated registration uses the area type listed
above.

Pin disagreement is `COHSDK002`; an enabled non-executable is `COHSDK008`; a disabled resource
reference is `COHSDK001`. Kind-prefix errors are `COHSDK009` and invalid certificate metadata is
`COHSDK010`. Image and pack diagnostics are inherited; see the
[complete base diagnostic table](../sdk/overview.md#diagnostics). This layer adds no validation
target or generated artifact of its own.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.SecretStore/Targets/Sdk.SecretStore.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
