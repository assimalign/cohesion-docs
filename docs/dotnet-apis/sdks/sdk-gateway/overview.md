# Overview

The Gateway SDK's properties, its one implicit reference, and the decisions behind them.

> **Status:** Implemented.

## Properties

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Set when empty, before `Microsoft.NET.Sdk` defaults it to `Library`. | `Sdk/Sdk.props` |
| `SelfContained` | `true` (executables) | Set when empty, so a gateway that references a self-contained `Sdk.<Area>` resource project builds and starts. | `Sdk/Sdk.targets` |
| `RuntimeIdentifier` | `$(NETCoreSdkRuntimeIdentifier)` | Set when empty and self-contained. | `Sdk/Sdk.targets` |
| `ValidateExecutableReferencesMatchSelfContained` | `false` | Set when empty, as for resource executables. | `Sdk/Sdk.targets` |
| `CohesionGatewayImplicitReference` | (empty) | `false` removes the implicit `Assimalign.Cohesion.ApplicationModel.Gateway` reference. | `Targets/Sdk.Gateway.props` |

The gateway inherits the base SDK's [project defaults](../sdk/overview.md#project-defaults):
`net10.0`, preview language and features, disabled implicit usings, enabled nullable analysis, and
AOT compatibility analysis.

## Items

| Item | Declaration | Condition |
|---|---|---|
| `PackageReference` | `Include="Assimalign.Cohesion.ApplicationModel.Gateway"`, `Version="$(CohesionVersion)"`, `IsImplicitlyDefined="true"` | `CohesionGatewayImplicitReference` is not `false` |

`$(CohesionVersion)` comes from the base SDK's frozen `Build.Version.props`. The reference brings the
gateway package's closure: Definition, Protocol, and the identity package.

## Decisions

- **No framework** — A gateway manages resources through their platform and never loads a
  resource's runtime, so it takes no `App` or `App.<Area>` framework and no area package. There is
  no `Assimalign.Cohesion.App.Gateway`.
- **Self-contained by default** — An in-process gateway references each resource's `Sdk.<Area>`
  project, and resource executables are self-contained and RID-specific; their App framework
  references flow in transitively and ship only as runtime packs. A gateway with no in-process
  resource may set `SelfContained=false`.
- **No platform by default** — A gateway runs on exactly the platform its `Program.cs` registers,
  so the platform is the consumer's own package reference: the in-process gateway half or a
  `cohesion-platforms` gateway half such as `Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes`.
- **Nothing generated** — The SDK runs no task and generates no source. The model is
  `ApplicationGateway.CreateBuilder(name)` and one `AddResource(provider, name)` per resource.

## Diagnostics

Pin disagreement is `COHSDK002`, inherited from the base SDK. The Gateway SDK adds no validation of
its own.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
