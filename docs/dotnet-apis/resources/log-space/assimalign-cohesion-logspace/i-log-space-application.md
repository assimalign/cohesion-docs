# ILogSpaceApplication

The `ILogSpaceApplication` type belongs to `Assimalign.Cohesion.LogSpace`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.LogSpace` Assembly: `Assimalign.Cohesion.LogSpace`

## Purpose and surface

The root contracts are hosting-free (O34): `ILogSpaceApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `ILogSpaceApplicationContext` exposes `ContentRootPath`.
`ILogSpaceApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`LogSpaceApplication.CreateBuilder(args)` returns the public concrete `LogSpaceApplicationBuilder`;
its `Build()` returns the public `LogSpaceApplication : Host<LogSpaceApplicationContext>`. The
public `LogSpaceApplicationContext` implements `ILogSpaceApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace/docs/Assembly/Assimalign.Cohesion.LogSpace/ILogSpaceApplication/OVERVIEW.md`.
