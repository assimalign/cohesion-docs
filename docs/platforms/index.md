# Platforms

A platform is a pair of packages that deploys a gateway's resources and carries every exchange between the gateway and its instances.

> **Status:** Partial. The in-process, Kubernetes, and Docker pairs deploy and manage Web resources end to end; authentication, more workload kinds, and storage are open.

## Platform pairs

A gateway runs one application model and deploys it through a platform. Each platform is two
packages that never reference each other:

| Half | Used by | Role |
|---|---|---|
| Gateway half (`ApplicationModel.Gateway.<Platform>`) | The gateway program | The infrastructure provider that deploys workloads, the inbound channel instances publish through, and the client the gateway calls their control planes with. |
| Instance half (`ApplicationModel.Hosting.<Platform>`) | Each resource's program | The gateway client a resource opts into on its options, such as `resource.UseKubernetes()`. |

The instance selects the client of the platform its environment names (`COHESION_GATEWAY`), so one
`Program.cs` that opts into several platforms runs on any of them. A platform the resource did not
opt into fails its build.

| Platform | Packages | Where | Target |
|---|---|---|---|
| In process | `ApplicationModel.Gateway.InProcess`, `ApplicationModel.Hosting.InProcess` | `cohesion` | Each program on a thread of the gateway's own process |
| [Kubernetes](kubernetes/index.md) | `ApplicationModel.Gateway.Kubernetes`, `ApplicationModel.Hosting.Kubernetes` | `cohesion-platforms` | Kubernetes clusters, kind on Podman for development |
| [Docker](docker/index.md) | `ApplicationModel.Gateway.Docker`, `ApplicationModel.Hosting.Docker` | `cohesion-platforms` | Docker-compatible engines, Podman locally |

## A gateway on a platform

A gateway is an executable on [`Assimalign.Cohesion.Sdk.Gateway`](../dotnet-apis/sdks/sdk-gateway/index.md)
that references the gateway half it deploys through and registers it on the builder:

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes;

await using KubernetesInfrastructure kubernetes = KubernetesInfrastructure.Create(new KubernetesInfrastructureOptions
{
    Namespace = "shop",
});
kubernetes.AddImage("Web", "orders-api", "localhost:5001/orders-api:1.0");

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("shop");
builder.UseKubernetes(kubernetes);
builder.AddResource("Web", "orders-api").WithReplicas(2);

await builder.Build().RunAsync();
```

Each resource's program opts into the platform with the instance half:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.Kubernetes;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

builder.Services.AddResource("orders-api", resource => resource.UseKubernetes());
```

The model stays the same across platforms; switching platform replaces the gateway half's package
and its registration. A platform's typed verbs, such as `WithReplicas`, record a platform extension
that only a gateway on that platform applies.

## Boundaries

- **No transport in the core packages** — Every transport belongs to a platform, push or pull.
- **No resource-area dependency** — Platform packages build on the generic gateway and instance
  contracts and reference no resource area. Resource areas never resolve a gateway assembly
  (`COHRES003`).
- **No NativeAOT mandate** — `cohesion-platforms` gateway halves are deploy-time control planes;
  the resource runtimes they deploy keep their NativeAOT requirement.
- **Authentication** — Publications and the gateway's calls are each platform's to authenticate;
  neither out-of-process pair authenticates its control-plane port yet.

Return to [Cohesion Documentation](../index.md).

## Sources

- **Architecture** — `cohesion-platforms/README.md`.
- **Application model** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
- **In-process pair** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/OVERVIEW.md`.
- **Platform areas** — `cohesion-platforms/platforms/Kubernetes/README.md` and `cohesion-platforms/platforms/Docker/README.md`.
