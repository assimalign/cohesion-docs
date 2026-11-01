# ApiManagerApplication

The `ApiManagerApplication` type belongs to `Assimalign.Cohesion.ApiManager.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ApiManager.Hosting` Assembly: `Assimalign.Cohesion.ApiManager.Hosting`

## Purpose

`ApiManagerApplication` is the public concrete application and creation entry point for the ApiManager hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns an `ApiManagerApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Exceptions

`CreateBuilder` throws `ArgumentNullException` when `args` is `null`. Building throws
`InvalidOperationException` when a registered service factory returns null or the builder was
already built.

## Usage

See the [source-backed usage examples](examples/index.md). `RunAsync` delegates to the shared
host runner; without a registration the host creates no listener.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Hosting/src/ApiManagerApplication.cs`.
- **Source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.Hosting/docs/OVERVIEW.md`.
