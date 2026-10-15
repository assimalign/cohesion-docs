# LoadBalancer

LoadBalancer is the Cohesion resource area intended for backend pools, probes, routing, affinity, draining, and failover.

> **Status:** Partial. The enabled-resource host and declarative plane work; domain services remain fillers.

## What it is

LoadBalancer is an L3 service platform. Its intended scope covers backend pools, probes, routing,
affinity, draining, and failover.
Those domain capabilities are not supplied by the current host. The executable provides explicit
service registration and resource management while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[LoadBalancer API reference](../dotnet-apis/resources/load-balancer/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.LoadBalancer`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.LoadBalancer.ApplicationModel`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer-applicationmodel/index.md) | Manifest-backed resource, typed descriptor, planner, and default control-plane factory. |
| [`Assimalign.Cohesion.LoadBalancer.Hosting`](../dotnet-apis/resources/load-balancer/assimalign-cohesion-loadbalancer-hosting/index.md) | Concrete builder and host lifecycle; private resource control-plane listener. |

## Hosting model

`LoadBalancerApplication.CreateBuilder(args)` in `Assimalign.Cohesion.LoadBalancer.Hosting` creates the
concrete `LoadBalancerApplicationBuilder`. Its `Build()` returns
`LoadBalancerApplication`, which supports `RunAsync` and asynchronous disposal.
The root `ILoadBalancerApplication` contract exposes `Context`, `StartAsync`, and `StopAsync`.

The concrete builder accepts `AddService` instance and context-factory registrations. Factories
run once per build; services start in registration order and stop in reverse order. Without an
enabled-resource registration, an empty host opens no listener and runs no domain service.

With `CohesionApplicationModel=enabled`, generated registration connects the host to
`ResourceRuntime`. The ambient `ResourceContext` provides identity, environment, content root,
endpoint bindings, and materialized mount paths. The `http` listener uses Hypertext Transfer Protocol
(HTTP) and exposes health,
readiness, liveness, endpoint discovery, graceful stop, and command envelopes. Managed namespaced
routes verify gateway bootstrap credentials using ES256 signatures. No domain command kinds are
registered; unsupported commands return HTTP 501.

Configuration and Secret mounts remain manifest-driven inputs resolved by the gateway. The host
does not supply a domain configuration loader merely because a mount exists. For HTTP over Transport
Layer Security (HTTPS), the hosting certificate accessor reads its named Secret mount, defaulting to
`tls`, as a Privacy-Enhanced Mail (PEM)
certificate/private-key/chain bundle.

## Application model

`Assimalign.Cohesion.LoadBalancer.ApplicationModel` contributes `AddLoadBalancer(manifest, options)`,
returning `ILoadBalancerResourceDescriptor`. The manifest-backed `LoadBalancerResource` and
`LoadBalancerResourceOptions` retain dependency edges and feed the area's planner.

The planner validates the `LoadBalancer` kind, a `Deployment` workload, endpoint protocols and schemes,
and an `http` control plane at `/cohesion/v1`. It emits a platform-neutral `ResourcePlan`;
the software development kit (SDK) defaults to HTTP over Transmission Control Protocol (TCP) on port 8080.

The SDK marks the artifact as non-composable; the planner preserves that constraint.

`LoadBalancerResourceControlPlane.Create()` supplies the default transport-neutral plane.
The package is NuGet-only and guarded by `COHAM001`; it references neither the runtime host
nor a platform gateway.

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.LoadBalancer` supplies the runtime through
`Assimalign.Cohesion.App.LoadBalancer`. The declarative `.ApplicationModel` package is separate
from the shared framework and is injected for enabled resource executables.
See the [LoadBalancer SDK reference](../dotnet-apis/sdks/sdk-load-balancer/index.md).

## Getting started

The area README demonstrates this minimal `Program.cs`. It runs the lifecycle shell; it does not
enable the deferred domain services.

```csharp
using Assimalign.Cohesion.LoadBalancer.Hosting;

LoadBalancerApplicationBuilder builder = LoadBalancerApplication.CreateBuilder(args);
await using LoadBalancerApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/LoadBalancer/README.md`.
- **Host contract** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/OVERVIEW.md` and `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/DESIGN.md`.
- **Declarative plane** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.ApplicationModel/docs/DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
