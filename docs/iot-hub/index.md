# IoTHub

IoTHub is the Cohesion resource area intended for device identity, provisioning, telemetry ingress, commands, and device state.

> **Status:** Partial. The enabled-resource host and declarative plane work; domain services remain fillers.

## What it is

IoTHub is an L3 service platform. Its intended scope covers device identity, provisioning, telemetry
ingress, commands, and device state.
Those domain capabilities are not supplied by the current host. The executable provides explicit
service registration and resource management while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[IoTHub API reference](../dotnet-apis/resources/iot-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.IoTHub`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.IoTHub.ApplicationModel`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub-applicationmodel/index.md) | Manifest-backed resource, typed descriptor, planner, and default control-plane factory. |
| [`Assimalign.Cohesion.IoTHub.Hosting`](../dotnet-apis/resources/iot-hub/assimalign-cohesion-iothub-hosting/index.md) | Concrete builder and host lifecycle; private resource control-plane listener. |

## Hosting model

`IoTHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.IoTHub.Hosting` creates the
concrete `IoTHubApplicationBuilder`. Its `Build()` returns
`IoTHubApplication`, which supports `RunAsync` and asynchronous disposal.
The root `IIoTHubApplication` contract exposes `Context`, `StartAsync`, and `StopAsync`.

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

`Assimalign.Cohesion.IoTHub.ApplicationModel` contributes `AddIoTHub(manifest, options)`,
returning `IIoTHubResourceDescriptor`. The manifest-backed `IoTHubResource` and
`IoTHubResourceOptions` retain dependency edges and feed the area's planner.

The planner validates the `IoTHub` kind, a `Deployment` workload, endpoint protocols and schemes,
and an `http` control plane at `/cohesion/v1`. It emits a platform-neutral `ResourcePlan`;
the software development kit (SDK) defaults to HTTP over Transmission Control Protocol (TCP) on port 8080.

`IoTHubResourceControlPlane.Create()` supplies the default transport-neutral plane.
The package is NuGet-only and guarded by `COHAM001`; it references neither the runtime host
nor a platform gateway.

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.IoTHub` supplies the runtime through
`Assimalign.Cohesion.App.IoTHub`. The declarative `.ApplicationModel` package is separate
from the shared framework and is injected for enabled resource executables.
See the [IoTHub SDK reference](../dotnet-apis/sdks/sdk-iot-hub/index.md).

## Getting started

The area README demonstrates this minimal `Program.cs`. It runs the lifecycle shell; it does not
enable the deferred domain services.

```csharp
using Assimalign.Cohesion.IoTHub.Hosting;

IoTHubApplicationBuilder builder = IoTHubApplication.CreateBuilder(args);
await using IoTHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/IoTHub/README.md`.
- **Host contract** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/docs/DESIGN.md`.
- **Declarative plane** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.ApplicationModel/docs/DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
