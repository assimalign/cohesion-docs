# GatewayControlPlane

Static composition entry point.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

## Remarks

Static composition entry point. `CreateFactory` produces application-scoped servers,
`CreateClient` produces either gateway-context or fixed-credential resolver clients, and
`Configure` installs non-destructive SDK defaults for the selected run mode.

## Members

| Member | Responsibility |
|---|---|
| `CreateFactory` | Creates application-scoped control-plane servers, with default or supplied options. |
| `CreateClient` | Creates a resolver client using gateway context or fixed credentials. |
| `Configure(ApplicationGatewayOptions, GatewayRunMode)` | Installs non-destructive SDK defaults for the selected mode. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/GatewayControlPlane/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlane.cs`.
