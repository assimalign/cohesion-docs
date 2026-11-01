# Overview

The Web SDK inherits shared build behavior and adds Web's interceptor, event, and web-root defaults.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

## Chaining and overrides

The [base overview](../sdk/overview.md) is the inherited property, item, and diagnostic contract.
Properties conditioned on an empty value apply before the consumer project body, and later project
assignments override them.

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; the SDK sets `_CohesionResourceSdk` before the base import. | base `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `SelfContained` | `true` | Executables run self-contained, because the App frameworks ship only as runtime packs. | base `Sdk/Sdk.targets` |
| `CohesionAutoIncludeAppFramework` | Unset; inclusion enabled | `false` before the props import suppresses both framework references. | `Sdk/Sdk.props` |

## Area properties and items

| Name | Default or assignment | Effect | Where defined |
|---|---|---|---|
| `EventSourceSupport` | `true` when empty | NativeAOT applications keep delivering the Web server's operator events. | `Targets/Sdk.Web.props` |
| `InterceptorsNamespaces` | Appends `Assimalign.Cohesion.Web.Api.Generated` | Enables the generated typed-handler interceptors. | `Targets/Sdk.Web.props` |
| `None Update="wwwroot/**"` | `CopyToOutputDirectory` and `CopyToPublishDirectory` `PreserveNewest` | Ships the web root that `UseStaticFiles()` serves; a consumer's own `None Update` wins. | `Targets/Sdk.Web.props` |

`Sdk/Sdk.props` adds the `FrameworkReference` items for `Assimalign.Cohesion.App` and
`Assimalign.Cohesion.App.Web` when `CohesionAutoIncludeAppFramework` is not `false`.

## Artifacts and validation

The SDK generates no resource manifest and no generated resource or control-plane source; a
resource is managed by its `Program.cs` registration. Pin disagreement is `COHSDK002`, inherited
from the base SDK; this layer adds no validation of its own.

[SDK home](index.md) · [MSBuild](msbuild/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
