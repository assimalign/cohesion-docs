# Kubernetes

The Kubernetes platform runs a gateway's resources on a cluster and reaches their instances only through the API server.

> **Status:** Partial. Deployments, Services, Lease-based publication, and pod-proxy actions work end to end on kind; other workload kinds, volumes, runtime inputs, autoscaling, and control-plane authentication are not back yet.

## Packages

| Package | Half | Purpose |
|---|---|---|
| `Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes` | Gateway | `KubernetesInfrastructure`: the infrastructure provider, the inbound channel, and the instance client in one. |
| `Assimalign.Cohesion.ApplicationModel.Hosting.Kubernetes` | Instance | `resource.UseKubernetes()` on a resource's `ApplicationInstanceOptions`. |

The two halves share one protocol file, `Hosting.Kubernetes/shared/KubernetesProtocol.cs`, compiled
into both. Because both reach the cluster only through its API server, a gateway runs inside the
cluster or outside it alike, and a pod needs no address of its gateway.

## Gateway half

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes;

await using KubernetesInfrastructure kubernetes = KubernetesInfrastructure.Create(new KubernetesInfrastructureOptions
{
    Namespace = "shop",
});
kubernetes
    .AddImage("Web", "orders-api", "localhost:5001/orders-api:1.0")
    .AddImage("Web", "storefront", "localhost:5001/storefront:1.0");

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("shop");
builder.UseKubernetes(kubernetes);
IApplicationResourceDefinition orders = builder.AddResource("Web", "orders-api").WithReplicas(2);
builder.AddResource("Web", "storefront").WithSetting("Orders:Url", orders.Endpoint);

await builder.Build().RunAsync();
```

| Member | Purpose |
|---|---|
| `KubernetesInfrastructure.Create(options)` | Connects to the cluster the options name: a kubeconfig path, `KUBECONFIG`, the default kubeconfig, or the pod's service account. |
| `KubernetesInfrastructure.AddImage(provider, name, image)` | Binds a resource to the container image that runs it. |
| `KubernetesInfrastructureOptions` | `KubeConfigPath`, `Context`, `Namespace` (`cohesion`), `EndpointPort` (8080), `ControlPlanePort` (9090), `ImagePullPolicy` (`IfNotPresent`). |
| `builder.UseKubernetes(infrastructure)` | Sets the infrastructure as the gateway's provider, inbound channel, and instance client. |
| `resource.WithReplicas(count)` | The resource's `replicas` extension on the `kubernetes` platform. |

Each resource is a Deployment and a Service in the gateway's namespace. The gateway also applies the
instances' ServiceAccount, Role, and RoleBinding, which allow only the namespace's Leases.

## Instance half

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.Kubernetes;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddResource("orders-api", resource => resource.UseKubernetes());
```

The client is selected only when the instance's environment names the platform
(`COHESION_GATEWAY=kubernetes`), which the gateway sets on every pod it runs. The instance publishes
and renews through a Lease of its own and serves its control plane on its pod's control-plane port,
which the gateway calls through the API server's pod proxy.

## The exchange

1. The gateway applies the namespace, the instance ServiceAccount and Role, and each resource's
   Deployment and Service.
2. A pod creates its Lease with its manifest; the gateway, watching the namespace's Leases, patches
   the Lease with its publication answer, which the pod reads.
3. The pod renews its Lease every third of the lease duration. A renewal the gateway does not know
   clears the answer, so the instance publishes again.
4. An operator's action is a `GET` or `POST` through `pods/{pod}:9090/proxy/actions/{name}`.

## Development on kind

The end-to-end test runs a gateway against a kind cluster with two `orders-api` instances and a
`storefront` configured with `orders-api`'s endpoint, then writes through the gateway and reads the
result from every instance. It needs the kind CLI and a rootful Podman machine, or `-Engine docker`
on a Docker engine; the fixture image must match the node's architecture.

```pwsh
pwsh platforms/Kubernetes/scripts/New-CohesionKindCluster.ps1
pwsh platforms/Kubernetes/scripts/Publish-CohesionKindFixtures.ps1
$env:COHESION_KIND_CONTEXT = 'kind-cohesion'
```

## Open work

- **Readiness and liveness** — The readiness probe assumes an `http` endpoint, so a worker would
  never become ready.
- **Control-plane authentication** — Calls through the pod proxy are the API server's to authorize,
  but another pod can reach a pod's control-plane port directly.
- **Backfill** — StatefulSet, DaemonSet, and Job workloads; volumes and claims; ConfigMap and Secret
  runtime inputs; autoscaling and disruption budgets; system installation and exposure; and image
  publication remain to come back from the original implementation.

Return to [Platforms](../index.md).

## Sources

- **Area** — `cohesion-platforms/platforms/Kubernetes/README.md`.
- **Gateway half** — `cohesion-platforms/platforms/Kubernetes/Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes/docs/OVERVIEW.md` and `cohesion-platforms/platforms/Kubernetes/Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes/docs/DESIGN.md`.
- **Instance half** — `cohesion-platforms/platforms/Kubernetes/Assimalign.Cohesion.ApplicationModel.Hosting.Kubernetes/docs/OVERVIEW.md`.
