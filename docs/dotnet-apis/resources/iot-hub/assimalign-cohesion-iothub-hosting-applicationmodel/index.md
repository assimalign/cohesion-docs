# Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel

The IoTHub resource integration: `builder.Services.AddResource(name, ...)` makes an IoTHub application a managed resource instance.

> **Status:** Implemented. The resource exposes only the actions the application adds; IoTHub has no actions of its own yet.

## Reference pages

- **[Design](design.md)** — Composition, lifecycle, and decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Through the client of the platform its gateway runs it on, the resource publishes its manifest,
restores the state the gateway kept, and lets the gateway run its actions. The application's own
composition does not change. Without a platform named in its environment, the application runs
unmanaged and nothing is registered.

## Public types

| Type | Role |
|---|---|
| `IoTHubResourceExtensions` | `AddResource(ResourceName name, Action<IoTHubResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on the application builder's `Services`. |
| `IoTHubResourceOptions` | Derives from `ApplicationInstanceOptions`, so every platform's verb (`UseInProcess()`) and `AddAction` apply; it adds only `Name`. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.IoTHub.Hosting;
using Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel;

IoTHubApplicationBuilder builder = IoTHubApplication.CreateBuilder(args);
builder.Services.AddResource("devices", resource => resource.UseInProcess());   // a resource on the in-process platform

await using IoTHubApplication application = builder.Build();
await application.RunAsync();
```

The resource opts into each platform it runs on with that platform's verb; a platform it did not
opt into fails the application's build, naming the platform. An application adds an action of its
own with the JSON context that declares the action's content types:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel;

builder.Services.AddResource("devices", resource => resource
    .UseInProcess()
    .AddAction(services => new DisableDeviceAction(services.GetRequiredService<DeviceRegistry>()))
    .AddAction(new DeviceTwinAction(), DevicesJsonContext.Default));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.IoTHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IoTHub.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to the [runtime module](../assimalign-cohesion-iothub-hosting/index.md) by this project's exact
name, with no exemption property. The package references no platform: the application references
each platform it opts into, such as [Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md). It is a public member of
`Assimalign.Cohesion.App.IoTHub`. The instance contracts are
[ApplicationModel.Hosting](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting/index.md).

[Parent: IoTHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel/src/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel.csproj`.
