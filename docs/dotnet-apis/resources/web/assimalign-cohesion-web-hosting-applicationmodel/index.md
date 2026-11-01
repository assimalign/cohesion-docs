# Assimalign.Cohesion.Web.Hosting.ApplicationModel

The Web resource integration: `builder.Services.AddResource(name, ...)` makes a Web application a managed resource instance whose actions expose its registered settings.

> **Status:** Implemented. The `cors` setting is the one Web action so far; named CORS policies and predicate policies stay fixed in code.

## Reference pages

- **[Design](design.md)** — Composition, lifecycle, and decisions W4 to W10.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Through the client of the platform its gateway runs it on, the application publishes its manifest,
restores the state the gateway kept, and lets the gateway run the resource's actions. The
application's own composition does not change: `builder.Services.AddCors(...)` and `app.UseCors()`
stay as they are, and the resource exposes the setting `AddCors` registered as the `cors` action.
Without a platform named in its environment, the application runs unmanaged with the settings its
code defines.

## Public types

| Type | Role |
|---|---|
| `WebResourceExtensions` | `AddResource(ResourceName name, Action<WebResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on `builder.Services`. |
| `WebResourceOptions` | Derives from `ApplicationInstanceOptions`, so every platform's verb (`UseInProcess()`) and `AddAction` apply; it adds only `Name`. |

The `cors` action, its content type, and the resource itself are internal.

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Web.Cors;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(cors => cors.Options.AddDefaultPolicy(policy => policy.WithOrigins("https://shop.example")));
builder.Services.AddResource("orders-api", resource => resource.UseInProcess());

WebApplication app = builder.Build();
app.UseCors();
await app.RunAsync();
```

The `cors` action's content is the default policy in camel case, such as
`{"origins": ["https://shop.example"], "methods": ["GET"], "allowCredentials": false}`. An empty
`origins` list means no default policy. A write changes the setting, which the CORS middleware
applies from the next request; the gateway restores it on every other instance. An application adds
an action of its own with the JSON context that declares its content types:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

builder.Services.AddResource("orders-api", resource => resource
    .UseInProcess()
    .AddAction(services => new FlushCacheAction(services.GetRequiredService<OrderCache>()))
    .AddAction(new LogLevelAction(), OrdersJsonContext.Default));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Cors` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to [Web.Hosting](../assimalign-cohesion-web-hosting/index.md)
by this project's exact name, with no exemption property. The package references no platform: the
application references each platform it opts into, such as
[Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md).
It is a public member of `Assimalign.Cohesion.App.Web`, so an `Sdk.Web` application needs no
package reference for it.

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/src/Assimalign.Cohesion.Web.Hosting.ApplicationModel.csproj`.
