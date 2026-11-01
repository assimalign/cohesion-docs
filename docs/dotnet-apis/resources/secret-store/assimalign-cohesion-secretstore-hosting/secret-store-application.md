# SecretStoreApplication

The `SecretStoreApplication` type belongs to `Assimalign.Cohesion.SecretStore.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore.Hosting` Assembly: `Assimalign.Cohesion.SecretStore.Hosting`

## Purpose

`SecretStoreApplication` is the public concrete application and creation entry point for the SecretStore hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `SecretStoreApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Usage

See the [source-backed usage examples](examples/index.md) and the [assembly overview](index.md) for
the area's runtime behavior. `RunAsync` delegates to the shared host runner.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/src/SecretStoreApplication.cs`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/OVERVIEW.md`.
