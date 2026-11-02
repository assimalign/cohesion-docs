# Assimalign.Cohesion.ApplicationModel.Hosting

The instance runtime: a resource instance, its actions and their state, its control plane, and the host service that joins it to its gateway.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Snapshots, leases, lifecycle, and platform clients.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

This is the physical side of the application model and the seam every resource area implements in
its `<Area>.Hosting.ApplicationModel` package. It ships in the `App` hosting kernel, so it is in the
closure of every resource application; it references no transport and no logical model.

## Public types

| Type | Role |
|---|---|
| `IApplicationInstance` | One running copy of a resource: provider, name, instance key, deployment, actions, and health contributors. |
| `IApplicationInstanceAction`, `IApplicationInstanceActionState`, `ApplicationInstanceActionState<T>` | An action the gateway can invoke, and the replayable snapshot state a write leaves. |
| `IApplicationInstanceHealthContributor`, `ApplicationInstanceHealthContribution`, `ApplicationInstanceHealthStatus` | Health contributions an instance reports. |
| `IApplicationInstanceControlPlane`, `ApplicationInstanceControlPlane` | Runs invocations and restores snapshots over an instance; it has no lifecycle of its own. |
| `IApplicationInstanceService`, `ApplicationInstanceService` | The host lifecycle service that publishes, restores, renews, and opens the control plane through a platform's client. |
| `IApplicationInstanceGatewayClient` | A platform's instance half: how an instance publishes to and is called by its gateway. |
| `ApplicationInstanceOptions` | The base of every area's resource options: the platforms a resource opts into, and `AddAction`. |

## Usage

An application opts its resource into each platform it may run on and adds an action of its own
with the JSON context that declares its content:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddResource("orders-api", resource => resource
    .UseInProcess()
    .AddAction(new LogLevelAction(), OrdersJsonContext.Default));
```

An area's integration composes these pieces once the application's container exists:

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Hosting;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Hosting;

if (options.GetGatewayClient() is not { } client)
{
    return services; // No platform named: the application runs unmanaged.
}

IApplicationContentSerializer serializer = JsonApplicationContentSerializer.Create([.. options.JsonContexts]);
services.AddSingleton<IHostService>(provider => ApplicationInstanceService.Create(
    ApplicationInstanceControlPlane.Create(new AreaResource(name, options.Deployment, options.CreateActions(provider)), serializer),
    client));
```

The area serves nothing itself: the platform's client calls the control plane's `ExecuteAsync` for
an invocation and `RestoreAsync` when the gateway replicates a write.

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Protocol` | `CohesionProjectReference` |

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting/docs/DESIGN.md`.
- **Area integration template** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel/src/Extensions/EmailHubResourceExtensions.cs`.
- **Kernel membership** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
