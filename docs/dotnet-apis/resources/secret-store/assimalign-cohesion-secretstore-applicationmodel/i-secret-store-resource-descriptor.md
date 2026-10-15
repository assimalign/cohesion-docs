# ISecretStoreResourceDescriptor

The `ISecretStoreResourceDescriptor` type belongs to `Assimalign.Cohesion.SecretStore.ApplicationModel`.

> **Status:** Partial.

`ISecretStoreResourceDescriptor` is the area-typed descriptor returned by `AddSecretStore`. Its
`Resource` property exposes `SecretStoreResource` directly while the inherited application-model
members retain ordinary dependency chaining.

The concrete descriptor is internal. `Application` code depends only on this public contract and the
shared `IApplicationResourceDescriptor` surface.

## See also

- **Detail** — [SecretStoreResource](secret-store-resource.md)
- **Detail** — [SecretStoreResourceExtensions](secret-store-resource-extensions.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.SecretStore.ApplicationModel/ISecretStoreResourceDescriptor/OVERVIEW.md`.
