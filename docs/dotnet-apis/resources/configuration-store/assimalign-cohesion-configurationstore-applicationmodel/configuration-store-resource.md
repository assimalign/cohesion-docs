# ConfigurationStoreResource

The `ConfigurationStoreResource` type belongs to `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

## Purpose

`ConfigurationStoreResource` is the typed `PlannedResource` for an enabled configuration-store
executable. Construction snapshots the supplied `ResourceManifest`; the snapshot remains the source
of truth for artifact, endpoint, mount, lifecycle, setting, and reference facts.

## Construction

See the [source-backed usage examples](examples/index.md).

The options argument is optional. A null manifest raises `ArgumentNullException`.

## Planning

`PlannerName` returns `ConfigurationStore planner`. `CreatePlan(context)` requires the context to
name the same resource as the snapshot and emits a platform-neutral `cohesion/plan/v1` plan. The
manifest must describe kind `ConfigurationStore`, a `StatefulSet`, exactly the `api` endpoint, and
exactly the `data` Volume. The resulting plan has exactly one replica with stable identity, a sized
per-replica claim, an API service, and a headless governing service.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [Options](configuration-store-resource-options.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/ConfigurationStoreResource/OVERVIEW.md`.
