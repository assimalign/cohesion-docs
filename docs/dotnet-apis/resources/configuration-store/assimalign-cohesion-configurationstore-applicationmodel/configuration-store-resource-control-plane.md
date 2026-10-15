# ConfigurationStoreResourceControlPlane

The `ConfigurationStoreResourceControlPlane` type belongs to `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

## Purpose

`ConfigurationStoreResourceControlPlane` supplies the default control-plane factory used by enabled
configuration-store executables.

## `Create`

See the [source-backed usage examples](examples/index.md).

Every call returns a fresh, isolated plane with no observed endpoints and the
`configurationstore.add-namespace`, `configurationstore.set-value`, and
`configurationstore.remove-value` command kinds. Generated `ResourceControlPlane.g.cs` registers the
factory with `ResourceRuntime` and observes the invocation's endpoints before
`ConfigurationStore.Hosting` serves the standard control-plane routes on `api`.

Typed `AddNamespace`, `SetValue`, and `RemoveValue` descriptor verbs declare these kinds.
`AddNamespace` creates an owned namespace with an optional seed; replay preserves its existing values.

## Links

- **[Assembly** — overview](index.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/ConfigurationStoreResourceControlPlane/OVERVIEW.md`.
