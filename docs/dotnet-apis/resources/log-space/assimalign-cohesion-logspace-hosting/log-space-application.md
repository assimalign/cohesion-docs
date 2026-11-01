# LogSpaceApplication

The `LogSpaceApplication` type belongs to `Assimalign.Cohesion.LogSpace.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.LogSpace.Hosting` Assembly: `Assimalign.Cohesion.LogSpace.Hosting`

## Purpose

`LogSpaceApplication` is the public concrete application and creation entry point for the LogSpace hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `LogSpaceApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Usage

See the [source-backed usage examples](examples/index.md) and the [assembly overview](index.md) for
the area's runtime behavior. `RunAsync` delegates to the shared host runner.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/src/LogSpaceApplication.cs`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/OVERVIEW.md`.
