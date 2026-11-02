# EventHub

EventHub is the Cohesion resource area intended for partitioned event streams, consumer groups, checkpoints, retention, and replay.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

EventHub is an L3 service platform. Its intended scope covers partitioned event streams, consumer
groups, checkpoints, retention, and replay.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[EventHub API reference](../dotnet-apis/resources/event-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.EventHub`](../dotnet-apis/resources/event-hub/assimalign-cohesion-eventhub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.EventHub.Hosting`](../dotnet-apis/resources/event-hub/assimalign-cohesion-eventhub-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.EventHub.Hosting.ApplicationModel`](../dotnet-apis/resources/event-hub/assimalign-cohesion-eventhub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`EventHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.EventHub.Hosting` creates the concrete
`EventHubApplicationBuilder`. Its `Build()` returns `EventHubApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IEventHubApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`EventHubApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("events", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.EventHub.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. EventHub exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("EventHub", "events")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.EventHub` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.EventHub`; the application references only the platform packages it
opts into. See the [EventHub SDK reference](../dotnet-apis/sdks/sdk-event-hub/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.EventHub.Hosting;
using Assimalign.Cohesion.EventHub.Hosting.ApplicationModel;

EventHubApplicationBuilder builder = EventHubApplication.CreateBuilder(args);
builder.Services.AddResource("events", resource => resource.UseInProcess());

await using EventHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/EventHub/README.md`.
- **Host contract** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
