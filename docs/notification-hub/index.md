# NotificationHub

NotificationHub is the Cohesion resource area intended for audiences, subscriptions, templates, channel routing, receipts, and suppression.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

NotificationHub is an L3 service platform. Its intended scope covers audiences, subscriptions,
templates, channel routing, receipts, and suppression.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[NotificationHub API reference](../dotnet-apis/resources/notification-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.NotificationHub`](../dotnet-apis/resources/notification-hub/assimalign-cohesion-notificationhub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.NotificationHub.Client`](../dotnet-apis/resources/notification-hub/assimalign-cohesion-notificationhub-client/index.md) | Placeholder client project; no implemented client operations. |
| [`Assimalign.Cohesion.NotificationHub.Hosting`](../dotnet-apis/resources/notification-hub/assimalign-cohesion-notificationhub-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.NotificationHub.Hosting.ApplicationModel`](../dotnet-apis/resources/notification-hub/assimalign-cohesion-notificationhub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`NotificationHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.NotificationHub.Hosting` creates the concrete
`NotificationHubApplicationBuilder`. Its `Build()` returns `NotificationHubApplication`, which supports `RunAsync`
and asynchronous disposal. The root `INotificationHubApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`NotificationHubApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("notifications", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.NotificationHub.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. NotificationHub exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("NotificationHub", "notifications")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.NotificationHub` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.NotificationHub`; the application references only the platform packages it
opts into. See the [NotificationHub SDK reference](../dotnet-apis/sdks/sdk-notification-hub/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.NotificationHub.Hosting;
using Assimalign.Cohesion.NotificationHub.Hosting.ApplicationModel;

NotificationHubApplicationBuilder builder = NotificationHubApplication.CreateBuilder(args);
builder.Services.AddResource("notifications", resource => resource.UseInProcess());

await using NotificationHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/NotificationHub/README.md`.
- **Host contract** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
