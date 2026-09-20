# SecretStoreResourceExtensions

The `SecretStoreResourceExtensions` type belongs to `Assimalign.Cohesion.SecretStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.SecretStore.ApplicationModel`

## Purpose

`SecretStoreResourceExtensions` contributes the C# extension member that adds a manifest-backed
secret store to an `IApplicationBuilder`.

## `AddSecretStore`

See the [source-backed usage examples](examples/index.md).

`AddSecretStore(ResourceManifest, SecretStoreResourceOptions?)` creates the typed resource, adds it
to the application graph, and returns the area-typed descriptor used for dependency chaining.
`descriptor.Resource` is a `SecretStoreResource`. Planning remains deferred until the graph is
built, where the planner enforces exactly one effective replica. A null manifest raises
`ArgumentNullException`.

The returned descriptor exposes `AddSecret` and `IssueCertificate` through
`SecretStoreResourceCommandExtensions`. `Enroll(platformStore)` is deferred to item 31t.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [SecretStoreResource](secret-store-resource.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.SecretStore.ApplicationModel/SecretStoreResourceExtensions/OVERVIEW.md`.
