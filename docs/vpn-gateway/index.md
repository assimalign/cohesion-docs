# VpnGateway

VpnGateway is the Cohesion resource area intended for virtual private network tunnels, peers, key exchange, routes, and session lifecycle.

> **Status:** Partial. The enabled-resource host and declarative plane work; domain services remain fillers.

## What it is

VpnGateway is an L3 service platform. Its intended scope covers virtual private network tunnels,
peers, key exchange, routes, and session lifecycle.
Those domain capabilities are not supplied by the current host. The executable provides explicit
service registration and resource management while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[VpnGateway API reference](../dotnet-apis/resources/vpn-gateway/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.VpnGateway`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.VpnGateway.ApplicationModel`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway-applicationmodel/index.md) | Manifest-backed resource, typed descriptor, planner, and default control-plane factory. |
| [`Assimalign.Cohesion.VpnGateway.Hosting`](../dotnet-apis/resources/vpn-gateway/assimalign-cohesion-vpngateway-hosting/index.md) | Concrete builder and host lifecycle; private resource control-plane listener. |

## Hosting model

`VpnGatewayApplication.CreateBuilder(args)` in `Assimalign.Cohesion.VpnGateway.Hosting` creates the
concrete `VpnGatewayApplicationBuilder`. Its `Build()` returns
`VpnGatewayApplication`, which supports `RunAsync` and asynchronous disposal.
The root `IVpnGatewayApplication` contract exposes `Context`, `StartAsync`, and `StopAsync`.

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

`Assimalign.Cohesion.VpnGateway.ApplicationModel` contributes `AddVpnGateway(manifest, options)`,
returning `IVpnGatewayResourceDescriptor`. The manifest-backed `VpnGatewayResource` and
`VpnGatewayResourceOptions` retain dependency edges and feed the area's planner.

The planner validates the `VpnGateway` kind, a `Deployment` workload, endpoint protocols and schemes,
and an `http` control plane at `/cohesion/v1`. It emits a platform-neutral `ResourcePlan`;
the software development kit (SDK) defaults to HTTP over Transmission Control Protocol (TCP) on port 8080.

The SDK marks the artifact as non-composable; the planner preserves that constraint.

`VpnGatewayResourceControlPlane.Create()` supplies the default transport-neutral plane.
The package is NuGet-only and guarded by `COHAM001`; it references neither the runtime host
nor a platform gateway.

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.VpnGateway` supplies the runtime through
`Assimalign.Cohesion.App.VpnGateway`. The declarative `.ApplicationModel` package is separate
from the shared framework and is injected for enabled resource executables.
See the [VpnGateway SDK reference](../dotnet-apis/sdks/sdk-vpn-gateway/index.md).

## Getting started

The area README demonstrates this minimal `Program.cs`. It runs the lifecycle shell; it does not
enable the deferred domain services.

```csharp
using Assimalign.Cohesion.VpnGateway.Hosting;

VpnGatewayApplicationBuilder builder = VpnGatewayApplication.CreateBuilder(args);
await using VpnGatewayApplication application = builder.Build();
await application.RunAsync();
```

The landing-zone template uses the same host entry point. Its network-role comment describes the
intended role; the current runtime does not implement a tunnel listener or peer management.

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/VpnGateway/README.md`.
- **Host contract** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.Hosting/docs/OVERVIEW.md` and `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.Hosting/docs/DESIGN.md`.
- **Declarative plane** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway.ApplicationModel/docs/DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Landing-zone host** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-landing-zone/Networking/Example.Networking.VpnGateway/Program.cs`.
