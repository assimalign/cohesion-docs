# INotificationHubApplication

The `INotificationHubApplication` type belongs to `Assimalign.Cohesion.NotificationHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.NotificationHub` Assembly: `Assimalign.Cohesion.NotificationHub`

## Purpose and surface

The root contracts are hosting-free (O34): `INotificationHubApplication` exposes `Context`,
`StartAsync`, and `StopAsync`; `INotificationHubApplicationContext` exposes `ContentRootPath`.
`INotificationHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific
verbs. The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`NotificationHubApplication.CreateBuilder(args)` returns the public concrete
`NotificationHubApplicationBuilder`; its `Build()` returns the public
`NotificationHubApplication : Host<NotificationHubApplicationContext>`. The public
`NotificationHubApplicationContext` implements `INotificationHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/NotificationHub/Assimalign.Cohesion.NotificationHub/docs/Assembly/Assimalign.Cohesion.NotificationHub/INotificationHubApplication/OVERVIEW.md`.
