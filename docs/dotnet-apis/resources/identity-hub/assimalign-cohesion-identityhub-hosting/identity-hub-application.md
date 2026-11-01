# IdentityHubApplication

The `IdentityHubApplication` type belongs to `Assimalign.Cohesion.IdentityHub.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.IdentityHub.Hosting` Assembly: `Assimalign.Cohesion.IdentityHub.Hosting`

## Purpose

`IdentityHubApplication` is the public concrete application and creation entry point for the IdentityHub hosting
module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns an `IdentityHubApplicationBuilder`.
- **Building the returned builder** — closes registration and materializes the registered host
  services in registration order, including the resource service `AddResource` registers for a
  managed instance.

## Usage

See the [source-backed usage examples](examples/index.md) and the [assembly overview](index.md) for
the area's runtime behavior. `RunAsync` delegates to the shared host runner.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/src/IdentityHubApplication.cs`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/OVERVIEW.md`.
