# IEventHubApplication

The `IEventHubApplication` type belongs to `Assimalign.Cohesion.EventHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.EventHub` Assembly: `Assimalign.Cohesion.EventHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IEventHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IEventHubApplicationContext` exposes `ContentRootPath`.
`IEventHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`;
its `Build()` returns the public `EventHubApplication : Host<EventHubApplicationContext>`. The
public `EventHubApplicationContext` implements `IEventHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub/docs/Assembly/Assimalign.Cohesion.EventHub/IEventHubApplication/OVERVIEW.md`.
