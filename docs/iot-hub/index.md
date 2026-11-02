# IoTHub

IoTHub is the Cohesion resource area intended for device identity, provisioning, telemetry ingress, commands, and device state.

> **Status:** Partial. The host and its resource integration work; domain services remain fillers.

## What it is

IoTHub is an L3 service platform. Its intended scope covers device identity, provisioning, telemetry
ingress, commands, and device state.
Those domain capabilities are not supplied by the current host. The executable composes explicit
host services and can be managed by a gateway while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[IoTHub API reference](../dotnet-apis/resources/iot-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.IoTHub`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.IoTHub.Hosting`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub-hosting/index.md) | Concrete builder, dependency-injection composition, and host lifecycle. |
| [`Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance. |

## Hosting model

`IoTHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.IoTHub.Hosting` creates the concrete
`IoTHubApplicationBuilder`. Its `Build()` returns `IoTHubApplication`, which supports `RunAsync`
and asynchronous disposal. The root `IIoTHubApplication` contract exposes `Context`, `StartAsync`,
and `StopAsync`.

`IoTHubApplicationBuilder.Services` is the application's one composition registry. `AddService`
registers an `IHostService` instance or a context factory; factories run once per build, services
start in registration order and stop in reverse. An empty host opens no listener and runs no domain
service.

## Managed resource

An application becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("devices", resource => resource.UseInProcess())`. The verb ships in
`Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel`, a public member of the area framework. When the
environment names a platform the resource opted into, the instance publishes its manifest, restores
the state its gateway kept, and runs the gateway's actions; otherwise it registers nothing and the
program runs as a plain application. IoTHub exposes no actions of its own yet; the application can add
its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("IoTHub", "devices")`. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.IoTHub` supplies the runtime and the resource integration
through `Assimalign.Cohesion.App.IoTHub`; the application references only the platform packages it
opts into. See the [IoTHub SDK reference](../dotnet-apis/sdks/sdk-iot-hub/index.md).

## Getting started

This minimal `Program.cs` runs the lifecycle shell as a resource on the in-process platform; it
does not enable the deferred domain services.

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.IoTHub.Hosting;
using Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel;

IoTHubApplicationBuilder builder = IoTHubApplication.CreateBuilder(args);
builder.Services.AddResource("devices", resource => resource.UseInProcess());

await using IoTHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/IoTHub/README.md`.
- **Host contract** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
