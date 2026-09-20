# SecretStoreResourceOptions

The `SecretStoreResourceOptions` type belongs to `Assimalign.Cohesion.SecretStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.SecretStore.ApplicationModel`

## Purpose

`SecretStoreResourceOptions` supplies deployer-owned planning overrides for a `SecretStoreResource`
. It derives from `ResourceOptions` without introducing a second area-specific storage model.

## Surface

- **`Replicas`** — comes from the shared options type, but must be unset or one until a
  replication protocol exists.
- **`Storage.Size`** — overrides the declared size of the per-replica `data` claim when set.

Endpoint names, ports, exposure, mount names and paths, and runtime environment values remain
build-produced manifest facts.

See the [source-backed usage examples](examples/index.md).

The planner rejects an effective replica count other than one, whether it comes from the manifest or
a deployer override. This prevents unsafe independent secret-store instances while replication and
consensus are unavailable. The planner also requires the final `data` size to be non-empty.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [SecretStoreResource](secret-store-resource.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.SecretStore.ApplicationModel/SecretStoreResourceOptions/OVERVIEW.md`.
