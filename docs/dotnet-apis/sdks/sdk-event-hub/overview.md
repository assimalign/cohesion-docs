# Overview

The EventHub SDK inherits shared build behavior and adds area-specific defaults.

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
| `CohesionResourceApplicationModel` | `Assimalign.Cohesion.EventHub.ApplicationModel` | Application-model assembly/package selected for an enabled resource. | `Targets/Sdk.EventHub.props`; when `'$(CohesionResourceApplicationModel)' == ''` |
| `CohesionResourceControlPlaneType` | `Assimalign.Cohesion.EventHub.ApplicationModel.EventHubResourceControlPlane` | Fully qualified control-plane type used by generated registration. | `Targets/Sdk.EventHub.props`; when `'$(CohesionResourceControlPlaneType)' == ''` |
| `CohesionResourceKind` | `EventHub` | Resource kind recorded in the manifest. | `Targets/Sdk.EventHub.props`; when `'$(CohesionResourceKind)' == ''` |
| `CohesionControlPlaneEndpoint` | `http` | Endpoint used for the default control plane. | `Targets/Sdk.EventHub.props`; when `'$(CohesionControlPlaneEndpoint)' == ''` |
| `CohesionControlPlanePath` | `/cohesion/v1` | Absolute control-plane route prefix. | `Targets/Sdk.EventHub.props`; when `'$(CohesionControlPlanePath)' == ''` |
| `CohesionWorkloadKind` | `Deployment` | Workload lifecycle kind written to the manifest. | `Targets/Sdk.EventHub.props`; when `'$(CohesionWorkloadKind)' == ''` |

## Area items

| Item | Declaration | Metadata | Where defined |
|---|---|---|---|
| `CohesionEndpoint` | `Include=http`; `Scheme=http`; `ContainerPort=8080`; `Public=false` | No additional child metadata | `Targets/Sdk.EventHub.props` |
| `CohesionProbe` | `Include=readiness`; `Endpoint=http`; `Http=/readyz` | No additional child metadata | `Targets/Sdk.EventHub.props` |
| `CohesionProbe` | `Include=liveness`; `Endpoint=http`; `Http=/livez` | No additional child metadata | `Targets/Sdk.EventHub.props` |

The `FrameworkReference` item for `Assimalign.Cohesion.App.EventHub` is added by `Sdk/Sdk.props`
when `CohesionAutoIncludeAppFramework` is not `false`. After substituting area names, this props
file is byte-identical to the `ApiManager`, `EmailHub`, `IoTHub`, `MediaHub`, `MessageHub`,
`NotificationHub` props files.

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

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Targets/Sdk.EventHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.EventHub/Targets/Sdk.EventHub.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
