# ClientCredential

The `ClientCredential` type belongs to `Assimalign.Cohesion.ConfigurationStore.Client`.

> **Status:** Partial.

`ClientCredential` carries the opaque Bearer token attached to every ConfigurationStore request.

## API

See the [source-backed usage examples](examples/index.md).

The constructor rejects a null, empty, or whitespace token with `ArgumentException`. The client
does not parse, validate, refresh, or persist the token. The token is not exposed by the public
surface, and `ToString()` always redacts it.

Credentials are bound when `ConfigurationStoreClient.Create` constructs a client. Item 25 can create
a new lightweight client for each reconciled credential while the underlying transport remains
shared.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [ConfigurationStoreClient](configuration-store-client.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Client/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.Client/ClientCredential/OVERVIEW.md`.
