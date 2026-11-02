# Rezolvr

Rezolvr is the Cohesion resource area intended to be a standalone DNS server; its DNS runtime remains deferred.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

Rezolvr is the Domain Name System (DNS) server product area, intended for authoritative zones,
forwarding, recursion, caching, transfers, and administration. It is not Cohesion's service
discovery subsystem; discovery uses the outputs a gateway reads from each deployment and the
platform's own Service DNS.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[Rezolvr API reference](../dotnet-apis/resources/rezolvr/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.Rezolvr`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.Rezolvr.Hosting`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`RezolvrApplication.CreateBuilder(args)` in `Assimalign.Cohesion.Rezolvr.Hosting` creates the concrete
`RezolvrApplicationBuilder`. Its `Build()` returns `RezolvrApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IRezolvrApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`RezolvrApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("resolver", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. Rezolvr exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("Rezolvr", "resolver")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.Rezolvr` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.Rezolvr`; the application references only the platform packages it
opts into. See the [Rezolvr SDK reference](../dotnet-apis/sdks/sdk-rezolvr/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Rezolvr.Hosting;
using Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel;

RezolvrApplicationBuilder builder = RezolvrApplication.CreateBuilder(args);
builder.Services.AddResource("resolver", resource => resource.UseInProcess());

await using RezolvrApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/Rezolvr/README.md`.
- **Host contract** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
