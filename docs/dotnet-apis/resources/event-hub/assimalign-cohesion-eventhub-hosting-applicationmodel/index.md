# Assimalign.Cohesion.EventHub.Hosting.ApplicationModel

The EventHub resource integration: `builder.Services.AddResource(name, ...)` makes an EventHub application a managed resource instance.

> **Status:** Implemented. The resource exposes only the actions the application adds; EventHub has no actions of its own yet.

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
| `EventHubResourceExtensions` | `AddResource(ResourceName name, Action<EventHubResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on the application builder's `Services`. |
| `EventHubResourceOptions` | Derives from `ApplicationInstanceOptions`, so every platform's verb (`UseInProcess()`) and `AddAction` apply; it adds only `Name`. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.EventHub.Hosting;
using Assimalign.Cohesion.EventHub.Hosting.ApplicationModel;

EventHubApplicationBuilder builder = EventHubApplication.CreateBuilder(args);
builder.Services.AddResource("events", resource => resource.UseInProcess());   // a resource on the in-process platform

await using EventHubApplication application = builder.Build();
await application.RunAsync();
```

The resource opts into each platform it runs on with that platform's verb; a platform it did not
opt into fails the application's build, naming the platform. An application adds an action of its
own with the JSON context that declares the action's content types:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.EventHub.Hosting.ApplicationModel;

builder.Services.AddResource("events", resource => resource
    .UseInProcess()
    .AddAction(services => new PausePartitionAction(services.GetRequiredService<PartitionSet>()))
    .AddAction(new RetentionAction(), EventsJsonContext.Default));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.EventHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.EventHub.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to the [runtime module](../assimalign-cohesion-eventhub-hosting/index.md) by this project's exact
name, with no exemption property. The package references no platform: the application references
each platform it opts into, such as [Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md). It is a public member of
`Assimalign.Cohesion.App.EventHub`. The instance contracts are
[ApplicationModel.Hosting](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting/index.md).

[Parent: EventHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub.Hosting.ApplicationModel/src/Assimalign.Cohesion.EventHub.Hosting.ApplicationModel.csproj`.
