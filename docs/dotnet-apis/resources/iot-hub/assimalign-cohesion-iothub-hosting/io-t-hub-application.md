# IoTHubApplication

The `IoTHubApplication` type belongs to `Assimalign.Cohesion.IoTHub.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.IoTHub.Hosting` Assembly: `Assimalign.Cohesion.IoTHub.Hosting`

## Purpose

`IoTHubApplication` is the public concrete application and creation entry point for the IoTHub hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns an `IoTHubApplicationBuilder`.
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

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/src/IoTHubApplication.cs`.
- **Source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting/docs/OVERVIEW.md`.
