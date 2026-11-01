# SchedulerApplication

The `SchedulerApplication` type belongs to `Assimalign.Cohesion.Scheduler.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Scheduler.Hosting` Assembly: `Assimalign.Cohesion.Scheduler.Hosting`

## Purpose

`SchedulerApplication` is the public concrete application and creation entry point for the Scheduler hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `SchedulerApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Usage

See the [source-backed usage examples](examples/index.md) and the [assembly overview](index.md) for
the area's runtime behavior. `RunAsync` delegates to the shared host runner.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/src/SchedulerApplication.cs`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/docs/OVERVIEW.md`.
