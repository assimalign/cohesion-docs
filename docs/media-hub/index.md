# MediaHub

MediaHub is the Cohesion resource area intended for media ingest, asset catalogs, processing jobs, packaging, and delivery policy.

> **Status:** Partial. The enabled-resource host and declarative plane work; domain services remain fillers.

## What it is

MediaHub is an L3 service platform. Its intended scope covers media ingest, asset catalogs,
processing jobs, packaging, and delivery policy.
Those domain capabilities are not supplied by the current host. The executable provides explicit
service registration and resource management while the area program remains unfinished.

See the [architecture overview](../overview.md) and
[MediaHub API reference](../dotnet-apis/resources/media-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.MediaHub`](../dotnet-apis/resources/media-hub/assimalign-cohesion-mediahub/index.md) | Hosting-free application, context, and builder contracts. |
| [`Assimalign.Cohesion.MediaHub.ApplicationModel`](../dotnet-apis/resources/media-hub/assimalign-cohesion-mediahub-applicationmodel/index.md) | Manifest-backed resource, typed descriptor, planner, and default control-plane factory. |
| [`Assimalign.Cohesion.MediaHub.Hosting`](../dotnet-apis/resources/media-hub/assimalign-cohesion-mediahub-hosting/index.md) | Concrete builder and host lifecycle; private resource control-plane listener. |

## Hosting model

`MediaHubApplication.CreateBuilder(args)` in `Assimalign.Cohesion.MediaHub.Hosting` creates the
concrete `MediaHubApplicationBuilder`. Its `Build()` returns
`MediaHubApplication`, which supports `RunAsync` and asynchronous disposal.
The root `IMediaHubApplication` contract exposes `Context`, `StartAsync`, and `StopAsync`.

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

`Assimalign.Cohesion.MediaHub.ApplicationModel` contributes `AddMediaHub(manifest, options)`,
returning `IMediaHubResourceDescriptor`. The manifest-backed `MediaHubResource` and
`MediaHubResourceOptions` retain dependency edges and feed the area's planner.

The planner validates the `MediaHub` kind, a `Deployment` workload, endpoint protocols and schemes,
and an `http` control plane at `/cohesion/v1`. It emits a platform-neutral `ResourcePlan`;
the software development kit (SDK) defaults to HTTP over Transmission Control Protocol (TCP) on port 8080.

`MediaHubResourceControlPlane.Create()` supplies the default transport-neutral plane.
The package is NuGet-only and guarded by `COHAM001`; it references neither the runtime host
nor a platform gateway.

## SDK and framework

The domain SDK `Assimalign.Cohesion.Sdk.MediaHub` supplies the runtime through
`Assimalign.Cohesion.App.MediaHub`. The declarative `.ApplicationModel` package is separate
from the shared framework and is injected for enabled resource executables.
See the [MediaHub SDK reference](../dotnet-apis/sdks/sdk-media-hub/index.md).

## Getting started

The area README demonstrates this minimal `Program.cs`. It runs the lifecycle shell; it does not
enable the deferred domain services.

```csharp
using Assimalign.Cohesion.MediaHub.Hosting;

MediaHubApplicationBuilder builder = MediaHubApplication.CreateBuilder(args);
await using MediaHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area and example** — `cohesion/resources/MediaHub/README.md`.
- **Host contract** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting/docs/DESIGN.md`.
- **Declarative plane** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.ApplicationModel/docs/DESIGN.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Framework boundaries** — `cohesion/.claude/rules/resource-areas.md`.
