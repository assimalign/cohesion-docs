# ApiManager

ApiManager is the Cohesion resource area intended for API backends, routes, products, subscriptions, contracts, and gateway policies.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

ApiManager is an L3 service platform. Its intended scope covers API backends, routes, products,
subscriptions, contracts, and gateway policies.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[ApiManager API reference](../dotnet-apis/resources/api-manager/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.ApiManager`](../dotnet-apis/resources/api-manager/assimalign-cohesion-apimanager/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.ApiManager.Hosting`](../dotnet-apis/resources/api-manager/assimalign-cohesion-apimanager-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.ApiManager.Hosting.ApplicationModel`](../dotnet-apis/resources/api-manager/assimalign-cohesion-apimanager-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`ApiManagerApplication.CreateBuilder(args)` in `Assimalign.Cohesion.ApiManager.Hosting` creates the concrete
`ApiManagerApplicationBuilder`. Its `Build()` returns `ApiManagerApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IApiManagerApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`ApiManagerApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("apis", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.ApiManager.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. ApiManager exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("ApiManager", "apis")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.ApiManager` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.ApiManager`; the application references only the platform packages it
opts into. See the [ApiManager SDK reference](../dotnet-apis/sdks/sdk-api-manager/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.ApiManager.Hosting;
using Assimalign.Cohesion.ApiManager.Hosting.ApplicationModel;

ApiManagerApplicationBuilder builder = ApiManagerApplication.CreateBuilder(args);
builder.Services.AddResource("apis", resource => resource.UseInProcess());

await using ApiManagerApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/ApiManager/README.md`.
- **Host contract** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Hosting/docs/OVERVIEW.md` and `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
