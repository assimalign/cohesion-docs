# Assimalign.Cohesion.ApplicationModel.Gateway

The gateway that runs one application model: it deploys resources through a platform in dependency order, receives their publications, lets child gateways join, and runs operators' actions.

> **Status:** Partial. The in-process, Kubernetes, and Docker platforms deploy through it; authentication of publications and calls belongs to each platform and remains open.

## Reference pages

- **[Design](design.md)** — Deployment order, readiness, records, leases, joins, and actions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

A gateway is an executable on [`Assimalign.Cohesion.Sdk.Gateway`](../../../sdks/sdk-gateway/index.md).
Its `Program.cs` declares the model with `ApplicationGateway.CreateBuilder(name)`, registers the
platform it deploys through, and runs. A resource is declared generically by provider and name; no
resource area ships a gateway-side package.

## Public types

| Area | Types |
|---|---|
| Composition | `ApplicationGateway`, `ApplicationGatewayBuilder`, `IApplicationGatewayBuilder`, `IApplicationGateway`, `IApplicationGatewayContext`, `ApplicationGatewayExtensions` (`RunAsync`) |
| Platform seams | `IApplicationGatewayInfrastructureProvider` (also the inbound channel), `IApplicationGatewayInstanceClient`, `IApplicationGatewayParent` |
| Deployments | `IApplicationGatewayDeployment`, `IApplicationGatewayDeploymentAction`, `ApplicationGatewayDeploymentPlan`, `ApplicationGatewayDeploymentChange`, `ApplicationGatewayDeploymentRecord`, `ApplicationGatewayOutputValue`, `ApplicationGatewayMembership` |
| Record | `IApplicationGatewayStore`, `ApplicationGatewayRecord`, `JsonFileApplicationGatewayStore` |
| Errors | `ApplicationGatewayException`, `ApplicationGatewayErrorCode` |

## Usage

The root gateway declares its model and runs it through a platform's infrastructure:

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("root");
builder.AddResource("IdentityHub", "identity");

ApplicationGateway root = builder.UseInfrastructure(infrastructure).Build();
await root.RunAsync(cancellationToken);
```

A child gateway is its own program. It joins its parent before it deploys anything, and its
`orders-api` deploys once `orders-db` is ready, with both endpoints in its configuration:

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("east");
IApplicationModelParent root = builder.UseParent(rootGateway);

IApplicationResourceDefinition database = builder.AddResource("Database.Sql", "orders-db");
builder.AddResource("Web", "orders-api")
    .DependsOn(database, "data.read")
    .WithSetting("Identity:Authority", root.Endpoint("IdentityHub", "identity"))
    .WithSetting("Database:Endpoint", database.Endpoint);

ApplicationGateway east = builder.UseInfrastructure(infrastructure).Build();
await east.RunAsync(cancellationToken);
```

An operator's action runs on one instance's control plane; a write's state is recorded and restored
on every other instance, and an instance that starts later restores it when it publishes:

```csharp
using Assimalign.Cohesion.ApplicationModel;

ApplicationActionResult result = await gateway.ExecuteAsync(
    "Web",
    "orders-api",
    "cors",
    ApplicationActionContext.Create(ApplicationActionOperation.Write, serializer, """{"origins":["https://partner.example"]}"""u8.ToArray()));
```

A host that drives the run itself calls `StartAsync` and `StopAsync` and watches `Completion`, which
faults when the run fails after startup. To run every resource in the gateway's own process, use
[Gateway.InProcess](../assimalign-cohesion-applicationmodel-gateway-inprocess/index.md):
`builder.UseInProcess(infrastructure)`.

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel.Definition` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Protocol` | `CohesionProjectReference` |

The gateway references the protocol but never the instance runtime. It is a NuGet package, not a
shared-framework member; `Sdk.Gateway` references it implicitly.

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Assimalign.Cohesion.ApplicationModel.Gateway.csproj`.
