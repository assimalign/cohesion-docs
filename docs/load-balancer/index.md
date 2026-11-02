# LoadBalancer

LoadBalancer is the Cohesion resource area intended for backend pools, probes, routing, affinity, draining, and failover.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

LoadBalancer is an L3 service platform. Its intended scope covers backend pools, probes, routing,
affinity, draining, and failover.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[LoadBalancer API reference](../dotnet-apis/resources/load-balancer/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.LoadBalancer`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.LoadBalancer.Hosting`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`LoadBalancerApplication.CreateBuilder(args)` in `Assimalign.Cohesion.LoadBalancer.Hosting` creates the concrete
`LoadBalancerApplicationBuilder`. Its `Build()` returns `LoadBalancerApplication`, which supports `RunAsync`
and asynchronous disposal. The root `ILoadBalancerApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`LoadBalancerApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("ingress", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. LoadBalancer exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("LoadBalancer", "ingress")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.LoadBalancer` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.LoadBalancer`; the application references only the platform packages it
opts into. See the [LoadBalancer SDK reference](../dotnet-apis/sdks/sdk-load-balancer/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.LoadBalancer.Hosting;
using Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel;

LoadBalancerApplicationBuilder builder = LoadBalancerApplication.CreateBuilder(args);
builder.Services.AddResource("ingress", resource => resource.UseInProcess());

await using LoadBalancerApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/LoadBalancer/README.md`.
- **Host contract** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/OVERVIEW.md` and `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
