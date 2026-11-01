# NatGateway

NatGateway is the Cohesion resource area intended for address translation rules, translation sessions, port allocation, and egress policy.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

NatGateway is an L3 service platform. Its intended scope covers address translation rules,
translation sessions, port allocation, and egress policy.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[NatGateway API reference](../dotnet-apis/resources/nat-gateway/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.NatGateway`](../dotnet-apis/resources/nat-gateway/assimalign-cohesion-natgateway/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.NatGateway.Hosting`](../dotnet-apis/resources/nat-gateway/assimalign-cohesion-natgateway-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.NatGateway.Hosting.ApplicationModel`](../dotnet-apis/resources/nat-gateway/assimalign-cohesion-natgateway-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`NatGatewayApplication.CreateBuilder(args)` in `Assimalign.Cohesion.NatGateway.Hosting` creates the concrete
`NatGatewayApplicationBuilder`. Its `Build()` returns `NatGatewayApplication`, which supports `RunAsync`
and asynchronous disposal. The root `INatGatewayApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`NatGatewayApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("egress", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.NatGateway.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. NatGateway exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("NatGateway", "egress")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.NatGateway` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.NatGateway`; the application references only the platform packages it
opts into. See the [NatGateway SDK reference](../dotnet-apis/sdks/sdk-nat-gateway/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.NatGateway.Hosting;
using Assimalign.Cohesion.NatGateway.Hosting.ApplicationModel;

NatGatewayApplicationBuilder builder = NatGatewayApplication.CreateBuilder(args);
builder.Services.AddResource("egress", resource => resource.UseInProcess());

await using NatGatewayApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/NatGateway/README.md`.
- **Host contract** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.Hosting/docs/OVERVIEW.md` and `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
