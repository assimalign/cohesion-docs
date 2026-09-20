# GatewayControlPlaneOptions

Configures the optional Local metadata root, token-validation clock, and exact resource-kind dispatcher registrations.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

## Remarks

Configures the optional Local metadata root, token-validation clock, and exact resource-kind
dispatcher registrations. The factory validates and snapshots registrations before serving.

## Members

| Member | Responsibility |
|---|---|
| `MetadataDirectory` | Optional Local discovery root; omission disables publication of `control-plane.json`. |
| `TimeProvider` | Clock for bearer-credential validation, defaulting to `TimeProvider.System`. |
| `CommandDispatchers` | Dispatcher registrations matched by exact resource kind in registration order. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/GatewayControlPlaneOptions/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlaneOptions.cs`.
