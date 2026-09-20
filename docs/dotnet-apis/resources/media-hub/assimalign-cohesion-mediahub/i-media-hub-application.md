# IMediaHubApplication

The `IMediaHubApplication` type belongs to `Assimalign.Cohesion.MediaHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.MediaHub` Assembly: `Assimalign.Cohesion.MediaHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IMediaHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IMediaHubApplicationContext` exposes `ContentRootPath`.
`IMediaHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`MediaHubApplication.CreateBuilder(args)` returns the public concrete `MediaHubApplicationBuilder`;
its `Build()` returns the public `MediaHubApplication : Host<MediaHubApplicationContext>`. The
public `MediaHubApplicationContext` implements `IMediaHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub/docs/Assembly/Assimalign.Cohesion.MediaHub/IMediaHubApplication/OVERVIEW.md`.
