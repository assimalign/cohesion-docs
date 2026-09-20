# IConfigurationStoreResourceDescriptor

The `IConfigurationStoreResourceDescriptor` type is part of the documented `Assimalign.Cohesion.ConfigurationStore.ApplicationModel` API.

> **Status:** Partial.

A typed ConfigurationStore graph descriptor for declaring dependencies and resource commands.

Namespace: `Assimalign.Cohesion.ConfigurationStore.ApplicationModel`.

## Documented behavior

`IConfigurationStoreResourceDescriptor` retains typed dependency chaining and declarative commands.
`ConfigurationStoreResourceCommandExtensions` contributes `SetValue` and `RemoveValue`; both use
the shared `IResourceCommandDescriptor` seam and source-generated payload metadata.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.ConfigurationStore.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/src/Abstractions/IConfigurationStoreResourceDescriptor.cs`.
