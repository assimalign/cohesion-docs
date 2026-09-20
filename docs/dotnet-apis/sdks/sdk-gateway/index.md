# Assimalign.Cohesion.Sdk.Gateway

The Gateway SDK generates an application composition surface from resource manifests and provider contributions.

> **Status:** Partial. Manifest dependency restoration and external provider integration remain explicit completion gates.

Choose this SDK for the application gateway that references resources through
`CohesionResourceReference`. It forces an executable, orchestration-enabled `Composite` resource
and generates `Gateway.CreateBuilder(args)`, resource verbs, external declarations, and provider
selection.

Gateway suppresses the implicit `Assimalign.Cohesion.App` reference before importing base props.
There is no `Assimalign.Cohesion.App.Gateway` framework. The normal orchestration path uses NuGet
packages. The active in-process exception currently adds `Assimalign.Cohesion.App`,
`Assimalign.Cohesion.App.Web`, and `Assimalign.Cohesion.App.Database` explicitly.

## Minimal consumer

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Gateway">
  <PropertyGroup>
    <CohesionApplicationName>example</CohesionApplicationName>
    <CohesionGateways>Local</CohesionGateways>
  </PropertyGroup>
</Project>
```

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.1-preview.3",
    "Assimalign.Cohesion.Sdk.Gateway": "10.0.1-preview.3"
  }
}
```

Supply a `Program.cs` entry point and enabled resource references for a runnable application. An
empty same-application manifest set produces a build warning; the generated builder rejects a
zero-resource application. The minimal project shows SDK resolution and required identity, not a
complete application topology.

## Current boundary

The shipped dependency bootstrap includes the seven Web, Database, ConfigurationStore, SecretStore,
IdentityHub, Rezolvr, and LogSpace ApplicationModel packages and the SecretStore, Database, and
ConfigurationStore clients. Manifest-derived requirements are discovered after restore and cannot
rewrite `project.assets.json`. A restore-visible producer dependency descriptor remains future
work. Other kinds use generic `ResourceOptions` /`AddResource` mapping. Docker/Kubernetes providers
belong to external platform packages; the SDK contains their package selection but does not supply
their implementations.

- **Overview** — [Consumer properties, provider metadata, outputs, and diagnostics](overview.md).
- **MSBuild** — [Import graph and task scheduling](msbuild/index.md).
- **Task assembly** —
  [`Assimalign.Cohesion.Sdk.Gateway.Tasks`](assimalign-cohesion-sdk-gateway-tasks/index.md).
- **Platforms** — [Deployment platform documentation](../../../platforms/index.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionCreateResourceVerbs.cs`
- **Source** — `cohesion/global.json`
