# Assimalign.Cohesion.ApplicationModel.Hosting.InProcess

The instance half of the in-process platform: the gateway client a resource uses when its gateway runs it inside the gateway's own process.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Endpoint resolution, attach timing, and call context.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`UseInProcess()` opts any area's resource into the platform. The client it adds publishes and renews
through the gateway's in-process endpoint and attaches the instance's control plane there for direct
calls. It references no gateway, so a resource that opts in carries neither the gateway nor the
logical model. The gateway half is
[`Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`](../assimalign-cohesion-applicationmodel-gateway-inprocess/index.md).

## Public types

| Type | Role |
|---|---|
| `InProcessApplicationInstanceOptionsExtensions` | `UseInProcess()` on every area's `ApplicationInstanceOptions`; it returns the area's options type for chaining. |
| `InProcessPlatform` | The platform's name (`inprocess`) and the process's gateway endpoints. |
| `IInProcessGateway` | The contract the gateway half implements at its endpoint. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddResource("orders-api", resource => resource.UseInProcess());
```

The instance uses this client only when its gateway names the `inprocess` platform, which the
in-process gateway does for every program it runs, together with the endpoint the client reaches it
at. A gateway half registers itself at a unique endpoint:

```csharp
using System;

using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;

Uri endpoint = new($"inprocess://{Guid.NewGuid():N}/");
using IDisposable registration = InProcessPlatform.RegisterGateway(endpoint, gateway);
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

The package is a NuGet package the application references; it is not a member of any shared
framework.

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting.InProcess/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting.InProcess/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting.InProcess/src/Assimalign.Cohesion.ApplicationModel.Hosting.InProcess.csproj`.
