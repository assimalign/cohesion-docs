# IRezolvrApplication

The `IRezolvrApplication` type belongs to `Assimalign.Cohesion.Rezolvr`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Rezolvr` Assembly: `Assimalign.Cohesion.Rezolvr`

## Purpose and surface

The root contracts are hosting-free (O34): `IRezolvrApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IRezolvrApplicationContext` exposes `ContentRootPath`.
`IRezolvrApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`RezolvrApplication.CreateBuilder(args)` returns the public concrete `RezolvrApplicationBuilder`;
its `Build()` returns the public `RezolvrApplication : Host<RezolvrApplicationContext>`. The public
`RezolvrApplicationContext` implements `IRezolvrApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr/docs/Assembly/Assimalign.Cohesion.Rezolvr/IRezolvrApplication/OVERVIEW.md`.
