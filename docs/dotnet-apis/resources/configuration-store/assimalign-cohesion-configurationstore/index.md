# Assimalign.Cohesion.ConfigurationStore

This project defines the public, contract-only builder and application lifecycle seam for the ConfigurationStore area.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IConfigurationNamespaceBuilder`](i-configuration-namespace-builder.md)** — Documented public type.
- **[`IConfigurationStoreApplication`](i-configuration-store-application.md)** — Documented public type.
- **[`IConfigurationStoreApplicationBuilder`](i-configuration-store-application-builder.md)** — Documented public type.

## Summary

This project defines the public, contract-only builder and application lifecycle seam for the
ConfigurationStore area. The implementation and creation entry point live in
`Assimalign.Cohesion.ConfigurationStore.Hosting`.

## Public surface

- **`IConfigurationStoreApplicationBuilder`** — owns area declarations and builds an `IConfigurationStoreApplication`.
- **`IConfigurationStoreApplicationBuilder.AddNamespace`** — declares a durable namespace's first-start values.
- **`IConfigurationNamespaceBuilder.Set`** — declares a string or null entry.
- **`IConfigurationStoreApplication`** — exposes `Context`, `StartAsync`, and `StopAsync`.

The concrete Hosting implementation adds the protocol endpoint automatically. Explicit lifecycle
services compose through the public concrete `ConfigurationStoreApplicationBuilder`.

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

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Parent: ConfigurationStore](../index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/src/Assimalign.Cohesion.ConfigurationStore.csproj`.
