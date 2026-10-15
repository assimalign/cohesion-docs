# ISchedulerApplication

The `ISchedulerApplication` type belongs to `Assimalign.Cohesion.Scheduler`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Scheduler` Assembly: `Assimalign.Cohesion.Scheduler`

## Purpose and surface

The root contracts are hosting-free (O34): `ISchedulerApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `ISchedulerApplicationContext` exposes `ContentRootPath` plus the immutable
`Jobs` and `ScheduleProviders` snapshots. `ISchedulerApplicationBuilder` owns area declarations and
`Build()`. The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library;
`COHRES004` enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`SchedulerApplication.CreateBuilder(args)` returns the public concrete `SchedulerApplicationBuilder`
; its `Build()` returns the public `SchedulerApplication : Host<SchedulerApplicationContext>`. The
public `SchedulerApplicationContext` implements `ISchedulerApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/docs/Assembly/Assimalign.Cohesion.Scheduler/ISchedulerApplication/OVERVIEW.md`.
