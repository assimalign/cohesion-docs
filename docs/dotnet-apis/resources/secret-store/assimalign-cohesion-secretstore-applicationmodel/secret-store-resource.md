# SecretStoreResource

The `SecretStoreResource` type belongs to `Assimalign.Cohesion.SecretStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.SecretStore.ApplicationModel`

## Purpose

`SecretStoreResource` is the typed `PlannedResource` for an enabled secret-store executable.
Construction snapshots the supplied `ResourceManifest`; the snapshot remains the source of truth
for artifact, endpoint, mount, lifecycle, setting, reference, and control-plane facts.

## Construction

See the [source-backed usage examples](examples/index.md).

The options argument is optional. A null manifest raises `ArgumentNullException`.

## Planning

`PlannerName` returns `SecretStore planner`. `CreatePlan(context)` requires the context to name the
same resource as the snapshot and emits a platform-neutral `cohesion/plan/v1` plan. The manifest
must describe kind `SecretStore`, a `StatefulSet`, an HTTP or HTTPS `api` endpoint, the `api`
control plane at `/cohesion/v1`, exactly one effective replica, and exactly one persistent Volume
named `data`. The resulting plan has stable identity, a sized per-replica claim, an API service,
and a headless governing service. Multiple replicas are rejected until the store has a replication
and consensus protocol.

Additional endpoints and non-persistent mounts retain the generic manifest-to-plan mapping.

## Exceptions

`CreatePlan` raises `ArgumentNullException` for a null context and `InvalidOperationException` when
the context or SecretStore shape is incompatible.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [Options](secret-store-resource-options.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.SecretStore.ApplicationModel/SecretStoreResource/OVERVIEW.md`.
