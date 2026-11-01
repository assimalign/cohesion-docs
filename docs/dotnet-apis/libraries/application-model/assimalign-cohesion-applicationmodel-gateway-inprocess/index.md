# Assimalign.Cohesion.ApplicationModel.Gateway.InProcess

The gateway half of the in-process platform: it runs each resource's program on a thread of the gateway's own process.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Program binding, environments, the channel, and lifecycle.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`InProcessInfrastructure` is the infrastructure provider (`inprocess`), the channel its programs
publish through, and the client the gateway calls their control planes with. Each resource is a
program the gateway program references; deploying a resource calls the program's entry point on a
thread of its own, in an environment scope of its own.

## Public types

| Type | Role |
|---|---|
| `InProcessInfrastructure` | Binds programs with `AddProgram(provider, name, main)`, runs them, receives their publications, and calls their control planes. |
| `InProcessApplicationGatewayBuilderExtensions` | `builder.UseInProcess(infrastructure)` sets the infrastructure as the gateway's provider, channel, and instance client. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.InProcess;

await using var infrastructure = new InProcessInfrastructure();
infrastructure
    .AddProgram("Web", "orders-api", Orders.Program.MainAsync)
    .AddProgram("Web", "storefront", Storefront.Program.MainAsync);

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("local").UseInProcess(infrastructure);
IApplicationResourceDefinition orders = builder.AddResource("Web", "orders-api");
builder.AddResource("Web", "storefront").WithSetting("Orders:Url", orders.Endpoint);

await builder.Build().RunAsync(cancellationToken);
```

Each program is an ordinary application that opts into the platform with
[Hosting.InProcess](../assimalign-cohesion-applicationmodel-hosting-inprocess/index.md): a Web
program calls `builder.Services.AddResource(name, resource => resource.UseInProcess())`. It listens
on the loopback port the infrastructure assigned, reads its settings from configuration, publishes
over the in-process channel, and receives the gateway's calls as direct calls. `storefront` starts
only once `orders-api` has published, with `Orders:Url` set to the address `orders-api` serves.
Programs outlive the gateway's run; disposing the infrastructure stops them, the last deployed first.

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel.Gateway` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess` | `CohesionProjectReference` |

The reference to the instance half is the in-process rendezvous only. The package references no
resource area.

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.csproj`.
