# Assimalign.Cohesion.NotificationHub.Hosting design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.NotificationHub.Hosting`.

> **Status:** Partial.

## Design intent

The hosting module implements the area root's contract-only application seam and composes the
application through its dependency-injection container. Runtime options and supporting services
remain internal.

## Filler execution model

The built host exposes the ordered `HostedServices` materialized from explicit builder
registrations and a production `HostEnvironment`; the collection remains empty when nothing is
registered. Instance and factory registrations share one order, factories run exactly once per
`Build()` after the context exists, and a null factory result fails the build. Services start in
registration order and stop in reverse through the shared host lifecycle, without claiming that
domain behavior exists.

## Boundaries

The module references the area root, DependencyInjection, and Hosting publicly. Its Web,
Web.Hosting, HTTP, and transport implementation dependencies are private, with their resolved
closure supplied by the area runtime framework. It references no application-model library: the
resource integration, `NotificationHub.Hosting.ApplicationModel`, references this module instead. It uses no
reflection or dynamic activation and remains trimming- and NativeAOT-safe.

## Resource integration

A managed instance is a registration the application makes: `builder.Services.AddResource(name,
...)` adds one `IHostService` that publishes, restores, and opens the control plane through the
platform's client before any other service starts.

## Concrete composition (T10 / O34)

`NotificationHubApplication.CreateBuilder(args)` returns the public concrete `NotificationHubApplicationBuilder`;
its `Build()` returns the public `NotificationHubApplication : Host<NotificationHubApplicationContext>`. The public
`NotificationHubApplicationContext` implements `INotificationHubApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

Background-work registration belongs to the concrete `NotificationHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<NotificationHubApplicationContext, IHostService>)`. Factories
run once per build against the same context retained by the application. Services start in
registration order and stop in reverse. No area-owned service abstraction is introduced.
## Dependency injection composition

`NotificationHubApplicationBuilder.Services` is the application's one composition registry, created with
`EnableDynamicCode = false`, `ValidateOnBuild = true`, and `ValidateScopes = true`. The module
registers only factories and instances, so the provider stays reflection-free. Every lifecycle
service is an `IHostService` registration, in registration order.

`Build` closes registration (the container becomes read-only, and a second `Build` throws),
creates the provider once, and resolves `IEnumerable<IHostService>` once into the snapshot the
host reuses for every start, stop, and rollback pass. The application owns the provider: disposal
stops the host, then disposes the provider, which disposes every factory-created service in reverse
creation order while instance registrations stay with their callers. A failed `Build` disposes the
provider before rethrowing. The concrete context exposes the provider as `ServiceProvider` to
hosting-layer factories; the area root and its features never see it.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.NotificationHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.Hosting/docs/DESIGN.md`.
- **Source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub.Hosting/src/Assimalign.Cohesion.NotificationHub.Hosting.csproj`.
