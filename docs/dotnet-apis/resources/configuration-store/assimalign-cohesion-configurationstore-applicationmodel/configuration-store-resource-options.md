# ConfigurationStoreResourceOptions

The `ConfigurationStoreResourceOptions` type belongs to `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

## Purpose

`ConfigurationStoreResourceOptions` supplies deployer-owned planning overrides for a
`ConfigurationStoreResource`. It derives from `ResourceOptions` without adding a second
area-specific options model.

## Surface

- **`Replicas`** — comes from the shared options type, but values other than one are rejected until a replication protocol exists.
- **`Storage.Size`** — overrides the declared size of the per-replica `data` claim when set.

Endpoint names, ports, exposure, mount names and paths, and runtime environment values remain
build-produced manifest facts.

See the [source-backed usage examples](examples/index.md).

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [ConfigurationStoreResource](configuration-store-resource.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/ConfigurationStoreResourceOptions/OVERVIEW.md`.
