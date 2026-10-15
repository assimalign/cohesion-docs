# IIoTHubApplication

The `IIoTHubApplication` type belongs to `Assimalign.Cohesion.IoTHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.IoTHub` Assembly: `Assimalign.Cohesion.IoTHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IIoTHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IIoTHubApplicationContext` exposes `ContentRootPath`.
`IIoTHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`IoTHubApplication.CreateBuilder(args)` returns the public concrete `IoTHubApplicationBuilder`; its
`Build()` returns the public `IoTHubApplication : Host<IoTHubApplicationContext>`. The public
`IoTHubApplicationContext` implements `IIoTHubApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub/docs/Assembly/Assimalign.Cohesion.IoTHub/IIoTHubApplication/OVERVIEW.md`.
