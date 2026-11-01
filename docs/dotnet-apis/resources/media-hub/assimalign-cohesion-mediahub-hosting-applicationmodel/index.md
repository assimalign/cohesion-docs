# Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel

The MediaHub resource integration: `builder.Services.AddResource(name, ...)` makes a MediaHub application a managed resource instance.

> **Status:** Implemented. The resource exposes only the actions the application adds; MediaHub has no actions of its own yet.

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
| `MediaHubResourceExtensions` | `AddResource(ResourceName name, Action<MediaHubResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on the application builder's `Services`. |
| `MediaHubResourceOptions` | Derives from `ApplicationInstanceOptions`, so every platform's verb (`UseInProcess()`) and `AddAction` apply; it adds only `Name`. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.MediaHub.Hosting;
using Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel;

MediaHubApplicationBuilder builder = MediaHubApplication.CreateBuilder(args);
builder.Services.AddResource("media", resource => resource.UseInProcess());   // a resource on the in-process platform

await using MediaHubApplication application = builder.Build();
await application.RunAsync();
```

The resource opts into each platform it runs on with that platform's verb; a platform it did not
opt into fails the application's build, naming the platform. An application adds an action of its
own with the JSON context that declares the action's content types:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel;

builder.Services.AddResource("media", resource => resource
    .UseInProcess()
    .AddAction(services => new PurgeCacheAction(services.GetRequiredService<ContentCache>()))
    .AddAction(new TranscodeProfileAction(), MediaJsonContext.Default));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.MediaHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.MediaHub.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to the [runtime module](../assimalign-cohesion-mediahub-hosting/index.md) by this project's exact
name, with no exemption property. The package references no platform: the application references
each platform it opts into, such as [Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md). It is a public member of
`Assimalign.Cohesion.App.MediaHub`. The instance contracts are
[ApplicationModel.Hosting](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting/index.md).

[Parent: MediaHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel/src/Assimalign.Cohesion.MediaHub.Hosting.ApplicationModel.csproj`.
