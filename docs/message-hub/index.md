# MessageHub

MessageHub is the Cohesion resource area intended for queues, topics, producers, consumers, settlement, retries, and dead-lettering.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

MessageHub is an L3 service platform. Its intended scope covers queues, topics, producers,
consumers, settlement, retries, and dead-lettering.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[MessageHub API reference](../dotnet-apis/resources/message-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.MessageHub`](../dotnet-apis/resources/message-hub/assimalign-cohesion-messagehub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.MessageHub.Client`](../dotnet-apis/resources/message-hub/assimalign-cohesion-messagehub-client/index.md) | Placeholder client project; no implemented client operations. |
| [`Assimalign.Cohesion.MessageHub.Hosting`](../dotnet-apis/resources/message-hub/assimalign-cohesion-messagehub-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.MessageHub.Hosting.ApplicationModel`](../dotnet-apis/resources/message-hub/assimalign-cohesion-messagehub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`MessageHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.MessageHub.Hosting` creates the concrete
`MessageHubApplicationBuilder`. Its `Build()` returns `MessageHubApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IMessageHubApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`MessageHubApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("messages", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.MessageHub.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. MessageHub exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("MessageHub", "messages")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.MessageHub` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.MessageHub`; the application references only the platform packages it
opts into. See the [MessageHub SDK reference](../dotnet-apis/sdks/sdk-message-hub/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.MessageHub.Hosting;
using Assimalign.Cohesion.MessageHub.Hosting.ApplicationModel;

MessageHubApplicationBuilder builder = MessageHubApplication.CreateBuilder(args);
builder.Services.AddResource("messages", resource => resource.UseInProcess());

await using MessageHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/MessageHub/README.md`.
- **Host contract** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
