# ConfigurationStoreResourceExtensions

The `ConfigurationStoreResourceExtensions` type belongs to `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`

## Purpose

`ConfigurationStoreResourceExtensions` contributes the C# extension member that adds a
manifest-backed configuration store to an `IApplicationBuilder`.

## `AddConfigurationStore`

See the [source-backed usage examples](examples/index.md).

`AddConfigurationStore(ResourceManifest, ConfigurationStoreResourceOptions?)` creates the typed
resource, adds it to the application graph, and returns the ordinary
`IConfigurationStoreResourceDescriptor` used for dependency chaining. Planning remains deferred
until the graph is built. A null manifest raises `ArgumentNullException`.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [ConfigurationStoreResource](configuration-store-resource.md)
- **[Project** — design](design.md)

`RemoteReferenceConfigurationStore(declaration, configure)` returns the same typed descriptor for a
manifest-backed external. Typed descriptors support `SetValue` and `RemoveValue` command
declarations.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/ConfigurationStoreResourceExtensions/OVERVIEW.md`.
