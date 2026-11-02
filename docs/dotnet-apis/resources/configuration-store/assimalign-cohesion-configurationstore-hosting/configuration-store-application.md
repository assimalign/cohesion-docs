# ConfigurationStoreApplication

The `ConfigurationStoreApplication` type belongs to `Assimalign.Cohesion.ConfigurationStore.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.Hosting` Assembly: `Assimalign.Cohesion.ConfigurationStore.Hosting`

## Purpose

`ConfigurationStoreApplication` is the public concrete application and creation entry point for the ConfigurationStore hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `ConfigurationStoreApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Usage

See the [source-backed usage examples](examples/index.md) and the [assembly overview](index.md) for
the area's runtime behavior. `RunAsync` delegates to the shared host runner.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/src/ConfigurationStoreApplication.cs`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/OVERVIEW.md`.
