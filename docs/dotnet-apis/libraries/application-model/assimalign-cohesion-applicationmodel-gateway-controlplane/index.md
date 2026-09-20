# Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane

Exposes authenticated application discovery and resource command delivery over HTTP.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`GatewayControlPlane`](gateway-control-plane.md)** — type reference.

- **[`GatewayControlPlaneOptions`](gateway-control-plane-options.md)** — type reference.

- **[`IResourceCommandDispatcher`](i-resource-command-dispatcher.md)** — type reference.

[ApplicationModel](../index.md)

## Scope

The server publishes the gateway's `ApplicationExportDocument` rather than defining another export
model. Server state and listener lifetime are application-scoped. Resolver clients require explicit
credentials and the expected application trust key; issuer command grants are enforced before
dispatch.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.ApplicationModel`](../../application-model/assimalign-cohesion-applicationmodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.ApplicationModel.Gateway`](../../application-model/assimalign-cohesion-applicationmodel-gateway/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.Tcp`](../../connections/assimalign-cohesion-connections-tcp/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Resources`](../../hosting/assimalign-cohesion-hosting-resources/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Connections`](../../http/assimalign-cohesion-http-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`](../../identity-model/assimalign-cohesion-identitymodel-token-jsonwebtoken/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `GatewayControlPlane` | `src/GatewayControlPlane.cs` |
| `GatewayControlPlaneOptions` | `src/GatewayControlPlaneOptions.cs` |
| `IResourceCommandDispatcher` | `src/IResourceCommandDispatcher.cs` |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlane.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlaneOptions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/IResourceCommandDispatcher.cs`.
