# EmailHub

EmailHub is the Cohesion resource area intended for email composition, templates, transport selection, delivery tracking, and suppression.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

EmailHub is an L3 service platform. Its intended scope covers email composition, templates,
transport selection, delivery tracking, and suppression.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[EmailHub API reference](../dotnet-apis/resources/email-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.EmailHub`](../dotnet-apis/resources/email-hub/assimalign-cohesion-emailhub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.EmailHub.Hosting`](../dotnet-apis/resources/email-hub/assimalign-cohesion-emailhub-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel`](../dotnet-apis/resources/email-hub/assimalign-cohesion-emailhub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`EmailHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.EmailHub.Hosting` creates the concrete
`EmailHubApplicationBuilder`. Its `Build()` returns `EmailHubApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IEmailHubApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`EmailHubApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("mail", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. EmailHub exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("EmailHub", "mail")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.EmailHub` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.EmailHub`; the application references only the platform packages it
opts into. See the [EmailHub SDK reference](../dotnet-apis/sdks/sdk-email-hub/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.EmailHub.Hosting;
using Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel;

EmailHubApplicationBuilder builder = EmailHubApplication.CreateBuilder(args);
builder.Services.AddResource("mail", resource => resource.UseInProcess());

await using EmailHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/EmailHub/README.md`.
- **Host contract** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
