# VpnGateway

VpnGateway is the Cohesion resource area intended for virtual private network tunnels, peers, key exchange, routes, and session lifecycle.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

VpnGateway is an L3 service platform. Its intended scope covers virtual private network tunnels,
peers, key exchange, routes, and session lifecycle.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[VpnGateway API reference](../dotnet-apis/resources/vpn-gateway/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.VpnGateway`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.VpnGateway.Hosting`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.VpnGateway.Hosting.ApplicationModel`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`VpnGatewayApplication.CreateBuilder(args)` in `Assimalign.Cohesion.VpnGateway.Hosting` creates the concrete
`VpnGatewayApplicationBuilder`. Its `Build()` returns `VpnGatewayApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IVpnGatewayApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`VpnGatewayApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("tunnel", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.VpnGateway.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. VpnGateway exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("VpnGateway", "tunnel")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.VpnGateway` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.VpnGateway`; the application references only the platform packages it
opts into. See the [VpnGateway SDK reference](../dotnet-apis/sdks/sdk-vpn-gateway/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.VpnGateway.Hosting;
using Assimalign.Cohesion.VpnGateway.Hosting.ApplicationModel;

VpnGatewayApplicationBuilder builder = VpnGatewayApplication.CreateBuilder(args);
builder.Services.AddResource("tunnel", resource => resource.UseInProcess());

await using VpnGatewayApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/VpnGateway/README.md`.
- **Host contract** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.Hosting/docs/OVERVIEW.md` and `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
