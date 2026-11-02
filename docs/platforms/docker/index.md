# Docker

The Docker platform runs a gateway's resources as containers on a Docker-compatible engine, with Podman as its local target.

> **Status:** Partial. Containers, polling publication, and actions on published loopback ports work end to end; authentication, remote engines, staged rollouts, and health checks are open.

## Packages

| Package | Half | Purpose |
|---|---|---|
| `Assimalign.Cohesion.ApplicationModel.Gateway.Docker` | Gateway | `DockerInfrastructure`: the infrastructure provider, the inbound channel, and the instance client in one. It talks to the engine's API directly, with no Docker SDK. |
| `Assimalign.Cohesion.ApplicationModel.Hosting.Docker` | Instance | `resource.UseDocker()` on a resource's `ApplicationInstanceOptions`. |

The two halves share one protocol file, `Hosting.Docker/shared/DockerProtocol.cs`, compiled into
both. The gateway runs on the engine's host and pulls: it reaches each instance on the loopback port
the engine publishes the container's control-plane port on, so a container needs no address of its
gateway.

## Gateway half

```csharp
using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.Docker;

await using DockerInfrastructure docker = DockerInfrastructure.Create(new DockerInfrastructureOptions
{
    Network = "shop",
});
docker
    .AddImage("Web", "orders-api", "orders-api:1.0")
    .AddImage("Web", "storefront", "storefront:1.0");

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("shop");
builder.UseDocker(docker);
IApplicationResourceDefinition orders = builder.AddResource("Web", "orders-api").WithDockerReplicas(2);
builder.AddResource("Web", "storefront").WithSetting("Orders:Url", orders.Endpoint);

await builder.Build().RunAsync();
```

| Member | Purpose |
|---|---|
| `DockerInfrastructure.Create(options)` | Resolves the engine the options name: `Endpoint`, `DOCKER_HOST`, then the platform's default socket or pipe. |
| `DockerInfrastructure.AddImage(provider, name, image)` | Binds a resource to the container image that runs it. |
| `DockerInfrastructureOptions` | `Endpoint`, `Network` (`cohesion`), `EndpointPort` (8080), `ControlPlanePort` (9090), `ImagePullPolicy` (`IfNotPresent`). |
| `builder.UseDocker(infrastructure)` | Sets the infrastructure as the gateway's provider, inbound channel, and instance client. |
| `resource.WithDockerReplicas(count)` | The resource's `replicas` extension on the `docker` platform. |

Each resource is one container per replica on the gateway's network under the resource's name. A
resource's `endpoint` output is `http://{name}:8080`, its name on the network; the engine also
publishes each container's `http` port on a loopback port of its host.

## Instance half

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.Docker;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddResource("orders-api", resource => resource.UseDocker());
```

The client is selected only when the instance's environment names the platform
(`COHESION_GATEWAY=docker`), which the gateway sets on every container it runs. The instance serves
its side of the publication channel and its control plane on its container's control-plane port.

## The exchange

1. The gateway creates the network once, pulls the image if missing, and creates and starts one
   container per replica with the runtime contract in its environment.
2. Every second it lists the network's containers and reads each one's channel state through the
   published loopback port; it hands an unanswered manifest to the gateway's publisher and posts the
   publication answer back.
3. The instance renews by moving its renewal count; a renewal the gateway does not know clears the
   answer, so the instance publishes again.
4. An operator's action is a `GET` or `POST` to `/actions/{name}`, and a restore a `PUT` to
   `/state/{name}`, on the container's published control-plane port.

## Development on an engine

The end-to-end tests run a gateway against a real engine: two `orders-api` instances and a
`storefront` configured with `orders-api`'s endpoint; a second gateway that adopts the containers the
first left running; and a program that fails its start, which fails the gateway's start with the
container's exit and the program's last output. They are skipped unless `COHESION_DOCKER_E2E=1`.

```pwsh
pwsh platforms/Docker/scripts/Publish-CohesionDockerFixtures.ps1
$env:COHESION_DOCKER_E2E = '1'
```

## Open work

- **Authentication** — Any process on the engine's host, and any container on the network, can
  reach an instance's channel and control plane; a per-container secret would close both.
- **Remote engines** — Ports are published on the engine's loopback, so the gateway must run on
  the engine's host; `tcp://`, TLS, and SSH engine endpoints are not supported.
- **Rollout** — An update starts every new replica at once beside the old ones.
- **Readiness and liveness** — Containers have no health check yet.

Return to [Platforms](../index.md).

## Sources

- **Area** — `cohesion-platforms/platforms/Docker/README.md`.
- **Gateway half** — `cohesion-platforms/platforms/Docker/Assimalign.Cohesion.ApplicationModel.Gateway.Docker/docs/OVERVIEW.md` and `cohesion-platforms/platforms/Docker/Assimalign.Cohesion.ApplicationModel.Gateway.Docker/docs/DESIGN.md`.
- **Instance half** — `cohesion-platforms/platforms/Docker/Assimalign.Cohesion.ApplicationModel.Hosting.Docker/docs/OVERVIEW.md`.
