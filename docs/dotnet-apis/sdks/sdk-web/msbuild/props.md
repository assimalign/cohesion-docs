# Props

The Web props files add the App and App.Web framework references and Web's build defaults.

## `Sdk/Sdk.props`

Sets `_CohesionResourceSdk=true` before importing the base `Sdk.props`, so the shared defaults
select an executable. The base pin must be present in `global.json`. Adds `FrameworkReference`
items for `Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Web` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.Web.props`

| Name | Assigned value | Effect | Condition |
|---|---|---|---|
| `EventSourceSupport` | `true` | Keeps EventSource delivery in NativeAOT publishes. | when empty |
| `InterceptorsNamespaces` | `$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated` | Enables generated typed-handler interceptors. | always |
| `None Update="wwwroot/**"` | `PreserveNewest` for output and publish | Ships the web root. | always |

The file declares no orchestration metadata, endpoints, probes, or mounts: the resource's
composition lives in its `Program.cs`.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`.
