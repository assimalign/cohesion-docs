# IApiManagerApplication

The `IApiManagerApplication` type belongs to `Assimalign.Cohesion.ApiManager`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ApiManager` Assembly: `Assimalign.Cohesion.ApiManager`

## Purpose and surface

The root contracts are hosting-free (O34): `IApiManagerApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `IApiManagerApplicationContext` exposes `ContentRootPath`.
`IApiManagerApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`ApiManagerApplication.CreateBuilder(args)` returns the public concrete
`ApiManagerApplicationBuilder`; its `Build()` returns the public
`ApiManagerApplication : Host<ApiManagerApplicationContext>`. The public
`ApiManagerApplicationContext` implements `IApiManagerApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager/docs/Assembly/Assimalign.Cohesion.ApiManager/IApiManagerApplication/OVERVIEW.md`.
