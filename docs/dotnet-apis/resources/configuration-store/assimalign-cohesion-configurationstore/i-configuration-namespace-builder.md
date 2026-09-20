# IConfigurationNamespaceBuilder

The `IConfigurationNamespaceBuilder` type belongs to `Assimalign.Cohesion.ConfigurationStore`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.ConfigurationStore` Assembly:
`Assimalign.Cohesion.ConfigurationStore`

## Purpose

`IConfigurationNamespaceBuilder` declares the first-start entries for one named namespace.

## API

See the [source-backed usage examples](examples/index.md).

`Set` accepts a nonblank key without `/` and a string or null value, replaces an earlier declaration
for the same key in the callback, and returns the builder for chaining. The wire protocol reserves
`/` to separate a namespace from its entry key. Declarations seed only missing durable namespaces;
they are not reapplied over values changed through the command endpoint.

## Links

- **Detail** — [Builder](i-configuration-store-application-builder.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/docs/Assembly/Assimalign.Cohesion.ConfigurationStore/IConfigurationNamespaceBuilder/OVERVIEW.md`.
