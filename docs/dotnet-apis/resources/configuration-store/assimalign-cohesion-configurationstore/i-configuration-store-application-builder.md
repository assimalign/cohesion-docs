# IConfigurationStoreApplicationBuilder

The `IConfigurationStoreApplicationBuilder` type belongs to `Assimalign.Cohesion.ConfigurationStore`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.ConfigurationStore` Assembly:
`Assimalign.Cohesion.ConfigurationStore`

## Purpose

`IConfigurationStoreApplicationBuilder` is the public composition seam for a configuration store
application. It declares area composition and `Build()` returning `IConfigurationStoreApplication`.

## Surface and behavior

- **`AddNamespace(string name, Action<IConfigurationNamespaceBuilder> configure)`** — declares first-start values for one durable namespace.
- **`Build()`** — creates a configured configuration store application.

The Hosting implementation appends its protocol listener after explicit services, so dependencies
start before the listener accepts requests and drain after it. Namespace declarations never replace
an existing durable namespace document. The concrete builder is public in
ConfigurationStore.Hosting.

## Exceptions

`AddNamespace` rejects a blank or duplicate name and a null callback. The concrete Hosting builder's
`AddService` rejects a null service or factory. `Build()` rejects reuse and a factory that returns
null.

## Usage

See the [source-backed usage examples](examples/index.md).

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `IConfigurationStoreApplication` exposes `Context`,
`StartAsync`, and `StopAsync`; `IConfigurationStoreApplicationContext` exposes `ContentRootPath`.
`IConfigurationStoreApplicationBuilder` owns area declarations and `Build()`. The root and feature
packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces the boundary.

`ConfigurationStoreApplication.CreateBuilder(args)` returns the public concrete
`ConfigurationStoreApplicationBuilder`; its `Build()` returns the public
`ConfigurationStoreApplication : Host<ConfigurationStoreApplicationContext>`. The public
`ConfigurationStoreApplicationContext` implements `IConfigurationStoreApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/docs/Assembly/Assimalign.Cohesion.ConfigurationStore/IConfigurationStoreApplicationBuilder/OVERVIEW.md`.
