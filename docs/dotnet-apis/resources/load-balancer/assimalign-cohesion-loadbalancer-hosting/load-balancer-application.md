# LoadBalancerApplication

The `LoadBalancerApplication` type belongs to `Assimalign.Cohesion.LoadBalancer.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.LoadBalancer.Hosting` Assembly: `Assimalign.Cohesion.LoadBalancer.Hosting`

## Purpose

`LoadBalancerApplication` is the public concrete application and creation entry point for the LoadBalancer hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `LoadBalancerApplicationBuilder`.
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

- **Primary source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/src/LoadBalancerApplication.cs`.
- **Source** — `cohesion/resources/LoadBalancer/Assimalign.Cohesion.LoadBalancer.Hosting/docs/OVERVIEW.md`.
