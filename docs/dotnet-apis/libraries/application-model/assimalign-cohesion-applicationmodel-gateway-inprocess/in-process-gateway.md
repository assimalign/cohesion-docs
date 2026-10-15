# InProcessGateway

`InProcessGateway` is the sealed `ApplicationGateway` implementation for the `inprocess` provider.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

## Remarks

`InProcessGateway` is the sealed `ApplicationGateway` implementation for the `inprocess`
provider. Its default constructor uses `InProcessGatewayOptions` defaults; the options constructor
validates directory, timing, and restart values before realization begins.

At `Build()`, the gateway rejects plain executables, image/package-only resources, missing entry
bindings, non-composable manifests, and plans the local process cannot honor. At run time it starts
its `ProcessHost`, invokes members in dependency order, publishes loopback endpoints and lifecycle
state, and stops adopted hosts in reverse order.

The ordinary application-gateway lifetime stops the gateway and every adopted member. Callers
normally do not construct it directly; `Sdk.Gateway` provider selection does so from command-line
options.

## Members

| Member | Responsibility |
|---|---|
| `InProcessGateway()` | Constructs the provider with default options. |
| `InProcessGateway(InProcessGatewayOptions)` | Validates and applies explicit provider options. |
| `Name` | Identifies the provider as `inprocess`. |
| `RenderAsync` | Renders the offline local plan-set envelope without invoking member entry points. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/InProcessGateway/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessGateway.cs`.
