# InProcessGatewayExtensions

`InProcessGatewayExtensions` adds `UseInProcessGateway()` overloads to `IApplicationBuilder`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

## Remarks

`InProcessGatewayExtensions` adds `UseInProcessGateway()` overloads to `IApplicationBuilder`.
The parameterless overload selects default options. The configuring overload creates an
`InProcessGatewayOptions` instance, applies the callback, and selects the resulting gateway.

Generated gateway applications normally call `UseGateway(args)` so the `--gateway inprocess`
selection and command-line state options are applied consistently. The direct extensions remain
useful for explicit programmatic composition and tests.

## Members

| Member | Responsibility |
|---|---|
| `UseInProcessGateway()` | Selects an in-process gateway with default options on an application builder. |
| `UseInProcessGateway(Action<InProcessGatewayOptions>)` | Creates options, applies the callback, and selects the configured gateway. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/InProcessGatewayExtensions/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Extensions/InProcessGatewayExtensions.cs`.
