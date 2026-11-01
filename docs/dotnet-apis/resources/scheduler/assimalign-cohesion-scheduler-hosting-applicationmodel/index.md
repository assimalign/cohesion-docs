# Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel

The Scheduler resource integration: `builder.Services.AddResource(name, ...)` makes a Scheduler application a managed resource instance.

> **Status:** Implemented. The resource exposes only the actions the application adds; Scheduler has no actions of its own yet.

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
| `SchedulerResourceExtensions` | `AddResource(ResourceName name, Action<SchedulerResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on the application builder's `Services`. |
| `SchedulerResourceOptions` | Derives from `ApplicationInstanceOptions`, so every platform's verb (`UseInProcess()`) and `AddAction` apply; it adds only `Name`. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Scheduler.Hosting;
using Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel;

SchedulerApplicationBuilder builder = SchedulerApplication.CreateBuilder(args);
builder.Services.AddResource("jobs", resource => resource.UseInProcess());   // a resource on the in-process platform

await using SchedulerApplication application = builder.Build();
await application.RunAsync();
```

The resource opts into each platform it runs on with that platform's verb; a platform it did not
opt into fails the application's build, naming the platform. An application adds an action of its
own with the JSON context that declares the action's content types:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel;

builder.Services.AddResource("jobs", resource => resource
    .UseInProcess()
    .AddAction(services => new PauseJobAction(services.GetRequiredService<JobCatalog>()))
    .AddAction(new TriggerJobAction(), JobsJsonContext.Default));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Scheduler` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Scheduler.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to the [runtime module](../assimalign-cohesion-scheduler-hosting/index.md) by this project's exact
name, with no exemption property. The package references no platform: the application references
each platform it opts into, such as [Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md). It is a public member of
`Assimalign.Cohesion.App.Scheduler`. The instance contracts are
[ApplicationModel.Hosting](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting/index.md).

[Parent: Scheduler](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel/src/Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel.csproj`.
