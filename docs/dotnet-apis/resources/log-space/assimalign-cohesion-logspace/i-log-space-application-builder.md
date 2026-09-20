# ILogSpaceApplicationBuilder

The `ILogSpaceApplicationBuilder` type belongs to `Assimalign.Cohesion.LogSpace`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.LogSpace` Assembly: `Assimalign.Cohesion.LogSpace`

## Purpose

`ILogSpaceApplicationBuilder` is the public composition seam for a LogSpace application. It exposes
`Build()` returning `ILogSpaceApplication`.

## Surface and behavior

- **`Build()`** — creates a configured LogSpace application.

The current filler builder has no area feature or service registrations by default. Services
registered on the concrete Hosting builder start in registration order and stop in reverse order.
The public concrete `LogSpaceApplicationBuilder` lives in `Assimalign.Cohesion.LogSpace.Hosting`.

## Exceptions

The concrete Hosting builder's `AddService` throws `ArgumentNullException` for a null service or
factory. `Build()` throws `InvalidOperationException` when a service factory returns null.

## Usage

See the [source-backed usage examples](examples/index.md).

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `ILogSpaceApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `ILogSpaceApplicationContext` exposes `ContentRootPath`.
`ILogSpaceApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`LogSpaceApplication.CreateBuilder(args)` returns the public concrete `LogSpaceApplicationBuilder`;
its `Build()` returns the public `LogSpaceApplication : Host<LogSpaceApplicationContext>`. The
public `LogSpaceApplicationContext` implements `ILogSpaceApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace/docs/Assembly/Assimalign.Cohesion.LogSpace/ILogSpaceApplicationBuilder/OVERVIEW.md`.
